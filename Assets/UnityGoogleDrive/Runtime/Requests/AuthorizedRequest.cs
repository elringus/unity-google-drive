using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// The derived requests will handle OAuth 2.0 authorization via Google API.
/// </summary>
public abstract class AuthorizedRequest<T> : GoogleDriveRequest<T> where T : AuthorizedRequest<T>
{
    // state, challenge, auth code, access token, refresh token to player prefs
    // Merge with GoogleDriveRequest <-------------------------------------

    private const string CODE_CHALLENGE_METHOD = "S256";

    public AuthorizedRequest (string url, string method)
        : base(url, method)
    {

    }

    protected override void OnBeforeSend (UnityWebRequest webRequest)
    {
        // Generate state and PKCE values.
        var authState = RandomDataBase64url(32);
        var codeVerifier = RandomDataBase64url(32);
        var codeChallenge = Base64UrlEncodeNoPadding(Sha256(codeVerifier));

        // Creates a redirect URI using an available port on the loopback address.
        var redirectUri = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());

        // Creates an HttpListener to listen for requests on that redirect URI.
        var http = new HttpListener();
        http.Prefixes.Add(redirectUri);
        http.Start();

        // Create the OAuth 2.0 authorization request.
        var authRequest = string.Format("{0}?response_type=code&scope=openid%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
            Settings.AuthCredentials.AuthUri,
            System.Uri.EscapeDataString(Settings.AuthCredentials.RedirectUris[1]),
            Settings.AuthCredentials.ClientId,
            authState,
            codeChallenge,
            CODE_CHALLENGE_METHOD);

        Application.OpenURL(authRequest);

        //webRequest.downloadHandler = new DownloadHandlerBuffer();
        //webRequest.SetRequestHeader("Content-Type", "text/plain");
    }

    protected override void OnBeforeDone (UnityWebRequest webRequest)
    {
        
    }

    private static int GetRandomUnusedPort ()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    /// <summary>
    /// Returns URI-safe data with a given input length.
    /// </summary>
    /// <param name="length">Input length (nb. output will be longer).</param>
    private static string RandomDataBase64url (uint length)
    {
        var cryptoProvider = new RNGCryptoServiceProvider();
        var bytes = new byte[length];
        cryptoProvider.GetBytes(bytes);
        return Base64UrlEncodeNoPadding(bytes);
    }

    /// <summary>
    /// Returns the SHA256 hash of the input string.
    /// </summary>
    private static byte[] Sha256 (string inputString)
    {
        var bytes = Encoding.ASCII.GetBytes(inputString);
        var sha256 = new SHA256Managed();
        return sha256.ComputeHash(bytes);
    }

    /// <summary>
    /// Base64url no-padding encodes the given input buffer.
    /// </summary>
    private static string Base64UrlEncodeNoPadding (byte[] buffer)
    {
        var base64 = Convert.ToBase64String(buffer);

        // Converts base64 to base64url.
        base64 = base64.Replace("+", "-");
        base64 = base64.Replace("/", "_");
        // Strips padding.
        base64 = base64.Replace("=", "");

        return base64;
    }
}
