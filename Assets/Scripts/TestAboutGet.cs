using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestAboutGet : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 940, 580);

    private GoogleDriveAbout.GetRequest request;
         
    private void Start ()
    {
        UpdateInfo();
    }

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Info");
    }

    private void InfoWindowGUI (int windowId)
    {
        if (request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
        }
        else
        {
            if (GUILayout.Button("Refresh"))
                UpdateInfo();
        }

        if (request.ResponseData != null)
        {
            GUILayout.Label(string.Format("User name: {0}\nUser email: {1}\nSpace used: {2:0}/{3:0} MB", 
                request.ResponseData.User.DisplayName,
                request.ResponseData.User.EmailAddress,
                request.ResponseData.StorageQuota.Usage * .000001f,
                request.ResponseData.StorageQuota.Limit * .000001f));
        }
    }

    private void UpdateInfo ()
    {
        request = GoogleDriveAbout.Get();
        request.Fields = new List<string> { "user", "storageQuota" };
        request.Send();
    }
}
