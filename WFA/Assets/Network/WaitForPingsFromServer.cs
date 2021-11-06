using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForPingsFromServer : MonoBehaviour
{
    public static WaitForPingsFromServer inst;
    public int secsSinceLastPing;

    void Start()
    {
        inst = this;

        StopAllCoroutines();
        StartCoroutine(checkStream());

        EventManager.MsgReceived += MsgReceived;
    }
    private void OnDisable()
    {
        EventManager.MsgReceived -= MsgReceived;
    }

    public void MsgReceived(string[] msg)
    {
        if (msg[0] != "ping") return;

        secsSinceLastPing = 0;
        //new NetMsg.Ping().Send();

        _ = NetMsg.SendMsg("ping;");
    }

    IEnumerator checkStream()
    {
        yield return new WaitUntil(() => SslClient.sslStream != null);
        yield return new WaitUntil(() => SslClient.sslStream.IsAuthenticated);

        while (true) {
            yield return new WaitForSeconds(1);
            secsSinceLastPing++;

            if(secsSinceLastPing > 6) {
                Debug.Log("Ping timeout. Closing the connection");
                SslClient.CloseConnection();
                break;
            }
        }
    }
}
