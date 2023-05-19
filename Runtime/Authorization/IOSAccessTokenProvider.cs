using System;
// ReSharper disable once RedundantUsingDirective
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityGoogleDrive
{
    // <summary>
    // Provides access token using custom URI scheme method to read authorization response.
    // </summary>
    public class IOSAccessTokenProvider : IAccessTokenProvider
    {
        private class ResponseMessageHandler : MonoBehaviour
        {
            public event Action<string> OnResponse;

            private void HandleResponseMessage (string message)
            {
                if (OnResponse != null) OnResponse.Invoke(message);
            }
        }

        public event Action<IAccessTokenProvider> OnDone;

        public bool IsDone { get; private set; }
        public bool IsError { get; private set; }

        private const string responseHandlerObjectName = "UnityGoogleDrive_IOSAccessTokenProvider_ResponseHandler";
        private GoogleDriveSettings settings;
        private AccessTokenRefresher accessTokenRefresher;
        private AuthCodeExchanger authCodeExchanger;
        private ResponseMessageHandler responseMessageHandler;

        public IOSAccessTokenProvider (GoogleDriveSettings googleDriveSettings)
        {
            settings = googleDriveSettings;

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
            var responseHandlerObject = new GameObject(responseHandlerObjectName);
            responseMessageHandler = responseHandlerObject.AddComponent<ResponseMessageHandler>();
            responseMessageHandler.OnResponse += HandleAuthorizationResponse;

            #if UNITY_IOS
            _UnityGoogleDriveIOS_PerformAuth(
                settings.UriSchemeClientCredentials.AuthUri,
                settings.UriSchemeClientCredentials.TokenUri,
                settings.UriSchemeClientCredentials.ClientId,
                Application.identifier.ToLowerInvariant() + ":/oauth2callback",
                settings.AccessScope);
            #endif
        }

        private void HandleAuthorizationResponse (string response)
        {
            if (responseMessageHandler) UnityEngine.Object.Destroy(responseMessageHandler.gameObject);

            if (string.IsNullOrEmpty(response) || response.Trim().StartsWith("Error"))
            {
                Debug.LogError($"UnityGoogleDrive: OAuth authorization error: {response}.");
                HandleProvideAccessTokenComplete(true);
                return;
            }

            var splittedResponse = response.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (splittedResponse.Length != 3)
            {
                Debug.LogError($"UnityGoogleDrive: Malformed OAuth authorization response: {response}.");
                HandleProvideAccessTokenComplete(true);
                return;
            }

            authCodeExchanger.ExchangeAuthCode(splittedResponse[0], splittedResponse[1], splittedResponse[2]);
        }

        #if UNITY_IOS
        [DllImport("__Internal")]
        extern static private void _UnityGoogleDriveIOS_PerformAuth (string authorizationEndpoint, string tokenEndpoint, string clientId, string redirectEndpoint, string scope);
        #endif
    }
}
