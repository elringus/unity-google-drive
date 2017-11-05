using UnityEngine;

public class GoogleDriveSettings : ScriptableObject
{
    public const string FULL_ACCESS_SCOPE = "https://www.googleapis.com/auth/drive";
    public const string REQUEST_CONTENT_TYPE = "application/x-www-form-urlencoded";
    public const string CODE_CHALLENGE_METHOD = "S256";
    public const int UNAUTHORIZED_RESPONSE_CODE = 401;

    public AuthCredentials AuthCredentials { get { return authCredentials; } }
    public string SharedRefreshToken { get { return sharedRefreshToken; } }
    public string LoopbackResponseHtml { get { return loopbackResponseHtml; } }

    [SerializeField] private AuthCredentials authCredentials = null;
    [SerializeField] private string sharedRefreshToken = null;
    [SerializeField] private string loopbackResponseHtml = "<html><h1>Please return to the app.</h1></html>";

    public static GoogleDriveSettings LoadFromResources (bool silent = false)
    {
        var settings = Resources.Load<GoogleDriveSettings>("GoogleDriveSettings");

        if (!settings && !silent)
        {
            Debug.LogError("Google Drive settings file not found. " +
                "Use 'Edit > Project Settings > Google Drive Settings' to create a new one.");
        }

        return settings;
    }
}
