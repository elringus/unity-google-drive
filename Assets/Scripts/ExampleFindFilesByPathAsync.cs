using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityGoogleDrive;

public class ExampleFindFilesByPathAsync : AdaptiveWindowGUI
{
    #if NET_4_6 || NET_STANDARD_2_0
    private bool running;
    private string filePath = string.Empty;
    private string uploadFilePath = string.Empty;
    private bool folder;
    private Dictionary<string, string> results;
    private Vector2 scrollPos;

    protected override void OnWindowGUI (int windowId)
    {
        if (running) GUILayout.Label("Loading, please wait...");

        if (!running && results != null)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var result in results)
            {
                GUILayout.Label(result.Value);
                if (string.IsNullOrEmpty(result.Key)) continue;
                GUILayout.BeginHorizontal();
                GUILayout.Label("ID:", GUILayout.Width(20));
                GUILayout.TextField(result.Key);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        if (!running)
        {
            GUILayout.BeginHorizontal();
            folder = GUILayout.Toggle(folder, "Folder", GUILayout.Width(65));
            GUILayout.Label("File path:", GUILayout.Width(70));
            filePath = GUILayout.TextField(filePath);
            if (GUILayout.Button("Search", GUILayout.Width(100))) FindFilesByPathAsync(filePath);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Upload file path:", GUILayout.Width(100));
            uploadFilePath = GUILayout.TextField(uploadFilePath);
            GUILayout.EndHorizontal();
        }
    }

    private async void FindFilesByPathAsync (string path)
    {
        running = true;

        if (File.Exists(uploadFilePath))
        {
            var uploadFile = new UnityGoogleDrive.Data.File { Content = File.ReadAllBytes(uploadFilePath) };
            uploadFile.Id = await Helpers.CreateOrUpdateFileAtPathAsync(uploadFile, filePath);
            BuildResults(new List<UnityGoogleDrive.Data.File> { uploadFile });
        }
        else
        {
            var files = await Helpers.FindFilesByPathAsync(path, fields: new List<string> { "files(id, name, size, mimeType, modifiedTime)" }, mime: folder ? Helpers.FolderMimeType : null);
            BuildResults(files);
        }

        running = false;
    }

    private void BuildResults (List<UnityGoogleDrive.Data.File> fileList)
    {
        if (fileList.Count == 0) { results = new Dictionary<string, string> { [string.Empty] = "Non files found." }; return; }

        results = new Dictionary<string, string>();

        foreach (var file in fileList)
        {
            var fileInfo = string.Format("Name: {0} Size: {1:0.00}MB '{2}' Modified: {3:dd.MM.yyyy}",
                file.Name,
                file.Size * .000001f,
                file.MimeType,
                file.ModifiedTime);
            results.Add(file.Id ?? "Failed", fileInfo);
        }
    }

    #else
    protected override void OnWindowGUI (int windowId)
    { 
        GUILayout.Label(".NET 4.x scripting backend is not enabled.");
    }
    #endif
}
