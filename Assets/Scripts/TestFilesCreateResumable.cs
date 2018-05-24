using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesCreateResumable : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 940, 580);
    public string UploadFilePath;

    private GoogleDriveFiles.ResumableCreateRequest request;
    private string resumableSessionUri;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Upload File (Resumable)");
    }

    private void InfoWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
            if (GUILayout.Button("Abort Upload")) request.Abort();
        }
        else
        {
            UploadFilePath = GUILayout.TextField(UploadFilePath);
            if (GUILayout.Button("Upload")) Upload();
        }

        if (!string.IsNullOrEmpty(resumableSessionUri))
        {
            GUILayout.Label(resumableSessionUri);
        }
    }

    private void Upload ()
    {
        var content = File.ReadAllBytes(UploadFilePath);
        if (content == null) return;

        var file = new UnityGoogleDrive.Data.File() { Name = Path.GetFileName(UploadFilePath), Content = content };
        request = GoogleDriveFiles.CreateResumable(file, resumableSessionUri);
        request.Fields = new List<string> { "id", "name", "size", "createdTime" };
        request.Send().OnDone += SaveSessionUri;
    }

    private void SaveSessionUri (string resumableSessionUri)
    {
        this.resumableSessionUri = resumableSessionUri;
    }
}
