using System;
using UnityEngine;

/// <summary>
/// Handles authorization procedures and provides tokens to access Google APIs.
/// Implementation based on Google OAuth 2.0 protocol: https://developers.google.com/identity/protocols/OAuth2.
/// </summary>
public class AuthState
{
    public event Action OnAccessTokenRefreshed;

    public string AccessToken { get { return PlayerPrefs.GetString(ACCESS_TOKEN_KEY); } private set { PlayerPrefs.SetString(ACCESS_TOKEN_KEY, value); } }
    public string RefreshToken { get { return PlayerPrefs.GetString(REFRESH_TOKEN_KEY); } private set { PlayerPrefs.SetString(REFRESH_TOKEN_KEY, value); } }
    public bool IsRefreshingAccessToken { get; private set; }

    private const string ACCESS_TOKEN_KEY = "GoogleDriveAccessToken";
    private const string REFRESH_TOKEN_KEY = "GoogleDriveRefreshToken";

    private AuthCredentials credentials;
    private IAuthProvider authProvider;
    private AccessTokenRefresher accessTokenRefresher;

    public AuthState (AuthCredentials authCredentials, string sharedRefreshToken = null)
    {
        credentials = authCredentials;
        if (!string.IsNullOrEmpty(sharedRefreshToken))
            RefreshToken = sharedRefreshToken;
    }

    public void RefreshAccessToken ()
    {
        if (IsRefreshingAccessToken) return;
        IsRefreshingAccessToken = true;

        // Refresh token isn't available; executing full auth procedure.
        if (string.IsNullOrEmpty(RefreshToken)) ExecuteAuthProvider();
        // Using refresh token to issue a new access token.
        else ExecuteAccessTokenRefresher();
    }

    private void HandleAccessTokenRefreshed ()
    {
        IsRefreshingAccessToken = false;
        if (OnAccessTokenRefreshed != null)
            OnAccessTokenRefreshed.Invoke();
    }

    private void ExecuteAuthProvider ()
    {
        #if UNITY_WEBGL // WebGL doesn't support loopback method; using redirection scheme.
        authProvider = new RedirectAuthProvider();
        #else
        authProvider = new LoopbackAuthProvider();
        #endif

        authProvider.OnDone += HandleAuthProviderDone;
        authProvider.ProvideAuth(credentials);
    }

    private void HandleAuthProviderDone (IAuthProvider provider)
    {
        authProvider.OnDone -= HandleAuthProviderDone;

        if (provider.IsError)
        {
            Debug.LogError("Failed to execute Google Drive auth procedure. Check application settings and credentials.");
            // TODO: Handle auth procedure fail for running requests.
        }
        else
        {
            AccessToken = provider.AccessToken;
            RefreshToken = provider.RefreshToken;
            HandleAccessTokenRefreshed();
        }
    }

    private void ExecuteAccessTokenRefresher ()
    {
        accessTokenRefresher = new AccessTokenRefresher();
        accessTokenRefresher.OnDone += HandleAccessTokenRefresherDone;
        accessTokenRefresher.RefreshAccessToken(credentials, RefreshToken);
    }

    private void HandleAccessTokenRefresherDone (AccessTokenRefresher refresher)
    {
        accessTokenRefresher.OnDone -= HandleAccessTokenRefresherDone;

        if (refresher.IsError)
        {
            Debug.LogWarning("Failed to refresh Google Drive access token; executing full auth procedure.");
            ExecuteAuthProvider();
        }
        else
        {
            AccessToken = refresher.AccesToken;
            HandleAccessTokenRefreshed();
        }
    }
}
