using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Retrieves access and refresh tokens using provided authorization code.
    /// Protocol: https://developers.google.com/identity/protocols/OAuth2WebServer#exchange-authorization-code.
    /// </summary>
    public class AuthCodeExchanger
    {
        #pragma warning disable 0649
        [Serializable] struct ExchangeResponse { public string error, error_description, access_token, refresh_token, expires_in, id_token, token_type; }
        #pragma warning restore 0649

        public event Action<AuthCodeExchanger> OnDone;

        public bool IsDone { get; private set; }
        public bool IsError { get; private set; }
        public string AccesToken { get; private set; }
        public string RefreshToken { get; private set; }

        private GoogleDriveSettings settings;
        private UnityWebRequest exchangeRequest;

        public AuthCodeExchanger (GoogleDriveSettings googleDriveSettings)
        {
            settings = googleDriveSettings;
        }

        public void ExchangeAuthCode (string authorizationCode, string codeVerifier, string redirectUri)
        {
            var tokenRequestURI = settings.AuthCredentials.TokenUri;

            var tokenRequestForm = new WWWForm();
            tokenRequestForm.AddField("code", authorizationCode);
            tokenRequestForm.AddField("redirect_uri", redirectUri);
            tokenRequestForm.AddField("client_id", settings.AuthCredentials.ClientId);
            tokenRequestForm.AddField("code_verifier", codeVerifier);
            tokenRequestForm.AddField("client_secret", settings.AuthCredentials.ClientSecret);
            tokenRequestForm.AddField("scope", settings.AccessScope);
            tokenRequestForm.AddField("grant_type", "authorization_code");

            exchangeRequest = UnityWebRequest.Post(tokenRequestURI, tokenRequestForm);
            exchangeRequest.SetRequestHeader("Content-Type", GoogleDriveSettings.REQUEST_CONTENT_TYPE);
            exchangeRequest.SetRequestHeader("Accept", "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            exchangeRequest.SendWebRequest().completed += HandleRequestComplete;
        }

        private void HandleExchangeComplete (bool error = false)
        {
            IsError = error;
            IsDone = true;
            if (OnDone != null)
                OnDone.Invoke(this);
        }

        private void HandleRequestComplete (AsyncOperation requestYeild)
        {
            if (exchangeRequest == null)
            {
                HandleExchangeComplete(true);
                return;
            }

            if (!string.IsNullOrEmpty(exchangeRequest.error))
            {
                Debug.LogError(exchangeRequest.error);
                HandleExchangeComplete(true);
                return;
            }

            var response = JsonUtility.FromJson<ExchangeResponse>(exchangeRequest.downloadHandler.text);
            if (!string.IsNullOrEmpty(response.error))
            {
                Debug.LogError(string.Format("UnityGoogleDrive: {0}: {1}", response.error, response.error_description));
                HandleExchangeComplete(true);
                return;
            }

            AccesToken = response.access_token;
            RefreshToken = response.refresh_token;
            HandleExchangeComplete();
        }
    }
}
