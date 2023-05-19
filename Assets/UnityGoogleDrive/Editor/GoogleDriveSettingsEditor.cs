using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityGoogleDrive
{
    [CustomEditor(typeof(GoogleDriveSettings))]
    public class GoogleDriveSettingsEditor : Editor
    {
        protected GoogleDriveSettings TargetSettings => target as GoogleDriveSettings;

        private SerializedProperty genericClientCredentials;
        private SerializedProperty uriSchemeClientCredentials;
        private SerializedProperty accessScopes;
        private SerializedProperty loopbackUri;
        private SerializedProperty loopbackResponseHtml;
        private SerializedProperty accessTokenPrefsKey;
        private SerializedProperty refreshTokenPrefsKey;

        private readonly static GUIContent genericClientCredentialsContent = new GUIContent("Generic Credentials", "Google Drive API application credentials used to authorize requests via loopback and redirect schemes.");
        private readonly static GUIContent uriSchemeClientCredentialsContent = new GUIContent("URI Scheme Credentials", "Google Drive API application credentials used to authorize requests via custom URI scheme.");
        private readonly static GUIContent accessScopesContent = new GUIContent("Access Scopes", "Scopes of access to the user's Google Drive the app will request.");
        private readonly static GUIContent loopbackUriContent = new GUIContent("Loopback URI", "A web address for the loopback authentication requests. Defult is 'localhost'.");
        private readonly static GUIContent loopbackResponseHtmlContent = new GUIContent("Loopback Response HTML", "HTML page shown to the user when loopback response is received.");
        private readonly static GUIContent accessTokenPrefsKeyContent = new GUIContent("Access Token Key", "PlayerPrefs key used to store access token.");
        private readonly static GUIContent refreshTokenPrefsKeyContent = new GUIContent("Refresh Token Key", "PlayerPrefs key used to store refresh token.");
        private readonly static GUIContent deleteCachedTokensContent = new GUIContent("Delete cached tokens", "Removes cached access and refresh tokens forcing user to login on the next request.");

        private static GoogleDriveSettings GetOrCreateSettings ()
        {
            var settings = GoogleDriveSettings.LoadFromResources(true);
            if (!settings)
            {
                settings = CreateInstance<GoogleDriveSettings>();
                Directory.CreateDirectory(Application.dataPath + "/UnityGoogleDrive/Resources");
                const string path = "Assets/UnityGoogleDrive/Resources/GoogleDriveSettings.asset";
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
                Debug.Log($"UnityGoogleDrive: Settings file didn't exist and was created at: {path}.\n" +
                          "You're free to move it, just make sure it stays in the root of a 'Resources' folder.");
            }
            return settings;
        }

        [SettingsProvider]
        internal static SettingsProvider CreateProjectSettingsProvider ()
        {
            var assetPath = AssetDatabase.GetAssetPath(GetOrCreateSettings());
            var keywords = SettingsProvider.GetSearchKeywordsFromPath(assetPath);
            return AssetSettingsProvider.CreateProviderFromAssetPath("Project/Google Drive", assetPath, keywords);
        }

        private void OnEnable ()
        {
            if (!TargetSettings) return;
            genericClientCredentials = serializedObject.FindProperty("genericClientCredentials");
            uriSchemeClientCredentials = serializedObject.FindProperty("uriSchemeClientCredentials");
            accessScopes = serializedObject.FindProperty("accessScopes");
            loopbackUri = serializedObject.FindProperty("loopbackUri");
            loopbackResponseHtml = serializedObject.FindProperty("loopbackResponseHtml");
            accessTokenPrefsKey = serializedObject.FindProperty("accessTokenPrefsKey");
            refreshTokenPrefsKey = serializedObject.FindProperty("refreshTokenPrefsKey");
        }

        public override void OnInspectorGUI ()
        {
            if (TargetSettings.GenericClientCredentials.ContainsSensitiveData() || TargetSettings.UriSchemeClientCredentials.ContainsSensitiveData())
                EditorGUILayout.HelpBox("The asset contains sensitive data about your Google Drive API app. " +
                                        "Consider excluding it from the version control systems.", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(genericClientCredentials, genericClientCredentialsContent, true);
            EditorGUILayout.PropertyField(uriSchemeClientCredentials, uriSchemeClientCredentialsContent, true);
            EditorGUILayout.PropertyField(accessScopes, accessScopesContent, true);
            EditorGUILayout.PropertyField(loopbackUri, loopbackUriContent);
            EditorGUILayout.PropertyField(loopbackResponseHtml, loopbackResponseHtmlContent);
            EditorGUILayout.PropertyField(accessTokenPrefsKey, accessTokenPrefsKeyContent);
            EditorGUILayout.PropertyField(refreshTokenPrefsKey, refreshTokenPrefsKeyContent);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Google Drive API app"))
                Application.OpenURL(@"https://console.developers.google.com/start/api?id=drive");

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(TargetSettings.GenericClientCredentials.ProjectId)))
                if (GUILayout.Button("Manage Google Drive API app"))
                    Application.OpenURL(string.Format(@"https://console.developers.google.com/apis/credentials?project={0}",
                        TargetSettings.GenericClientCredentials.ProjectId));

            if (GUILayout.Button("Parse generic credentials JSON file..."))
                ParseGenericCredentialsJson(EditorUtility.OpenFilePanel("Select Drive API app credentials JSON file", "", "json"));

            if (GUILayout.Button("Parse URI scheme credentials PLIST file..."))
                ParseUriSchemeCredentialsPlist(EditorUtility.OpenFilePanel("Select Drive API app credentials PLIST file", "", "plist"));

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(!TargetSettings.IsAnyAuthTokenCached()))
                if (GUILayout.Button(deleteCachedTokensContent))
                    TargetSettings.DeleteCachedAuthTokens();

            serializedObject.ApplyModifiedProperties();
        }

        private void ParseGenericCredentialsJson (string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (!File.Exists(path))
            {
                Debug.LogError("UnityGoogleDrive: Specified path to credentials JSON file is not valid.");
                return;
            }

            const string START_MARKER = "{\"client_id\":";
            var jsonString = File.ReadAllText(path);
            if (!jsonString.Contains(START_MARKER))
            {
                Debug.LogError("UnityGoogleDrive: Specified credentials file is not valid.");
                return;
            }

            // Extracting auth json object from the initial json string.
            var startIndex = jsonString.IndexOf(START_MARKER, StringComparison.Ordinal);
            var authJson = jsonString.Substring(startIndex).Replace("}}", "}");
            TargetSettings.GenericClientCredentials.OverwriteFromJson(authJson);
            serializedObject.Update();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }

        private void ParseUriSchemeCredentialsPlist (string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (!File.Exists(path))
            {
                Debug.LogError("UnityGoogleDrive: Specified path to credentials JSON file is not valid.");
                return;
            }

            var plistXml = File.ReadAllText(path);
            TargetSettings.UriSchemeClientCredentials.OverwriteFromXml(plistXml);
            serializedObject.Update();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}
