using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

public class EventManager : MonoBehaviour
{
    public static EventManager inst;

    public static event DcEv DCed;
    public delegate void DcEv();

    public static bool invokeDC = false;

    static Dictionary<int, NetMsg.Message> dict = new Dictionary<int, NetMsg.Message>();

    static List<KeyValuePair<NetMsg.Message, byte[]>> queuedMessages = new List<KeyValuePair<NetMsg.Message, byte[]>>();

    private void Awake()
    {
        inst = this;

        dict.Clear();

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
        dict.Add(tilDat.ID, tilDat);
    }

    public static void ProcessServerMsg(byte[] dat)
    {
        int msgID = BitConverter.ToInt32(dat, 0);
        byte[] data = dat.Skip(4).ToArray();

        dict.TryGetValue(msgID, out NetMsg.Message msg);
        msg.data = data;

        queuedMessages.Add(new KeyValuePair<NetMsg.Message, byte[]>(msg, data));
    }

    private void Update()
    {
        if (queuedMessages.Count == 0) return;

        List<KeyValuePair<NetMsg.Message, byte[]>> queuedMsg = new List<KeyValuePair<NetMsg.Message, byte[]>>();
        queuedMsg.AddRange(queuedMessages);

        queuedMessages.Clear();

        foreach (var e in queuedMsg) {
            e.Key.Receive(e.Value);
        }
    }
}
