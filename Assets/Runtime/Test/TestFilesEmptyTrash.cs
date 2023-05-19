using UnityEngine;
using UnityGoogleDrive;

public class TestFilesEmptyTrash : AdaptiveWindowGUI
{
    private GoogleDriveFiles.EmptyTrashRequest request;
    private string result;

    protected override void OnWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
        }
        else
        {
            if (GUILayout.Button("Empty Trash")) EmptyTrash();
        }
        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.Label(result);
        }
    }

    private void EmptyTrash ()
    {
        request = GoogleDriveFiles.EmptyTrash();
        request.Send().OnDone += _ => result = request.IsError ? request.Error : "trash emptied";
    }
}
