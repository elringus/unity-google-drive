using System;
using System.Threading;
using UnityEngine;

namespace UnityGoogleDrive
{
    // <summary>
    // Provides access token using custom URI scheme method to read authorization response.
    // </summary>
    public class AndroidAccessTokenProvider : IAccessTokenProvider
    {
        private struct AuthorizationResponse { public bool IsError; public string Error, CodeVerifier, RedirectUri, AuthorizationCode; };

        private class OnAuthorizationResponseListener : AndroidJavaProxy
        {
            public event Action<AuthorizationResponse> OnAuthResponse;

            public OnAuthorizationResponseListener ()
                : base("com.elringus.unitygoogledriveandroid.AuthorizationActivity$OnAuthorizationResponseListener") { }

            private void onAuthorizationResponse (bool isError, string error, string codeVerifier, string redirectUri, string authorizationCode)
            {
                var response = new AuthorizationResponse {
                    IsError = isError,
                    Error = error,
                    CodeVerifier = codeVerifier,
                    RedirectUri = redirectUri,
                    AuthorizationCode = authorizationCode
                };

                if (OnAuthResponse != null)
                    OnAuthResponse.Invoke(response);
            }

            private void onAuthorizationResponse (bool isError, AndroidJavaObject error, string codeVerifier, string redirectUri, string authorizationCode)
            {
                onAuthorizationResponse(isError, error?.ToString(), codeVerifier, redirectUri, authorizationCode);
            }
        }

        public event Action<IAccessTokenProvider> OnDone;

        public bool IsDone { get; private set; }
        public bool IsError { get; private set; }

        private SynchronizationContext unitySyncContext;
        private GoogleDriveSettings settings;
        private AccessTokenRefresher accessTokenRefresher;
        private AuthCodeExchanger authCodeExchanger;

        public AndroidAccessTokenProvider (GoogleDriveSettings googleDriveSettings)
        {
            settings = googleDriveSettings;
            unitySyncContext = SynchronizationContext.Current;

            accessTokenRefresher = new AccessTokenRefresher(settings.UriSchemeClientCredentials);
            accessTokenRefresher.OnDone += HandleAccessTokenRefreshed;

            authCodeExchanger = new AuthCodeExchanger(settings, settings.UriSchemeClientCredentials);
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
                        message += $"\nDetails: {refresher.Error}";
                    Debug.Log(message);
                }
                ExecuteFullAuth();
            }
            else
            {
                settings.CachedAccessToken = refresher.AccessToken;
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
                settings.CachedAccessToken = authCodeExchanger.AccessToken;
                settings.CachedRefreshToken = authCodeExchanger.RefreshToken;
                HandleProvideAccessTokenComplete();
            }
        }

        private void ExecuteFullAuth ()
        {
            var redirectEndpoint = Application.identifier + ":/oauth2callback";
            var responseListener = new OnAuthorizationResponseListener();
            responseListener.OnAuthResponse += HandleAuthorizationResponse;

            var applicationClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var applicationActivity = applicationClass.GetStatic<AndroidJavaObject>("currentActivity");

            var activityClass = new AndroidJavaClass("com.elringus.unitygoogledriveandroid.AuthorizationActivity");
            activityClass.CallStatic("SetResponseListener", responseListener);

            var intent = new AndroidJavaObject("android.content.Intent", applicationActivity, activityClass);
            intent.Call<AndroidJavaObject>("putExtra", "authorizationEndpoint", ToJavaObject(settings.UriSchemeClientCredentials.AuthUri));
            intent.Call<AndroidJavaObject>("putExtra", "tokenEndpoint", ToJavaObject(settings.UriSchemeClientCredentials.TokenUri));
            intent.Call<AndroidJavaObject>("putExtra", "clientId", ToJavaObject(settings.UriSchemeClientCredentials.ClientId));
            intent.Call<AndroidJavaObject>("putExtra", "redirectEndpoint", ToJavaObject(redirectEndpoint));
            intent.Call<AndroidJavaObject>("putExtra", "scope", ToJavaObject(settings.AccessScope));
            applicationActivity.Call("startActivity", intent);
        }

        private void HandleAuthorizationResponse (AuthorizationResponse response)
        {
            // This method is called on a background thread; rerouting it to the Unity's thread.
            unitySyncContext.Post(HandleAuthorizationResponseOnUnityThread, response);
        }

        private void HandleAuthorizationResponseOnUnityThread (object responseObj)
        {
            var response = (AuthorizationResponse)responseObj;

            if (response.IsError)
            {
                Debug.LogError($"UnityGoogleDrive: OAuth authorization error: {response.Error}.");
                HandleProvideAccessTokenComplete(true);
                return;
            }

            authCodeExchanger.ExchangeAuthCode(response.AuthorizationCode, response.CodeVerifier, response.RedirectUri);
        }

        private static AndroidJavaObject ToJavaObject (string value)
        {
            return new AndroidJavaObject("java.lang.String", value);
        }
    }
}
