using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class TestFilesDownload : MonoBehaviour
{
    public Rect WindowRect = new Rect(10, 10, 300, 200);
    public SpriteRenderer SpriteRenderer;

    private GoogleDriveFiles.DownloadRequest request;
    private string fileId = string.Empty;

    private void OnGUI ()
    {
        GUILayout.Window(0, WindowRect, InfoWindowGUI, "Google Drive Image Downloader");
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
            GUILayout.Label("Image file ID:", GUILayout.Width(85));
            fileId = GUILayout.TextField(fileId);
            if (GUILayout.Button("Download", GUILayout.Width(100))) DownloadImage();
            GUILayout.EndHorizontal();
        }
    }

    private void DownloadImage ()
    {
        var getRequest = GoogleDriveFiles.Get(fileId);
        getRequest.Fields = new List<string> { "id, imageMediaMetadata(width, height)" };
        getRequest.Send().OnDone += HandleImageMetaReceived;
    }

    private void HandleImageMetaReceived (UnityGoogleDrive.Data.File file)
    {
        request = GoogleDriveFiles.Download(file);
        request.Send().OnDone += RenderImage;
    }

    private void RenderImage (UnityGoogleDrive.Data.File file)
    {
        var texture = new Texture2D(file.ImageMediaMetadata.Width.Value, file.ImageMediaMetadata.Height.Value, TextureFormat.RGBA32, false);
        texture.LoadImage(file.Content); 
        var rect = new Rect(0, 0, texture.width, texture.height);
        SpriteRenderer.sprite = Sprite.Create(texture, rect, Vector2.one * .5f);
    }
}
