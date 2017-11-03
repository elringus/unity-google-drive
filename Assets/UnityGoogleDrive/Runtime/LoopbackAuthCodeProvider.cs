using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoopbackServer : NetworkServerSimple
{
    public LoopbackServer ()
    {
        Debug.Log("LoopbackServer: Created");
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
        Debug.Log("LoopbackServer: OnDataError conn: " + conn.ToString() + " error: " + error.ToString());
    }

    public override void OnDisconnected (NetworkConnection conn)
    {
        base.OnDisconnected(conn);

        Debug.Log("LoopbackServer: OnDisconnected conn: " + conn.ToString());
    }
}
