using System.IO;
using UnityEditor;
using UnityEngine;

public class SettingsWindow : EditorWindow
{
    [MenuItem("Edit/Project Settings/Google Drive")]
    private static void OpenSettingsWindow ()
    {
        var window = GetWindow<SettingsWindow>();
        window.Show();
    }

    private void OnGUI ()
    {
        EditorGUILayout.LabelField("Google Drive Settings", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Sensitive data is stored in editor's PlayerPrefs and won't be exposed in project assets.", MessageType.Info);

        EditorGUILayout.Space();

        EditorGUILayout.TextField("Client ID", GoogleDriveSettings.Credentials.ClientId);
        EditorGUILayout.TextField("Project ID", GoogleDriveSettings.Credentials.ProjectId);
        EditorGUILayout.TextField("Auth URI", GoogleDriveSettings.Credentials.AuthUri);
        EditorGUILayout.TextField("Token URI", GoogleDriveSettings.Credentials.TokenUri);
        EditorGUILayout.TextField("x509 URI", GoogleDriveSettings.Credentials.AuthProviderX509CertUrl);
        EditorGUILayout.TextField("Client Secret", GoogleDriveSettings.Credentials.ClientSecret);
        if (GoogleDriveSettings.Credentials.RedirectUris != null)
            for (int i = 0; i < GoogleDriveSettings.Credentials.RedirectUris.Count; i++)
                EditorGUILayout.TextField("Redirect URI #" + (i + 1), GoogleDriveSettings.Credentials.RedirectUris[i]);

        EditorGUILayout.Space();

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(EditorGUIUtility.labelWidth + 4);
            if (GUILayout.Button("Create Google Drive API project", EditorStyles.miniButton))
                Application.OpenURL(@"https://console.developers.google.com/start/api?id=drive");
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(EditorGUIUtility.labelWidth + 4);
            if (GUILayout.Button("Upload credentials JSON...", EditorStyles.miniButton))
                UploadCredentialsJson(EditorUtility.OpenFilePanel("Select Google Drive credentials JSON file", "", "json"));
        }
    }

    private void UploadCredentialsJson (string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        if (!File.Exists(path))
        {
            Debug.LogError("Specified path to Google Drive credentials JSON file is not valid.");
            return;
        }

        var jsonString = File.ReadAllText(path);
        var startMarker = "{\"installed\":";
        if (!jsonString.StartsWith(startMarker))
        {
            Debug.LogError("Specified file is not valid. Make sure to setup Drive API to be used with installed platforms.");
            return;
        }
        // Extracting auth json object from the initial json string.
        var authJson = jsonString.Substring(startMarker.Length, jsonString.Length - startMarker.Length - 1);
        GoogleDriveSettings.Credentials = AuthCredentials.FromJson(authJson);
    }
}
