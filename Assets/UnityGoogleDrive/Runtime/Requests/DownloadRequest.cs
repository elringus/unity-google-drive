using UnityEngine;
using UnityEngine.Networking;

public class DownloadRequest : GoogleDriveRequest<DownloadRequest>
{
    public DownloadRequest ()
        : base(@"https://www.googleapis.com/drive/v3/files", UnityWebRequest.kHttpVerbGET) { }

    protected override void OnBeforeSend (UnityWebRequest webRequest)
    {
        webRequest.downloadHandler = new DownloadHandlerBuffer();
    }

    protected override void OnBeforeDone (UnityWebRequest webRequest)
    {
        Debug.Log("Downloaded: " + webRequest.downloadHandler.text);
    }
}
