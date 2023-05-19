using System.IO;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesCreateResumable : AdaptiveWindowGUI
{
    public string UploadFilePath;

    private GoogleDriveFiles.ResumableCreateRequest request;
    private string resumableSessionUri;

    protected override void OnWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
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
        request.Send().OnDone += SaveSessionUri;
    }

    private void SaveSessionUri (string resumableSessionUri)
    {
        this.resumableSessionUri = resumableSessionUri;
    }
}
