using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityGoogleDrive
{
    [CustomEditor(typeof(GoogleDriveSettings))]
    public class GoogleDriveSettingsEditor : Editor
    {
        protected GoogleDriveSettings TargetSettings { get { return target as GoogleDriveSettings; } }

        private static readonly string[] ACCESS_SCOPE_OPTIONS = new string[] {
            GoogleDriveSettings.FULL_ACCESS_SCOPE,
            GoogleDriveSettings.READONLY_ACCESS_SCOPE
        };

        private SerializedProperty authCredentials;
        private SerializedProperty accessScope;
        private SerializedProperty loopbackResponseHtml;
        private SerializedProperty accessTokenPrefsKey;
        private SerializedProperty refreshTokenPrefsKey;

        private GUIContent authCredentialsContent = new GUIContent("Authorization Credentials", "Google Drive API application credentials used to authorize requests.");
        private GUIContent accessScopeContent = new GUIContent("Access Scope", "Scope of access to the user's Google Drive the app will request.");
        private GUIContent[] accessScopeOptionsContent = new GUIContent[] { new GUIContent("Full"), new GUIContent("Readonly") };
        private GUIContent loopbackResponseHtmlContent = new GUIContent("Loopback Response HTML", "HTML page shown to the user when loopback response is received.");
        private GUIContent accessTokenPrefsKeyContent = new GUIContent("Acces Token Key", "PlayerPrefs key used to store access token.");
        private GUIContent refreshTokenPrefsKeyContent = new GUIContent("Refresh Token Key", "PlayerPrefs key used to store refresh token.");
        private GUIContent deleteCachedTokensContent = new GUIContent("Delete cached tokens", "Removes cached access and refresh tokens forcing user to login on the next request.");

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
                Debug.Log(string.Format("UnityGoogleDrive: Settings file didn't exist and was created at: {0}.\n" +
                    "You're free to move it, just make sure it stays in the root of a 'Resources' folder.", path));
            }
            return settings;
        }

        [MenuItem("Edit/Project Settings/Google Drive Settings")]
        private static void SelectSettings ()
        {
            var settings = GetOrCreateSettings();
            Selection.activeObject = settings;
        }

        private void OnEnable ()
        {
            authCredentials = serializedObject.FindProperty("authCredentials");
            accessScope = serializedObject.FindProperty("accessScope");
            loopbackResponseHtml = serializedObject.FindProperty("loopbackResponseHtml");
            accessTokenPrefsKey = serializedObject.FindProperty("accessTokenPrefsKey");
            refreshTokenPrefsKey = serializedObject.FindProperty("refreshTokenPrefsKey");
        }

        public override void OnInspectorGUI ()
        {
            if (TargetSettings.AuthCredentials.ContainsSensitiveData())
                EditorGUILayout.HelpBox("The asset contains sensitive data about your Google Drive API app. " +
                    "Consider excluding it from the version control systems.", MessageType.Info);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(authCredentials, authCredentialsContent, true);
            AccessScopeGUI();
            EditorGUILayout.PropertyField(loopbackResponseHtml, loopbackResponseHtmlContent);
            EditorGUILayout.PropertyField(accessTokenPrefsKey, accessTokenPrefsKeyContent);
            EditorGUILayout.PropertyField(refreshTokenPrefsKey, refreshTokenPrefsKeyContent);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Google Drive API app"))
                Application.OpenURL(@"https://console.developers.google.com/start/api?id=drive");

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(TargetSettings.AuthCredentials.ProjectId)))
                if (GUILayout.Button("Manage Google Drive API app"))
                    Application.OpenURL(string.Format(@"https://console.developers.google.com/apis/credentials?project={0}",
                        TargetSettings.AuthCredentials.ProjectId));

            if (GUILayout.Button("Parse credentials JSON file..."))
                ParseCredentialsJson(EditorUtility.OpenFilePanel("Select Drive API app credentials JSON file", "", "json"));

            EditorGUILayout.Space();

            using (new EditorGUI.DisabledScope(!TargetSettings.IsAnyAuthTokenCached()))
                if (GUILayout.Button(deleteCachedTokensContent))
                    TargetSettings.DeleteCachedAuthTokens();

            serializedObject.ApplyModifiedProperties();
        }

        private void AccessScopeGUI ()
        {
            var selectedIndex = Array.IndexOf(ACCESS_SCOPE_OPTIONS, accessScope.stringValue);
            selectedIndex = EditorGUILayout.Popup(accessScopeContent, selectedIndex, accessScopeOptionsContent);
            accessScope.stringValue = ACCESS_SCOPE_OPTIONS[selectedIndex];
        }

        private void ParseCredentialsJson (string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            if (!File.Exists(path))
            {
                Debug.LogError("UnityGoogleDrive: Specified path to credentials JSON file is not valid.");
                return;
            }

            const string START_MARKER = "{\"web\":";
            var jsonString = File.ReadAllText(path);
            if (!jsonString.StartsWith(START_MARKER))
            {
                Debug.LogError("UnityGoogleDrive: Specified file is not valid. Make sure to setup Drive API to be used with the web platform.");
                return;
            }

            // Extracting auth json object from the initial json string.
            var authJson = jsonString.Substring(START_MARKER.Length, jsonString.Length - START_MARKER.Length - 1);
            TargetSettings.AuthCredentials.OverwriteFromJson(authJson);
            serializedObject.Update();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }
}
