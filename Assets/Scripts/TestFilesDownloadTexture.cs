using UnityEngine;
using UnityGoogleDrive;

public class TestFilesDownloadTexture : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 300, 200);
    public SpriteRenderer SpriteRenderer;

    private GoogleDriveFiles.DownloadTextureRequest request;
    private string fileId = string.Empty;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Texture Downloader");
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
            GUILayout.Label("Texture file ID:", GUILayout.Width(85));
            fileId = GUILayout.TextField(fileId);
            if (GUILayout.Button("Download", GUILayout.Width(100))) DownloadTexture();
            GUILayout.EndHorizontal();
        }
    }

    private void DownloadTexture ()
    {
        request = GoogleDriveFiles.DownloadTexture(fileId, true);
        request.Send().OnDone += RenderImage;
    }

    private void RenderImage (UnityGoogleDrive.Data.TextureFile textureFile)
    {
        var texture = textureFile.Texture;
        var rect = new Rect(0, 0, texture.width, texture.height);
        SpriteRenderer.sprite = Sprite.Create(texture, rect, Vector2.one * .5f);
    }
}
