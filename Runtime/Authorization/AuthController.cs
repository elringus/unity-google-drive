using System;
using UnityEngine;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Controls authorization procedures and provides token to access Google APIs.
    /// Implementation based on Google OAuth 2.0 protocol: https://developers.google.com/identity/protocols/OAuth2.
    /// </summary>
    public static class AuthController
    {
        /// <summary>
        /// Invoked when <see cref="AccessToken"/> has been refreshed.
        /// Return false on authorization fail.
        /// </summary>
        public static event Action<bool> OnAccessTokenRefreshed;

        public static string AccessToken => settings.CachedAccessToken;
        public static bool IsRefreshingAccessToken { get; private set; }

        private static GoogleDriveSettings settings;
        private static IAccessTokenProvider accessTokenProvider;

        static AuthController ()
        {
            settings = GoogleDriveSettings.LoadFromResources();

            #if UNITY_WEBGL && !UNITY_EDITOR // WebGL doesn't support loopback method; using redirection scheme instead.
            accessTokenProvider = new RedirectAccessTokenProvider(settings);
            #elif UNITY_ANDROID && !UNITY_EDITOR // On Android a native OpenID lib is used for better UX.
            accessTokenProvider = new AndroidAccessTokenProvider(settings);
            #elif UNITY_IOS && !UNITY_EDITOR // On iOS a native OpenID lib is used for better UX.
            accessTokenProvider = new IOSAccessTokenProvider(settings);
            #else // Loopback scheme is used on other platforms.
            accessTokenProvider = new LoopbackAccessTokenProvider(settings);
            #endif
        }

        public static void RefreshAccessToken ()
        {
            if (IsRefreshingAccessToken) return;
            IsRefreshingAccessToken = true;

            accessTokenProvider.OnDone += HandleAccessTokenProviderDone;
            accessTokenProvider.ProvideAccessToken();
        }

        public static void CancelAuth ()
        {
            if (IsRefreshingAccessToken)
                HandleAccessTokenProviderDone(accessTokenProvider);
        }

        private static void HandleAccessTokenProviderDone (IAccessTokenProvider provider)
        {
            accessTokenProvider.OnDone -= HandleAccessTokenProviderDone;

            var authFailed = !provider.IsDone || provider.IsError;

            if (authFailed)
                Debug.LogError("UnityGoogleDrive: Failed to execute authorization procedure. Check application settings and credentials.");

            IsRefreshingAccessToken = false;

            if (OnAccessTokenRefreshed != null)
                OnAccessTokenRefreshed.Invoke(!authFailed);
        }
    }
}
