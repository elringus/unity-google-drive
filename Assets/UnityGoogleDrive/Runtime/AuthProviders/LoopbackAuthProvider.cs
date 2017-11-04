using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Provides auth and refresh tokens using local loopback method to retrieve authorization code.
/// Implementation based on: https://github.com/googlesamples/oauth-apps-for-windows.
/// </summary>
public class LoopbackAuthProvider : NetworkServerSimple, IAuthProvider
{
    public event Action<IAuthProvider> OnDone;

    public bool IsDone { get; private set; }
    public bool IsError { get; private set; }
    public string AccessToken { get; private set; } 
    public string RefreshToken { get; private set; }

    private string authorizationCode;

    public LoopbackAuthProvider ()
    {
        
    }

    public AuthProviderYeildInstruction ProvideAuth (AuthCredentials authCredentials)
    {
        return null;
    }

    public override void OnConnected (NetworkConnection conn)
    {
        base.OnConnected(conn);

        Debug.Log("LoopbackServer: OnConnected " + conn.address);
    }

    public override void OnConnectError (int connectionId, byte error)
    {
        base.OnConnectError(connectionId, error);

        Debug.Log("LoopbackServer: OnConnectError connectionID: " + connectionId.ToString() + " error: " + error.ToString());
    }

    public override void OnData (NetworkConnection conn, int receivedSize, int channelId)
    {
        base.OnData(conn, receivedSize, channelId);

        Debug.Log("LoopbackServer: OnData conn: " + conn.ToString() + " receivedSize: " + receivedSize.ToString());
    }

    public override void OnDataError (NetworkConnection conn, byte error)
    {
        base.OnDataError(conn, error);

        Debug.Log("LoopbackServer: OnDataError conn: " + conn.ToString() + " error: " + error.ToString());
    }

    public override void OnDisconnected (NetworkConnection conn)
    {
        base.OnDisconnected(conn);

        Debug.Log("LoopbackServer: OnDisconnected conn: " + conn.ToString());
    }
}
