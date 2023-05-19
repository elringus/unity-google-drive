using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace UnityGoogleDrive
{
    /// <summary>
    /// Provides access token using local loopback method to read authorization response.
    /// Implementation based on: https://github.com/googlesamples/oauth-apps-for-windows.
    /// </summary>
    public class LoopbackAccessTokenProvider : IAccessTokenProvider
    {
        public event Action<IAccessTokenProvider> OnDone;

        public bool IsDone { get; private set; }
        public bool IsError { get; private set; }

        private SynchronizationContext unitySyncContext;
        private GoogleDriveSettings settings;
        private AccessTokenRefresher accessTokenRefresher;
        private AuthCodeExchanger authCodeExchanger;
        private string expectedState;
        private string codeVerifier;
        private string redirectUri;
        private string authorizationCode;

        public LoopbackAccessTokenProvider (GoogleDriveSettings googleDriveSettings)
        {
            settings = googleDriveSettings;
            unitySyncContext = SynchronizationContext.Current;

            accessTokenRefresher = new AccessTokenRefresher(settings.GenericClientCredentials);
            accessTokenRefresher.OnDone += HandleAccessTokenRefreshed;

            authCodeExchanger = new AuthCodeExchanger(settings, settings.GenericClientCredentials);
            authCodeExchanger.OnDone += HandleAuthCodeExchanged;
        }

        public void ProvideAccessToken ()
        {
            if (!settings.GenericClientCredentials.ContainsSensitiveData())
            {
                Debug.LogError("Generic credentials are not valid.");
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
            // Generate state and PKCE values.
            expectedState = CryptoUtils.RandomDataBase64Uri(32);
            codeVerifier = CryptoUtils.RandomDataBase64Uri(32);
            var codeVerifierHash = CryptoUtils.Sha256(codeVerifier);
            var codeChallenge = CryptoUtils.Base64UriEncodeNoPadding(codeVerifierHash);

            // Creates a redirect URI using an available port on the loopback address.
            redirectUri = $"{settings.LoopbackUri}:{GetRandomUnusedPort()}";

            // Listen for requests on the redirect URI.
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add(redirectUri + '/');
            httpListener.Start();

            // Create the OAuth 2.0 authorization request.
            // https://developers.google.com/identity/protocols/OAuth2WebServer#creatingclient
            var authRequest = string.Format("{0}?response_type=code&scope={1}&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}" +
                    "&access_type=offline" + // Forces to return a refresh token at the auth code exchange phase.
                    "&approval_prompt=force", // Forces to show consent screen for each auth request. Needed to return refresh tokens on consequent auth runs.
                settings.GenericClientCredentials.AuthUri,
                Uri.EscapeDataString(settings.AccessScope),
                Uri.EscapeDataString(redirectUri),
                settings.GenericClientCredentials.ClientId,
                expectedState,
                codeChallenge,
                GoogleDriveSettings.CodeChallengeMethod);

            // Open request in the browser.
            Application.OpenURL(authRequest);

            // Wait for the authorization response.
            var asyncResult = httpListener.BeginGetContext(HandleHttpListenerCallback, httpListener);

            // Block the thread when background mode is not supported to serve HTTP response while the application is not in focus.
            if (!Application.runInBackground)
                asyncResult.AsyncWaitHandle.WaitOne();
        }

        private void HandleHttpListenerCallback (IAsyncResult result)
        {
            // This method is called on a background thread; rerouting it to the Unity's thread.
            unitySyncContext.Send(HandleHttpListenerCallbackOnUnityThread, result);
        }

        private void HandleHttpListenerCallbackOnUnityThread (object state)
        {
            var result = (IAsyncResult)state;
            var httpListener = (HttpListener)result.AsyncState;
            var context = httpListener.EndGetContext(result);

            // Send an HTTP response to the browser to notify the user to close the browser.
            var response = context.Response;
            var responseString = settings.LoopbackResponseHtml;
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            responseOutput.Write(buffer, 0, buffer.Length);
            responseOutput.Close();
            httpListener.Close();

            // Check for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                Debug.LogError($"UnityGoogleDrive: OAuth authorization error: {context.Request.QueryString.Get("error")}.");
                HandleProvideAccessTokenComplete(true);
                return;
            }
            if (context.Request.QueryString.Get("code") == null || context.Request.QueryString.Get("state") == null)
            {
                Debug.LogError($"UnityGoogleDrive: Malformed authorization response. {context.Request.QueryString}");
                HandleProvideAccessTokenComplete(true);
                return;
            }

            // Extract the authorization code.
            authorizationCode = context.Request.QueryString.Get("code");
            var incomingState = context.Request.QueryString.Get("state");

            // Compare the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incomingState != expectedState)
            {
                Debug.LogError($"UnityGoogleDrive: Received request with invalid state ({incomingState}).");
                HandleProvideAccessTokenComplete(true);
                return;
            }

            // Exchange the authorization code for tokens.
            authCodeExchanger.ExchangeAuthCode(authorizationCode, codeVerifier, redirectUri);
        }

        private int GetRandomUnusedPort ()
        {
            // Based on: http://stackoverflow.com/a/3978040.
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}
