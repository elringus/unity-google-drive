using UnityEngine;

public class GoogleDriveSettings : ScriptableObject
{
    public AuthCredentials AuthCredentials { get { return authCredentials; } }

    [SerializeField] private AuthCredentials authCredentials = null;

    public static GoogleDriveSettings LoadFromResources (bool silent = false)
    {
        var settings = Resources.Load<GoogleDriveSettings>("GoogleDriveSettings");

        if (!settings && !silent)
        {
            Debug.LogError("Google Drive settings file not found. " +
                "Use Edit > Project Settings > Google Drive Settings to create a new one.");
        }

        return settings;
    }

}
