using System.Text;
using UnityEngine;
using UnityGoogleDrive;

public class ExampleExportDocument : AdaptiveWindowGUI
{
    private GoogleDriveFiles.ExportRequest request;
    private string fileId = string.Empty;
    private string result = string.Empty;

    protected override void OnWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
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
