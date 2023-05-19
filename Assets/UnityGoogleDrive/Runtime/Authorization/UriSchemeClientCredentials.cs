using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace UnityGoogleDrive
{
    [System.Serializable]
    public class UriSchemeClientCredentials : IClientCredentials
    {
        public string AuthUri => "https://accounts.google.com/o/oauth2/v2/auth";
        public string TokenUri => "https://accounts.google.com/o/oauth2/token";
        public string ClientId => clientId;
        public string ClientSecret => null; // Client secret is not required in custom URI scheme.
        public string ReversedClientId => reversedClientId;
        public string PlistVersion => plistVersion;
        public string BundleId => bundleId;

        [SerializeField] private string clientId;
        [SerializeField] private string reversedClientId;
        [SerializeField] private string plistVersion;
        [SerializeField] private string bundleId;

        public void OverwriteFromXml (string xmlString)
        {
            var xml = new XmlDocument();
            xml.XmlResolver = null;
            using (TextReader reader = new StringReader(xmlString))
                xml.Load(reader);
            var rootNode = xml.DocumentElement?.ChildNodes[0];
            var dict = ParsePlistDictionary(rootNode);

            clientId = dict["CLIENT_ID"];
            reversedClientId = dict["REVERSED_CLIENT_ID"];
            plistVersion = dict["PLIST_VERSION"];
            bundleId = dict["BUNDLE_ID"];
        }

        public bool ContainsSensitiveData ()
        {
            return !string.IsNullOrEmpty(ClientId + ReversedClientId + BundleId);
        }

        private static Dictionary<string, string> ParsePlistDictionary (XmlNode node)
        {
            var children = node.ChildNodes;
            if (children.Count % 2 != 0)
            {
                Debug.LogError("Dictionary elements must have an even number of child nodes");
                return null;
            }

            var dict = new Dictionary<string, string>();
            for (int i = 0; i < children.Count; i += 2)
            {
                var keyNode = children[i];
                var valueNode = children[i + 1];
                if (keyNode.Name != "key")
                {
                    Debug.LogError("Expected a key node.");
                    return null;
                }
                dict.Add(keyNode.InnerText, valueNode.InnerText);
            }

            return dict;
        }
    }
}
