using System.Text;
using UnityEngine;
using UnityGoogleDrive;

public class ExampleExportDocument : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 300, 200);

    private GoogleDriveFiles.ExportRequest request;
    private string fileId = string.Empty;
    private string result = string.Empty;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Export Google Document");
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
            GUILayout.Label("Doc file ID:", GUILayout.Width(85));
            fileId = GUILayout.TextField(fileId);
            if (GUILayout.Button("Download", GUILayout.Width(100))) ExportDocument();
            GUILayout.EndHorizontal();

            GUILayout.Label("Result:", GUILayout.Width(85));
            GUILayout.TextArea(result);
        }
    }

    private void ExportDocument ()
    {
        request = GoogleDriveFiles.Export(fileId, "text/plain");
        request.Send().OnDone += file => result = Encoding.UTF8.GetString(file.Content);
    }
}
