using UnityEngine;
using UnityGoogleDrive;

public class TestFilesDelete : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 940, 580);

    private GoogleDriveFiles.DeleteRequest request;
    private string result;
    private string fileId = string.Empty;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Delete File");
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
