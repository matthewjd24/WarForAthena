using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConnectScreen : MonoBehaviour
{
    public void Connect(string ip)
    {
        if (SslClient.sslStream != null && !SslClient.sslStream.IsClosed && SslClient.sslStream.IsAuthenticated) return;
        SslClient.inst.Connect(ip);
    }

    //public void SendTestMessage()
    //{
    //    new NetMsg.TextMessage() {
    //        msg = "hi"
    //    }.Send();
    //}
}
