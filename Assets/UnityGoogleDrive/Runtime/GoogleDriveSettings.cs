using UnityEngine;

public class GoogleDriveSettings : ScriptableObject
{
    public static AuthCredentials Credentials { get { return GetCredentials(); } set { SetCredentials(value); } }

    private const string CREDENTIALS_KEY = "GoogleDriveCredentials";

    #pragma warning disable 0169
    [SerializeField, HideInInspector] private AuthCredentials cachedCredentials;
    #pragma warning restore

    private static AuthCredentials GetCredentials ()
    {
        #if UNITY_EDITOR
        if (PlayerPrefs.HasKey(CREDENTIALS_KEY))
            return AuthCredentials.FromJson(PlayerPrefs.GetString(CREDENTIALS_KEY));
        else return new AuthCredentials();
        #else
        return cachedCredentials;
        #endif
    }

    private static void SetCredentials (AuthCredentials credentials)
    {
        #if UNITY_EDITOR
        PlayerPrefs.SetString(CREDENTIALS_KEY, credentials.ToJson());
        #endif
    }
}
