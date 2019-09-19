using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesCreate : AdaptiveWindowGUI
{
    public string UploadFilePath;

    private GoogleDriveFiles.CreateRequest request;
    private string result;

    protected override void OnWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
        }
        else
        {
            UploadFilePath = GUILayout.TextField(UploadFilePath);
            if (GUILayout.Button("Upload To Root")) Upload(false);
            if (GUILayout.Button("Upload To AddData")) Upload(true);
        }

        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.TextField(result);
        }
    }

    private void Upload (bool toAppData)
    {
        var content = File.ReadAllBytes(UploadFilePath);
        if (content == null) return;

        var file = new UnityGoogleDrive.Data.File() { Name = Path.GetFileName(UploadFilePath), Content = content };
        if (toAppData) file.Parents = new List<string> { "appDataFolder" };
        request = GoogleDriveFiles.Create(file);
        request.Fields = new List<string> { "id", "name", "size", "createdTime" };
        request.Send().OnDone += PrintResult;
    }

    private void PrintResult (UnityGoogleDrive.Data.File file)
    {
        result = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy HH:MM:ss}\nID: {3}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime,
                file.Id);
    }
}
