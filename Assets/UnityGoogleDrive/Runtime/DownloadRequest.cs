using UnityEngine;
using UnityEngine.Networking;

public class DownloadRequest : AuthorizedRequest<DownloadRequest>
{
    public DownloadRequest ()
        : base("", UnityWebRequest.kHttpVerbGET) { }

    protected override void OnBeforeSend (UnityWebRequest webRequest)
    {
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "text/plain");
    }

    protected override void OnBeforeDone (UnityWebRequest webRequest)
    {
        Debug.Log("Downloaded: " + webRequest.downloadHandler.text);
    }
}
