using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

public class EventManager : MonoBehaviour
{
    public static EventManager inst;

    public static event MsgRecEventHandler MsgReceived;
    public delegate void MsgRecEventHandler(string[] msg);

    //public static event DcEv DCed;
    //public delegate void DcEv();

    //public static bool invokeDC = false;

    public static List<string[]> queuedMessages = new List<string[]>();

    //static Dictionary<int, NetMsg.Message> dict = new Dictionary<int, NetMsg.Message>();

    //static List<KeyValuePair<NetMsg.Message, byte[]>> queuedMessages = new List<KeyValuePair<NetMsg.Message, byte[]>>();
    //int framesSinceMsg = 0;

    private void Awake()
    {
        inst = this;

        //dict.Clear();

        /*
        NetMsg.Ping resp = new NetMsg.Ping();
        NetMsg.Login req = new NetMsg.Login();
        NetMsg.Register reg = new NetMsg.Register();
        NetMsg.WriteTile til = new NetMsg.WriteTile();
        NetMsg.RequestTileRange reqTilR = new NetMsg.RequestTileRange();
        NetMsg.TileData tilDat = new NetMsg.TileData();
        dict.Add(resp.ID, resp);
        dict.Add(req.ID, req);
        dict.Add(reg.ID, reg);
        dict.Add(til.ID, til);
        dict.Add(reqTilR.ID, reqTilR);
        dict.Add(tilDat.ID, tilDat);*/
    }

    public static void ProcessServerMsg(byte[] dat)
    {
        /*int msgID = BitConverter.ToInt32(dat, 0);
        byte[] data = dat.Skip(4).ToArray();*/

        string message = System.Text.Encoding.UTF8.GetString(dat);

        string[] parts = message.Split(';');

        if(parts[parts.Length - 1] == "") {
            parts = parts.Take(parts.Count() - 1).ToArray();
        }

        queuedMessages.Add(parts);

        //dict.TryGetValue(msgID, out NetMsg.Message msg);
        //msg.data = data;

        //queuedMessages.Add(new KeyValuePair<NetMsg.Message, byte[]>(msg, data));
    }

    private void Update()
    {
        if (queuedMessages.Count == 0) return;
        if (MsgReceived == null) return;

        try {
            foreach (var e in queuedMessages) {
                MsgReceived(e);
            }
        }
        catch (Exception e) {
            if (e.Message.Contains("modified")) {
                Debug.Log("Was modified- try again next frame");
                return;
            }

            Debug.LogError(e);
        }

        queuedMessages.Clear();

        /*
        
        List<KeyValuePair<NetMsg.Message, byte[]>> queuedMsg = new List<KeyValuePair<NetMsg.Message, byte[]>>();

        try {
            queuedMsg.AddRange(queuedMessages);
        }
        catch (Exception e){
            //Debug.Log(e);
            return;
        }

        queuedMessages.Clear();

        foreach (var e in queuedMsg) {
            e.Key.Receive(e.Value);
        }*/
    }
}
