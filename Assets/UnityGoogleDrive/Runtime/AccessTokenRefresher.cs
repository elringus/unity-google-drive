using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessTokenRefresher
{
    public event Action<AccessTokenRefresher> OnDone;

    public bool IsDone { get; private set; }
    public bool IsError { get; private set; }
    public string AccesToken { get; private set; }

    public void RefreshAccessToken (AuthCredentials authCredentials, string refreshToken)
    {
        if (OnDone != null) OnDone.Invoke(this);
    }
}
