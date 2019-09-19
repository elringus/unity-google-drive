using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesGet : AdaptiveWindowGUI
{
    private GoogleDriveFiles.GetRequest request;
    private string result;
    private string fileId = string.Empty;

    protected override void OnWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("File ID:", GUILayout.Width(70));
            fileId = GUILayout.TextField(fileId);
            if (GUILayout.Button("Get", GUILayout.Width(100))) GetFile();
            GUILayout.EndHorizontal();
        }
        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.Label(result);
        }
    }

    private void GetFile ()
    {
        request = GoogleDriveFiles.Get(fileId);
        request.Fields = new List<string> { "name, size, createdTime" };
        request.Send().OnDone += BuildResultString;
    }

    private void BuildResultString (UnityGoogleDrive.Data.File file)
    {
        result = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy HH:MM:ss}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime);
    }
}
