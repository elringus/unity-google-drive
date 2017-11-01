using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GoogleDriveSettings))]
public class GoogleDriveSettingsEditor : Editor
{
    protected GoogleDriveSettings TargetSettings { get { return target as GoogleDriveSettings; } }

    private SerializedProperty authCredentials;

    [InitializeOnLoadMethod]
    private static GoogleDriveSettings GetOrCreateSettings ()
    {
        var settings = GoogleDriveSettings.LoadFromResources(true);
        if (!settings)
        {
            settings = CreateInstance<GoogleDriveSettings>();
            Directory.CreateDirectory(Application.dataPath + "/UnityGoogleDrive/Resources");
            AssetDatabase.CreateAsset(settings, "Assets/UnityGoogleDrive/Resources/GoogleDriveSettings.asset");
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            Debug.Log("Google Drive settings file didn't exist and was created.");
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
    }

    public override void OnInspectorGUI ()
    {
        if (TargetSettings.AuthCredentials.ContainsSensitiveData())
            EditorGUILayout.HelpBox("The asset contains sensitive data about your Google Drive API app. " +
                "Consider excluding it from the version control systems.", MessageType.Info);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(authCredentials, true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Google Drive API app"))
            Application.OpenURL(@"https://console.developers.google.com/start/api?id=drive");

        if (GUILayout.Button("Upload credentials JSON..."))
            UploadCredentialsJson(EditorUtility.OpenFilePanel("Select Google Drive credentials JSON file", "", "json"));

        serializedObject.ApplyModifiedProperties();
    }

    private void UploadCredentialsJson (string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        if (!File.Exists(path))
        {
            Debug.LogError("Specified path to Google Drive credentials JSON file is not valid.");
            return;
        }

        const string START_MARKER = "{\"installed\":";
        var jsonString = File.ReadAllText(path);
        if (!jsonString.StartsWith(START_MARKER))
        {
            Debug.LogError("Specified file is not valid. Make sure to setup Drive API to be used with installed platforms.");
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
