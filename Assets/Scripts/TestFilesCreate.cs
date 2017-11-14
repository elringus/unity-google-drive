using UnityEngine;

public class TestFilesCreate : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 940, 580);
    public Texture2D ImageToUpload;

    private GoogleDriveFiles.CreateRequest request;
    private string result;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Upload Image");
    }

    private void InfoWindowGUI (int windowId)
    {
        if (request != null && request.IsRunning)
        {
            GUILayout.Label(string.Format("Loading: {0:P2}", request.Progress));
        }
        else if (GUILayout.Button("Upload")) Upload();

        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.Label(result);
        }
    }

    private void Upload ()
    {
        var content = System.Text.Encoding.ASCII.GetBytes("dsfjodsjfoi32rj9032j02fjf");//ImageToUpload.GetRawTextureData();
        var file = new Data.File() { Name = "TestUnityGoogleDriveFilesUpload", Content = content };
        request = GoogleDriveFiles.Create(file);
        request.Send().OnDone += PrintResult;
    }

    private void PrintResult (Data.File file)
    {
        result = string.Format("Name: {0} Size: {1:0}MB Created: {2:dd.MM.yyyy}",
                file.Name,
                file.Size * .000001f,
                file.CreatedTime);
    }
}
