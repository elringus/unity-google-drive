using System;
using UnityEngine;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Controls authorization procedures and provides token to access Google APIs.
    /// Implementation based on Google OAuth 2.0 protocol: https://developers.google.com/identity/protocols/OAuth2.
    /// </summary>
    public class AuthController
    {
        /// <summary>
        /// Invoked when <see cref="AccessToken"/> has been refreshed.
        /// Return false on authorization fail.
        /// </summary>
        public event Action<bool> OnAccessTokenRefreshed;

        public string AccessToken { get { return settings.CachedAccessToken; } }
        public bool IsRefreshingAccessToken { get; private set; }

        private GoogleDriveSettings settings;
        private IAccessTokenProvider accessTokenProvider;

        public AuthController (GoogleDriveSettings googleDriveSettings)
        {
            settings = googleDriveSettings;

            // WebGL doesn't support loopback method; using redirection scheme instead.
            #if UNITY_WEBGL && !UNITY_EDITOR
            accessTokenProvider = new RedirectAccessTokenProvider(settings);
            #else
            accessTokenProvider = new LoopbackAccessTokenProvider(settings);
            #endif

            accessTokenProvider.OnDone += HandleAccessTokenProviderDone;
        }

        public void RefreshAccessToken ()
        {
            if (IsRefreshingAccessToken) return;
            IsRefreshingAccessToken = true;

            accessTokenProvider.ProvideAccessToken();
        }

        private void HandleAccessTokenProviderDone (IAccessTokenProvider provider)
        {
            if (provider.IsError)
            {
                Debug.LogError("UnityGoogleDrive: Failed to execute authorization procedure. Check application settings and credentials.");
            }
            else IsRefreshingAccessToken = false;

            if (OnAccessTokenRefreshed != null)
                OnAccessTokenRefreshed.Invoke(!provider.IsError);
        }
    }
}
