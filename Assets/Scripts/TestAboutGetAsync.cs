using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestAboutGetAsync : AdaptiveWindowGUI
{
    private GoogleDriveAbout.GetRequest request;
    private UnityGoogleDrive.Data.About aboutData;
         
    private async void Start () => aboutData = await UpdateInfo();

    protected override void OnWindowGUI (int windowId)
    {
        if (aboutData != null)
        {
            GUILayout.Label(string.Format("User name: {0}\nUser email: {1}\nSpace used: {2:0}/{3:0} MB",
                aboutData.User.DisplayName,
                aboutData.User.EmailAddress,
                aboutData.StorageQuota.Usage * .000001f,
                aboutData.StorageQuota.Limit * .000001f));
        }
        else if (request != null && request.IsRunning) GUILayout.Label($"Loading: {request.Progress:P2}");
    }

    private async System.Threading.Tasks.Task<UnityGoogleDrive.Data.About> UpdateInfo ()
    {
        AuthController.CancelAuth();

        request = GoogleDriveAbout.Get();
        request.Fields = new List<string> { "user", "storageQuota" };
        return await request.Send();
    }
}
