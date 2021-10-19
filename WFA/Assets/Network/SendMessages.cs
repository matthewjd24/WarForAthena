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

    public void RequestTiles()
    {
        int startX = 0;
        int endX = 30;

        int startY = 0;
        int endY = 30;

        new NetMsg.RequestTileRange() {
            world = 1,
            startX = startX,
            endX = endX,
            startY = startY,
            endY = endY
        }.Send();
    }

    public void WriteTile(Vector2Int pos, int tileType)
    {
        new NetMsg.WriteTile() {
            world = 1,
            hasCity = false,
            x = pos.x,
            y = pos.y,
            tileType = tileType,
        }.Send();
    }
}
