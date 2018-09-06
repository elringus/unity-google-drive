using System;
using UnityEngine;

namespace UnityGoogleDrive
{
    // <summary>
    // Provides access token using custom URI scheme method to read authorization response.
    // </summary>
    public class AndroidAccessTokenProvider : IAccessTokenProvider
    {
        private struct AuthorizationResponse { public bool IsError; public string Error, AuthorizationCode; };

        private class OnAuthorizationResponseListener : AndroidJavaProxy
        {
            public event Action<AuthorizationResponse> OnAuthResponse;

            public OnAuthorizationResponseListener ()
                : base("net.openid.appauth.AuthorizationClientActivity$OnAuthorizationResponseListener") { }

            private void onAuthorizationResponse (bool isError, string error, string authorizationCode)
            {
                var response = new AuthorizationResponse {
                    IsError = isError,
                    Error = error,
                    AuthorizationCode = authorizationCode
                };

                if (OnAuthResponse != null)
                    OnAuthResponse.Invoke(response);
            }
        }

        public event Action<IAccessTokenProvider> OnDone;

        public bool IsDone { get; private set; }
        public bool IsError { get; private set; }

        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        const string tokenEndpoint = "https://accounts.google.com/o/oauth2/token";

        private GoogleDriveSettings settings;
        private AccessTokenRefresher accessTokenRefresher;
        private AuthCodeExchanger authCodeExchanger;

        public AndroidAccessTokenProvider (GoogleDriveSettings googleDriveSettings)
        {
            settings = googleDriveSettings;

            accessTokenRefresher = new AccessTokenRefresher(settings);
            accessTokenRefresher.OnDone += HandleAccessTokenRefreshed;

            authCodeExchanger = new AuthCodeExchanger(settings);
            authCodeExchanger.OnDone += HandleAuthCodeExchanged;
        }

        public void ProvideAccessToken ()
        {
            if (!settings.UriSchemeClientCredentials.ContainsSensitiveData())
            {
                Debug.LogError("URI Scheme credentials are not valid.");
                HandleProvideAccessTokenComplete(true);
                return;
            }

            // Refresh token isn't available; executing full auth procedure.
            if (string.IsNullOrEmpty(settings.CachedRefreshToken)) ExecuteFullAuth();
            // Using refresh token to issue a new access token.
            else accessTokenRefresher.RefreshAccessToken(settings.CachedRefreshToken);
        }

        private void HandleProvideAccessTokenComplete (bool error = false)
        {
            IsError = error;
            IsDone = true;
            if (OnDone != null)
                OnDone.Invoke(this);
        }

        private void HandleAccessTokenRefreshed (AccessTokenRefresher refresher)
        {
            if (refresher.IsError)
            {
                if (Debug.isDebugBuild)
                {
                    var message = "UnityGoogleDrive: Failed to refresh access token; executing full auth procedure.";
                    if (!string.IsNullOrEmpty(refresher.Error))
                        message += string.Format("\nDetails: {0}", refresher.Error);
                    Debug.Log(message);
                }
                ExecuteFullAuth();
            }
            else
            {
                settings.CachedAccessToken = refresher.AccesToken;
                HandleProvideAccessTokenComplete();
            }
        }

        private void HandleAuthCodeExchanged (AuthCodeExchanger exchanger)
        {
            if (authCodeExchanger.IsError)
            {
                Debug.LogError("UnityGoogleDrive: Failed to exchange authorization code.");
                HandleProvideAccessTokenComplete(true);
            }
            else
            {
                settings.CachedAccessToken = authCodeExchanger.AccesToken;
                settings.CachedRefreshToken = authCodeExchanger.RefreshToken;
                HandleProvideAccessTokenComplete();
            }
        }

        private void ExecuteFullAuth ()
        {
            var redirectEndpoint = Uri.EscapeDataString(settings.UriSchemeClientCredentials.ReversedClientId + ":/oauth2redirect");
            var scope = Uri.EscapeDataString(string.Join(" ", settings.AccessScopes.ToArray()));
            var responseListener = new OnAuthorizationResponseListener();
            responseListener.OnAuthResponse += HandleAuthorizationResponse;

            var applicationClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var applicationActivity = applicationClass.GetStatic<AndroidJavaObject>("currentActivity");
            //var applicationContext = applicationActivity.Call<AndroidJavaObject>("getApplicationContext");

            var activityClass = new AndroidJavaClass("net.openid.appauth.AuthorizationClientActivity");
            activityClass.CallStatic("SetResponseListener", responseListener);

            var intent = new AndroidJavaObject("android.content.Intent", applicationActivity, activityClass);
            intent.Call<AndroidJavaObject>("putExtra", "authorizationEndpoint", ToJavaObject(authorizationEndpoint));
            intent.Call<AndroidJavaObject>("putExtra", "tokenEndpoint", ToJavaObject(tokenEndpoint));
            intent.Call<AndroidJavaObject>("putExtra", "clientId", ToJavaObject(settings.UriSchemeClientCredentials.ClientId));
            intent.Call<AndroidJavaObject>("putExtra", "redirectEndpoint", ToJavaObject(redirectEndpoint));
            intent.Call<AndroidJavaObject>("putExtra", "scope", ToJavaObject(scope));
            applicationActivity.Call("startActivity", intent);

            Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> ExecuteFullAuth END <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
        }

        private void HandleAuthorizationResponse (AuthorizationResponse response)
        {
            Debug.Log("Response: " + response.IsError + " " + response.Error + " " + response.AuthorizationCode);
        }

        private static AndroidJavaObject ToJavaObject (string value)
        {
            return new AndroidJavaObject("java.lang.String", value);
        }
    }
}
