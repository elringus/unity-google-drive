using UnityEngine;
using UnityGoogleDrive;

public class TestFilesDelete : AdaptiveWindowGUI
{
    private GoogleDriveFiles.DeleteRequest request;
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
            if (GUILayout.Button("Delete", GUILayout.Width(100))) DeleteFile();
            GUILayout.EndHorizontal();
        }
        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.Label(result);
        }
    }

    private void DeleteFile ()
    {
        request = GoogleDriveFiles.Delete(fileId);
        request.Send().OnDone += _ => result = request.IsError ? request.Error : "file deleted";
    }
}
