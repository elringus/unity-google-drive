using System.Collections.Generic;
using UnityEngine;

public class FilesDisplay : MonoBehaviour
{
    public Rect WindowRect = new Rect(300, 0, 500, 0);
    public int ResultsPerPage = 100;

    private GoogleDriveFiles.ListRequest listRequest;
    private string result;
    private string query = string.Empty;
    private Vector2 scrollPos;

    private void Start ()
    {
        ListFiles();
    }

    private void OnGUI ()
    {
        GUILayout.Window(1, WindowRect, InfoWindowGUI, "Google Drive Files List");
    }

    private void InfoWindowGUI (int windowId)
    {

        if (listRequest.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", listRequest.Progress));
        }
        else if (!string.IsNullOrEmpty(result))
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.Label(result);
            GUILayout.EndScrollView();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("File name:", GUILayout.Width(70));
        query = GUILayout.TextField(query);
        if (GUILayout.Button("Search", GUILayout.Width(100))) ListFiles();
        if (NextPageExists() && GUILayout.Button(">>", GUILayout.Width(50)))
            ListFiles(listRequest.Response.NextPageToken);
        GUILayout.EndHorizontal();
    }

    private void ListFiles (string nextPageToken = null)
    {
        listRequest = GoogleDriveFiles.List();
        listRequest.Fields = new List<string> { "nextPageToken, files(name, size, createdTime)" };
        listRequest.PageSize = ResultsPerPage;
        if (!string.IsNullOrEmpty(query))
            listRequest.Q = string.Format("name contains '{0}'", query);
        if (!string.IsNullOrEmpty(nextPageToken))
            listRequest.PageToken = nextPageToken;
        listRequest.Send().OnDone += GenerateFilesList;
    }

    private bool NextPageExists ()
    {
        return listRequest != null && 
            listRequest.Response != null && 
            !string.IsNullOrEmpty(listRequest.Response.NextPageToken);
    }

    private void GenerateFilesList (Data.FileList fileList)
    {
        result = string.Empty;

        foreach (var file in fileList.Files)
            result += string.Format("Name: {0} Size: {1:0}MB Created: {2:dd.MM.yyyy}\n",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime);
    }
}
