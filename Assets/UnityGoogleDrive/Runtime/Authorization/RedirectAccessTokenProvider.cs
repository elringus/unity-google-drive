using System;
using System.Linq;
using UnityEngine;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Provides access token extracting it from the redirection url (when running in web).
    /// Protocol: https://developers.google.com/identity/protocols/OAuth2UserAgent.
    /// </summary>
    public class RedirectAccessTokenProvider : IAccessTokenProvider
    {
        public event Action<IAccessTokenProvider> OnDone;

        public bool IsDone { get; private set; }
        public bool IsError { get; private set; }

        private const string TOKEN_ARG_NAME = "access_token";

        private GoogleDriveSettings settings;

        public RedirectAccessTokenProvider (GoogleDriveSettings googleDriveSettings)
        {
            settings = googleDriveSettings;
        }

        public void ProvideAccessToken ()
        {
            if (!settings.AuthCredentials.ContainsSensitiveData())
            {
                HandleProvideAccessTokenComplete(true);
                return;
            }

            var accessToken = ExtractAccessTokenFromApplicationUrl();
            if (string.IsNullOrEmpty(accessToken)) // Access token isn't available; retrieve it.
            {
                var authRequest = string.Format("{0}?response_type=token&scope={1}&redirect_uri={2}&client_id={3}",
                    settings.AuthCredentials.AuthUri,
                    settings.AccessScope,
                    Uri.EscapeDataString(Application.absoluteURL),
                    settings.AuthCredentials.ClientId);

                Application.OpenURL(authRequest);
            }
            else // Access token is already injected to the URL; using it.
            {
                settings.CachedAccessToken = accessToken;
                HandleProvideAccessTokenComplete();
            }
        }

        private void HandleProvideAccessTokenComplete (bool error = false)
        {
            IsError = error;
            IsDone = true;
            if (OnDone != null)
                OnDone.Invoke(this);
        }

        private string ExtractAccessTokenFromApplicationUrl ()
        {
            var applicationUrl = Application.absoluteURL;

            if (!applicationUrl.Contains(TOKEN_ARG_NAME))
                return null;

            var arguments = applicationUrl.Substring(applicationUrl.IndexOf(TOKEN_ARG_NAME)).Split('&')
                .Select(q => q.Split('=')).ToDictionary(q => q.FirstOrDefault(), q => q.Skip(1).FirstOrDefault());

            if (!arguments.ContainsKey(TOKEN_ARG_NAME)) return null;
            else return arguments[TOKEN_ARG_NAME];
        }
    }
}
