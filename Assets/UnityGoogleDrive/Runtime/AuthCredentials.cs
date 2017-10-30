using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AuthCredentials
{
    public string ClientId { get { return client_id; } }
    public string ProjectId { get { return project_id; } }
    public string AuthUri { get { return auth_uri; } }
    public string TokenUri { get { return token_uri; } }
    public string AuthProviderX509CertUrl { get { return auth_provider_x509_cert_url; } }
    public string ClientSecret { get { return client_secret; } }
    public List<string> RedirectUris { get { return redirect_uris; } }

    #pragma warning disable 0649
    [SerializeField] private string client_id;
    [SerializeField] private string project_id;
    [SerializeField] private string auth_uri;
    [SerializeField] private string token_uri;
    [SerializeField] private string auth_provider_x509_cert_url;
    [SerializeField] private string client_secret;
    [SerializeField] private List<string> redirect_uris;
    #pragma warning restore

    public static AuthCredentials FromJson (string json)
    {
        return JsonUtility.FromJson<AuthCredentials>(json);
    }

    public string ToJson (bool prettyPrint = false)
    {
        return JsonUtility.ToJson(this, prettyPrint);
    }
}
