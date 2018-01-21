using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesList : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 940, 580);
    [Range(1, 1000)]
    public int ResultsPerPage = 100;

    private GoogleDriveFiles.ListRequest request;
    private Dictionary<string, string> results;
    private string query = string.Empty;
    private Vector2 scrollPos;

    private void Start ()
    {
        ListFiles();
    }

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Files List");
    }

    private void InfoWindowGUI (int windowId)
    {
        if (request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
        }
        else if (results != null)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var result in results)
            {
                GUILayout.Label(result.Value);
                GUILayout.BeginHorizontal();
                GUILayout.Label("ID:", GUILayout.Width(20));
                GUILayout.TextField(result.Key);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("File name:", GUILayout.Width(70));
        query = GUILayout.TextField(query);
        if (GUILayout.Button("Search", GUILayout.Width(100))) ListFiles();
        if (NextPageExists() && GUILayout.Button(">>", GUILayout.Width(50)))
            ListFiles(request.ResponseData.NextPageToken);
        GUILayout.EndHorizontal();
    }

    private void ListFiles (string nextPageToken = null)
    {
        request = GoogleDriveFiles.List();
        request.Fields = new List<string> { "nextPageToken, files(id, name, size, createdTime)" };
        request.PageSize = ResultsPerPage;
        if (!string.IsNullOrEmpty(query))
            request.Q = string.Format("name contains '{0}'", query);
        if (!string.IsNullOrEmpty(nextPageToken))
            request.PageToken = nextPageToken;
        request.Send().OnDone += BuildResults;
    }

    private void BuildResults (UnityGoogleDrive.Data.FileList fileList)
    {
        results = new Dictionary<string, string>();

        foreach (var file in fileList.Files)
        {
            var fileInfo = string.Format("Name: {0} Size: {1:0.00}MB Created: {2:dd.MM.yyyy}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime);
            results.Add(file.Id, fileInfo);
        }
    }

    private bool NextPageExists ()
    {
        return request != null && 
            request.ResponseData != null && 
            !string.IsNullOrEmpty(request.ResponseData.NextPageToken);
    }
}
