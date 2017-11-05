using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDownload : MonoBehaviour
{
    private DownloadRequest request;

    private void Start ()
    {
        request = new DownloadRequest();
        request.Send();
    }

    private void OnGUI ()
    {
        if (request != null && !string.IsNullOrEmpty(request.Result))
            GUILayout.TextArea(request.Result.Substring(0, 1000));
        else GUILayout.TextArea(Time.realtimeSinceStartup.ToString());
    }
}
