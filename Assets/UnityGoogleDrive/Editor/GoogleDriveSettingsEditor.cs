using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GoogleDriveSettings)), InitializeOnLoad]
public class GoogleDriveSettingsEditor : Editor
{
    protected GoogleDriveSettings TargetSettings { get { return target as GoogleDriveSettings; } }

    private SerializedProperty authCredentials;

    static GoogleDriveSettingsEditor ()
    {
        GoogleDriveSettings.LoadFromResources();
    }

    private void OnEnable ()
    {
        authCredentials = serializedObject.FindProperty("_authCredentials");
    }

    public override void OnInspectorGUI ()
    {
        EditorGUILayout.PropertyField(authCredentials, true);

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
        TargetSettings.AuthCredentials = AuthCredentials.FromJson(authJson);
    }
}
