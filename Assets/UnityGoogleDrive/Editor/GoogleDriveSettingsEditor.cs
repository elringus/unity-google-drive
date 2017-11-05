using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GoogleDriveSettings))]
public class GoogleDriveSettingsEditor : Editor
{
    protected GoogleDriveSettings TargetSettings { get { return target as GoogleDriveSettings; } }

    private SerializedProperty authCredentials;
    private SerializedProperty sharedRefreshToken;
    private SerializedProperty loopbackResponseHtml;

    private GUIContent authCredentialsContent = new GUIContent("Authorization Credentials", "Google Drive API application credentials used to authorize requests.");
    private GUIContent sharedRefreshTokenContent = new GUIContent("Shared Refresh Token", "Used to provide shared access to the authorized user's drive.");
    private GUIContent loopbackResponseHtmlContent = new GUIContent("Loopback Response HTML", "HTML page shown to the user when loopback response is received.");

    [InitializeOnLoadMethod]
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
            Debug.Log(string.Format("Google Drive settings file didn't exist and was created at: {0}\n" +
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
        sharedRefreshToken = serializedObject.FindProperty("sharedRefreshToken");
        loopbackResponseHtml = serializedObject.FindProperty("loopbackResponseHtml");
    }

    public override void OnInspectorGUI ()
    {
        if (TargetSettings.AuthCredentials.ContainsSensitiveData())
            EditorGUILayout.HelpBox("The asset contains sensitive data about your Google Drive API app. " +
                "Consider excluding it from the version control systems.", MessageType.Info);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(authCredentials, authCredentialsContent, true);
        EditorGUILayout.PropertyField(sharedRefreshToken, sharedRefreshTokenContent);
        EditorGUILayout.PropertyField(loopbackResponseHtml, loopbackResponseHtmlContent);

        EditorGUILayout.Space();

        if (GUILayout.Button("Create Google Drive API app"))
            Application.OpenURL(@"https://console.developers.google.com/start/api?id=drive");

        if (GUILayout.Button("Parse credentials JSON file..."))
            ParseCredentialsJson(EditorUtility.OpenFilePanel("Select Drive API app credentials JSON file", "", "json"));

        using (new EditorGUI.DisabledScope(!TargetSettings.AuthCredentials.ContainsSensitiveData()))
            if (GUILayout.Button("Retrieve shared refresh token"))
                Application.OpenURL(@"https://console.developers.google.com");

        serializedObject.ApplyModifiedProperties();
    }

    private void ParseCredentialsJson (string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        if (!File.Exists(path))
        {
            Debug.LogError("Specified path to Google Drive credentials JSON file is not valid.");
            return;
        }

        const string START_MARKER = "{\"web\":";
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
