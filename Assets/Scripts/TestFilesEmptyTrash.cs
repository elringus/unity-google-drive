using UnityEngine;
using UnityGoogleDrive;

public class TestFilesEmptyTrash : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 940, 580);

    private GoogleDriveFiles.EmptyTrashRequest request;
    private string result;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Empty Trash");
    }

    private void InfoWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
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
