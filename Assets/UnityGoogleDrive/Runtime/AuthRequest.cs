using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;

public class AuthRequest : GoogleDriveRequest<AuthRequest>
{
    public string Result;

    public AuthRequest () 
        : base(@"https://unity3d.com/robots.txt", UnityWebRequest.kHttpVerbGET)
    {
        // Generates state and PKCE values.
        //string state = randomDataBase64url(32);
        //string code_verifier = randomDataBase64url(32);
        //string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
        //const string code_challenge_method = "S256";
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
