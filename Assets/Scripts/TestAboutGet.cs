using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestAboutGet : AdaptiveWindowGUI
{
    private GoogleDriveAbout.GetRequest request;
    private GoogleDriveSettings settings;

    protected override void Awake ()
    {
        base.Awake();
        settings = GoogleDriveSettings.LoadFromResources();
    }

    private void Start ()
    {
        UpdateInfo();
    }

    protected override void OnWindowGUI (int windowId)
    {
        if (request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
        }
        else
        {
            if (GUILayout.Button("Refresh"))
                UpdateInfo();
        }

        if (settings.IsAnyAuthTokenCached() && GUILayout.Button("Delete Cached Tokens"))
            settings.DeleteCachedAuthTokens();

        if (request.ResponseData != null)
        {
            GUILayout.Label(string.Format("User name: {0}\nUser email: {1}\nSpace used: {2:0}/{3:0} MB", 
                request.ResponseData.User.DisplayName,
                request.ResponseData.User.EmailAddress,
                request.ResponseData.StorageQuota.Usage * .000001f,
                request.ResponseData.StorageQuota.Limit * .000001f));
        }

        if (request.IsError)
            GUILayout.Label(string.Format("Request failed: {0}", request.Error));
    }

    private void UpdateInfo ()
    {
        AuthController.CancelAuth();

        request = GoogleDriveAbout.Get();
        request.Fields = new List<string> { "user", "storageQuota" };
        request.Send();
    }
}
