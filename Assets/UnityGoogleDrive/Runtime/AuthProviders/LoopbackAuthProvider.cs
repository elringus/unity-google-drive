using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Provides auth and refresh tokens using local loopback method to retrieve authorization code.
/// Implementation based on: https://github.com/googlesamples/oauth-apps-for-windows.
/// </summary>
public class LoopbackAuthProvider : IAuthProvider
{
    public event Action<IAuthProvider> OnDone;

    public bool IsDone { get; private set; }
    public bool IsError { get; private set; }
    public string AccessToken { get; private set; } 
    public string RefreshToken { get; private set; }

    private const string LOOPBACK_URI = @"http://localhost";

    private AuthCodeExchanger authCodeExchanger;
    private GoogleDriveSettings settings;
    private string expectedState;
    private string codeVerifier;
    private string redirectUri;
    private string authorizationCode;

    public void ProvideAuth (GoogleDriveSettings googleDriveSettings)
    {
        settings = googleDriveSettings;

        // Generate state and PKCE values.
        expectedState = CryptoUtils.RandomDataBase64Uri(32);
        codeVerifier = CryptoUtils.RandomDataBase64Uri(32);
        var codeVerifierHash = CryptoUtils.Sha256(codeVerifier);
        var codeChallenge = CryptoUtils.Base64UriEncodeNoPadding(codeVerifierHash);

        // Creates a redirect URI using an available port on the loopback address.
        redirectUri = string.Format("{0}:{1}", LOOPBACK_URI, GetRandomUnusedPort());

        // Listen for requests on the redirect URI.
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add(redirectUri + '/');
        httpListener.Start();

        // Create the OAuth 2.0 authorization request.
        // https://developers.google.com/identity/protocols/OAuth2WebServer#creatingclient
        var authRequest = string.Format("{0}?response_type=code&scope={1}%20profile&redirect_uri={2}&client_id={3}&state={4}&code_challenge={5}&code_challenge_method={6}" +
                "&access_type=offline" + // Forces to return a refresh token at the auth code exchange phase.
                "&approval_prompt=force", // Forces to show consent screen for each auth request. Needed to return refresh tokens on consequent auth runs.
            settings.AuthCredentials.AuthUri,
            GoogleDriveSettings.FULL_ACCESS_SCOPE,
            Uri.EscapeDataString(redirectUri),
            settings.AuthCredentials.ClientId,
            expectedState,
            codeChallenge,
            GoogleDriveSettings.CODE_CHALLENGE_METHOD);

        // Open request in the browser.
        Application.OpenURL(authRequest);

        // Wait for the authorization response.
        var context = httpListener.GetContext();

        // Send an HTTP response to the browser to notify the user to close the browser.
        var response = context.Response;
        var responseString = settings.LoopbackResponseHtml;
        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var responseOutput = response.OutputStream;
        responseOutput.Write(buffer, 0, buffer.Length);
        responseOutput.Close();
        httpListener.Stop();

        // Check for errors.
        if (context.Request.QueryString.Get("error") != null)
        {
            Debug.LogError(string.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
            HandleProvideAuthComplete(true);
            return;
        }
        if (context.Request.QueryString.Get("code") == null || context.Request.QueryString.Get("state") == null)
        {
            Debug.LogError("Malformed authorization response. " + context.Request.QueryString);
            HandleProvideAuthComplete(true);
            return;
        }

        // Extract the authorization code.
        authorizationCode = context.Request.QueryString.Get("code");
        var incomingState = context.Request.QueryString.Get("state");

        // Compare the receieved state to the expected value, to ensure that
        // this app made the request which resulted in authorization.
        if (incomingState != expectedState)
        {
            Debug.LogError(string.Format("Received request with invalid state ({0}).", incomingState));
            HandleProvideAuthComplete(true);
            return;
        }

        // Exchange the authorization code for tokens.
        authCodeExchanger = new AuthCodeExchanger();
        authCodeExchanger.OnDone += HandleAuthCodeExchanged;
        authCodeExchanger.ExchangeAuthCode(settings, authorizationCode, codeVerifier, redirectUri);
    }

    private void HandleProvideAuthComplete (bool error = false)
    {
        IsError = error;
        IsDone = true;
        if (OnDone != null)
            OnDone.Invoke(this);
    }

    private void HandleAuthCodeExchanged(AuthCodeExchanger exchanger)
    {
        authCodeExchanger.OnDone -= HandleAuthCodeExchanged;

        if (authCodeExchanger.IsError)
        {
            Debug.LogError("Failed to exchange authorization code.");
            HandleProvideAuthComplete(true);
            return;
        }

        AccessToken = authCodeExchanger.AccesToken;
        RefreshToken = authCodeExchanger.RefreshToken;
        HandleProvideAuthComplete();
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
