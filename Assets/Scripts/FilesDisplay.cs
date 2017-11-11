using System.Collections.Generic;
using UnityEngine;

public class FilesDisplay : MonoBehaviour
{
    public Rect WindowRect = new Rect(300, 0, 500, 0);

    private GoogleDriveFiles.ListRequest listRequest;
    private string result;
    private Vector2 scrollPos;

    private void Start ()
    {
        UpdateInfo();
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

        if (GUILayout.Button("Refresh"))
            UpdateInfo();
    }

    private void UpdateInfo ()
    {
        listRequest = GoogleDriveFiles.List();
        listRequest.Fields = new List<string> { "files(name, size, createdTime)" };
        listRequest.Send().OnDone += GenerateFilesList;
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
