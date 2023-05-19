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
        // ReSharper disable NotAccessedField.Local
        [Serializable] struct ExchangeResponse { public string error, error_description, access_token, refresh_token, expires_in, id_token, token_type; }
        // ReSharper restore NotAccessedField.Local
        #pragma warning restore 0649

        public event Action<AuthCodeExchanger> OnDone;

        public bool IsDone { get; private set; }
        public bool IsError { get; private set; }
        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

        private readonly GoogleDriveSettings settings;
        private readonly IClientCredentials credentials;
        private UnityWebRequest exchangeRequest;

        public AuthCodeExchanger (GoogleDriveSettings googleDriveSettings, IClientCredentials clientCredentials)
        {
            settings = googleDriveSettings;
            credentials = clientCredentials;
        }

        public void ExchangeAuthCode (string authorizationCode, string codeVerifier, string redirectUri)
        {
            var tokenRequestForm = new WWWForm();
            tokenRequestForm.AddField("code", authorizationCode);
            tokenRequestForm.AddField("redirect_uri", redirectUri);
            tokenRequestForm.AddField("client_id", credentials.ClientId);
            tokenRequestForm.AddField("code_verifier", codeVerifier);
            if (!string.IsNullOrEmpty(credentials.ClientSecret))
                tokenRequestForm.AddField("client_secret", credentials.ClientSecret);
            tokenRequestForm.AddField("scope", settings.AccessScope);
            tokenRequestForm.AddField("grant_type", "authorization_code");

            exchangeRequest = UnityWebRequest.Post(credentials.TokenUri, tokenRequestForm);
            exchangeRequest.SetRequestHeader("Content-Type", GoogleDriveSettings.RequestContentType);
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

        private void HandleRequestComplete (AsyncOperation requestYield)
        {
            if (CheckRequestErrors(exchangeRequest))
            {
                HandleExchangeComplete(true);
                return;
            }

            var response = JsonUtility.FromJson<ExchangeResponse>(exchangeRequest.downloadHandler.text);
            AccessToken = response.access_token;
            RefreshToken = response.refresh_token;
            HandleExchangeComplete();
        }

        private static bool CheckRequestErrors (UnityWebRequest request)
        {
            if (request == null)
            {
                Debug.LogError("UnityGoogleDrive: Exchange auth code request failed. Request object is null.");
                return true;
            }

            var errorDescription = string.Empty;

            if (!string.IsNullOrEmpty(request.error))
                errorDescription += " HTTP Error: " + request.error;

            if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
            {
                var response = JsonUtility.FromJson<ExchangeResponse>(request.downloadHandler.text);
                if (!string.IsNullOrEmpty(response.error))
                    errorDescription += " API Error: " + response.error + " API Error Description: " + response.error_description;
            }

            var isError = errorDescription.Length > 0;
            if (isError) Debug.LogError("UnityGoogleDrive: Exchange auth code request failed." + errorDescription);
            return isError;
        }
    }
}
