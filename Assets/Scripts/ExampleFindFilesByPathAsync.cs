using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class ExampleFindFilesByPathAsync : MonoBehaviour
{
    #if NET_4_6 || NET_STANDARD_2_0
    public Rect WindowRect = new Rect(10, 10, 940, 580);

    private bool running;
    private string filePath = string.Empty;
    private bool folder;
    private Dictionary<string, string> results;
    private Vector2 scrollPos;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Find Files By Path");
    }

    private void InfoWindowGUI (int windowId)
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
        }
    }

    private async void FindFilesByPathAsync (string path)
    {
        running = true;
        var files = await Helpers.FindFilesByPathAsync(path, fields: new List<string> { "files(id, name, size, modifiedTime)" }, mime: folder ? Helpers.FOLDER_MIME_TYPE : null);
        BuildResults(files);
        running = false;
    }

    private void BuildResults (List<UnityGoogleDrive.Data.File> fileList)
    {
        if (fileList.Count == 0) { results = new Dictionary<string, string> { [string.Empty] = "Non files found." }; return; }

        results = new Dictionary<string, string>();

        foreach (var file in fileList)
        {
            var fileInfo = string.Format("Name: {0} Size: {1:0.00}MB Modified: {2:dd.MM.yyyy}",
                file.Name,
                file.Size * .000001f,
                file.ModifiedTime);
            results.Add(file.Id, fileInfo);
        }
    }

    #else
    private void OnGUI () { GUILayout.Label(".NET 4.x scripting backend is not enabled."); }
    #endif
}
