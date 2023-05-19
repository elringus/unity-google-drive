using System;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Issues a new access token using provided refresh token.
    /// Protocol: https://developers.google.com/identity/protocols/OAuth2WebServer#offline.
    /// </summary>
    public class AccessTokenRefresher
    {
        #pragma warning disable 0649
        // ReSharper disable NotAccessedField.Local
        [Serializable] struct RefreshResponse { public string error, error_description, access_token, expires_in, token_type; }
        // ReSharper restore NotAccessedField.Local
        #pragma warning restore 0649

        public event Action<AccessTokenRefresher> OnDone;

        public bool IsDone { get; private set; }
        public bool IsError { get; private set; }
        public string Error { get; } = "";
        public string AccessToken { get; private set; }

        private readonly IClientCredentials credentials;
        private UnityWebRequest refreshRequest;

        public AccessTokenRefresher (IClientCredentials clientCredentials)
        {
            credentials = clientCredentials;
        }

        public void RefreshAccessToken (string refreshToken)
        {
            var refreshRequestForm = new WWWForm();
            refreshRequestForm.AddField("client_id", credentials.ClientId);
            if (!string.IsNullOrEmpty(credentials.ClientSecret))
                refreshRequestForm.AddField("client_secret", credentials.ClientSecret);
            refreshRequestForm.AddField("refresh_token", refreshToken);
            refreshRequestForm.AddField("grant_type", "refresh_token");

            refreshRequest = UnityWebRequest.Post(credentials.TokenUri, refreshRequestForm);
            refreshRequest.SetRequestHeader("Content-Type", GoogleDriveSettings.RequestContentType);
            refreshRequest.SetRequestHeader("Accept", "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            refreshRequest.SendWebRequest().completed += HandleRequestComplete;
        }

        private void HandleRefreshComplete (bool error = false)
        {
            IsError = error;
            IsDone = true;
            if (OnDone != null)
                OnDone.Invoke(this);
        }

        private void HandleRequestComplete (AsyncOperation requestYield)
        {
            if (CheckRequestErrors(refreshRequest))
            {
                HandleRefreshComplete(true);
                return;
            }

            var response = JsonUtility.FromJson<RefreshResponse>(refreshRequest.downloadHandler.text);
            AccessToken = response.access_token;
            HandleRefreshComplete();
        }

        private static bool CheckRequestErrors (UnityWebRequest request)
        {
            if (request == null)
            {
                Debug.LogError("UnityGoogleDrive: Refresh token request failed. Request object is null.");
                return true;
            }

            var errorDescription = string.Empty;

            if (!string.IsNullOrEmpty(request.error))
                errorDescription += " HTTP Error: " + request.error;

            if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
            {
                var response = JsonUtility.FromJson<RefreshResponse>(request.downloadHandler.text);
                if (!string.IsNullOrEmpty(response.error))
                    errorDescription += " API Error: " + response.error + " API Error Description: " + response.error_description;
            }

            var isError = errorDescription.Length > 0;
            if (isError) Debug.LogError("UnityGoogleDrive: Refresh token code request failed." + errorDescription);
            return isError;
        }
    }
}
