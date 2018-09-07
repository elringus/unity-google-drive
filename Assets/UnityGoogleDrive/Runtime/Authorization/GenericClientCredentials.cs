using System.Collections.Generic;
using UnityEngine;

namespace UnityGoogleDrive
{
    [System.Serializable]
    public class GenericClientCredentials : IClientCredentials
    {
        public string ClientId { get { return client_id; } }
        public string ProjectId { get { return project_id; } }
        public string AuthUri { get { return auth_uri; } }
        public string TokenUri { get { return token_uri; } }
        public string AuthProviderX509CertUrl { get { return auth_provider_x509_cert_url; } }
        public string ClientSecret { get { return client_secret; } }
        public List<string> RedirectUris { get { return redirect_uris; } }

        [SerializeField] private string client_id = null;
        [SerializeField] private string project_id = null;
        [SerializeField] private string auth_uri = null;
        [SerializeField] private string token_uri = null;
        [SerializeField] private string auth_provider_x509_cert_url = null;
        [SerializeField] private string client_secret = null;
        [SerializeField] private List<string> redirect_uris = null;

        public static GenericClientCredentials FromJson (string json)
        {
            return JsonUtility.FromJson<GenericClientCredentials>(json);
        }

        public void OverwriteFromJson (string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public string ToJson (bool prettyPrint = false)
        {
            return JsonUtility.ToJson(this, prettyPrint);
        }

        public bool ContainsSensitiveData ()
        {
            return !string.IsNullOrEmpty(ClientId + ProjectId + ClientSecret);
        }
    }
}
