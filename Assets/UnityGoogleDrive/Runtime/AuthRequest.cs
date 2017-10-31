using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AuthRequest : GoogleDriveRequest<AuthRequest>
{
    public string Result;

    public AuthRequest () 
        : base(@"https://unity3d.com/robots.txt", UnityWebRequest.kHttpVerbGET)
    {

    }

    protected override void OnBeforeSend (UnityWebRequest webRequest)
    {
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        webRequest.SetRequestHeader("Content-Type", "text/plain");
    }

    protected override void OnBeforeDone (UnityWebRequest webRequest)
    {
        Result = webRequest.downloadHandler.text;
    }
}
