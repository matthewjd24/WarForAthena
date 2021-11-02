using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessages : MonoBehaviour
{
    public static SendMessages inst;

    private void Awake()
    {
        inst = this;
    }

    public static void RequestTiles(Vector2Int startPos, Vector2Int endPos)
    {
        new NetMsg.RequestTileRange() {
            world = PlayerID.currentWorld,
            startX = startPos.x,
            endX = endPos.x,
            startY = startPos.y,
            endY = endPos.y
        }.Send();
    }

    public static void WriteTile(Vector2Int pos, int tileType)
    {
        new NetMsg.WriteTile() {
            world = PlayerID.currentWorld,
            hasCity = false,
            x = pos.x,
            y = pos.y,
            tileType = tileType,
        }.Send();
    }
}
