using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesCreate : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 940, 580);
    public Texture2D ImageToUpload;

    private GoogleDriveFiles.CreateRequest request;
    private string result;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Upload Image");
    }

    private void InfoWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
        }
        else if (GUILayout.Button("Upload")) Upload();

        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.Label(result);
        }
    }

    private void Upload ()
    {
        var content = ImageToUpload.EncodeToPNG();
        var file = new UnityGoogleDrive.Data.File() { Name = "TestUnityGoogleDriveFilesUpload.png", Content = content, MimeType = "image/png" };
        request = GoogleDriveFiles.Create(file);
        request.Fields = new List<string> { "id", "name", "size", "createdTime" };
        request.Send().OnDone += PrintResult;
    }

    private void PrintResult (UnityGoogleDrive.Data.File file)
    {
        result = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy HH:MM:ss}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime);
    }
}
