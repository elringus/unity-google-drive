using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestAboutGetAsync : AdaptiveWindowGUI
{
    #if NET_4_6 || NET_STANDARD_2_0
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
        else if (request != null && request.IsRunning) GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
    }

    private async System.Threading.Tasks.Task<UnityGoogleDrive.Data.About> UpdateInfo ()
    {
        AuthController.CancelAuth();

        request = GoogleDriveAbout.Get();
        request.Fields = new List<string> { "user", "storageQuota" };
        return await request.Send();
    }

    #else
    protected override void OnWindowGUI (int windowId) { GUILayout.Label(".NET 4.x scripting backend is not enabled."); }
    #endif
}
