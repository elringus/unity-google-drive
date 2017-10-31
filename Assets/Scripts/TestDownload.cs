using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDownload : MonoBehaviour
{
    private void Start ()
    {
        var authRequest = new AuthRequest();
        authRequest.OnDone += r => print(r.Result);
        authRequest.Send();
    }

}
