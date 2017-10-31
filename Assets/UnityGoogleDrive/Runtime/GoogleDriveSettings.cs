using UnityEngine;

public class GoogleDriveSettings : ScriptableObject
{
    public AuthCredentials AuthCredentials { get { return _authCredentials; } set { _authCredentials = value; } }

    [SerializeField] private AuthCredentials _authCredentials;

    public static GoogleDriveSettings LoadFromResources ()
    {
        var settings = Resources.Load<GoogleDriveSettings>("GoogleDriveSettings");

        if (!settings)
        {
            #if UNITY_EDITOR
            settings = CreateInstance<GoogleDriveSettings>();
            System.IO.Directory.CreateDirectory(Application.dataPath + "/UnityGoogleDrive/Resources");
            UnityEditor.AssetDatabase.CreateAsset(settings, "Assets/UnityGoogleDrive/Resources/GoogleDriveSettings.asset");
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.AssetDatabase.SaveAssets();
            Debug.Log("Google Drive settings file didn't exist and was created.");
            UnityEditor.Selection.activeObject = settings;
            #else
            Debug.LogError("Google Drive settings file not found.");
            #endif
        }

        return settings;
    }
}
