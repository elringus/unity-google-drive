using System.Collections;
using System.Collections.Generic;
using UnityCommon;
using UnityEngine;
using UnityGoogleDrive;

public class ExampleGetFileByPath : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 940, 580);

    private GoogleDriveFiles.ListRequest request;
    private string filePath = string.Empty;
    private string result = string.Empty;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Get File By Path");
    }

    private void InfoWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
        }
        else 
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("File path:", GUILayout.Width(70));
            filePath = GUILayout.TextField(filePath);
            if (GUILayout.Button("Get", GUILayout.Width(100)))
                StartCoroutine(GetFileByPathRoutine(filePath));
            GUILayout.EndHorizontal();
        }

        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Result:", GUILayout.Width(70));
            result = GUILayout.TextField(result);
            GUILayout.EndHorizontal();
        }
    }

    private IEnumerator GetFileByPathRoutine (string filePath)
    {
        // A folder in Google Drive is actually a file with the MIME type 'application/vnd.google-apps.folder'. 
        // Hierarchy relationship is implemented via File's 'Parents' property. To get the actual file using it's path 
        // we have to find ID of the file's parent folder, and for this we need IDs of all the folders in the chain. 
        // Thus, we need to traverse the entire hierarchy chain using List requests. 
        // More info about the Google Drive folders: https://developers.google.com/drive/v3/web/folder.

        var fileName = filePath.Contains("/") ? filePath.GetAfter("/") : filePath;
        var parentNames = filePath.Contains("/") ? filePath.GetBeforeLast("/").Split('/') : null;

        // Resolving folder IDs one by one to find ID of the file's parent folder.
        var parendId = "root"; // 'root' is alias ID for the root folder in Google Drive.
        if (parentNames != null)
        {
            for (int i = 0; i < parentNames.Length; i++)
            {
                request = new GoogleDriveFiles.ListRequest();
                request.Fields = new List<string> { "files(id)" };
                request.Q = string.Format("'{0}' in parents and name = '{1}' and mimeType = 'application/vnd.google-apps.folder' and trashed = false",
                    parendId, parentNames[i]);

                yield return request.Send();

                if (request.IsError || request.ResponseData.Files == null || request.ResponseData.Files.Count == 0)
                {
                    result = string.Format("Failed to retrieve '{0}' part of '{1}' file path.", parentNames[i], filePath);
                    yield break;
                }

                if (request.ResponseData.Files.Count > 1)
                    Debug.LogWarning(string.Format("Multiple '{0}' folders been found.", parentNames[i]));

                parendId = request.ResponseData.Files[0].Id;
            }
        }

        // Searching the file.
        request = new GoogleDriveFiles.ListRequest();
        request.Fields = new List<string> { "files(id, size, modifiedTime)" };
        request.Q = string.Format("'{0}' in parents and name = '{1}'", parendId, fileName);

        yield return request.Send();

        if (request.IsError || request.ResponseData.Files == null || request.ResponseData.Files.Count == 0)
        {
            result = string.Format("Failed to retrieve '{0}' file.", filePath);
            yield break;
        }

        if (request.ResponseData.Files.Count > 1)
            Debug.LogWarning(string.Format("Multiple '{0}' files been found.", filePath));

        var file = request.ResponseData.Files[0];

        result = string.Format("ID: {0} Size: {1:0.00}MB Modified: {2:dd.MM.yyyy HH:MM:ss}",
            file.Id, file.Size * .000001f, file.CreatedTime);
    }
}
