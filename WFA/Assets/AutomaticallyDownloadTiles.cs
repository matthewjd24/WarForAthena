using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticallyDownloadTiles : MonoBehaviour
{
    public static AutomaticallyDownloadTiles inst;
    public int radius = 5;

    [SerializeField] RectTransform centerOfViewport;

    private void Awake()
    {
        inst = this;
    }

    private void OnEnable()
    {
        EventManager.MsgReceived += EventManager_MsgReceived;

        if (TileMapGenerator.map == null) return;

        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(centerOfViewport.position);

        Vector2Int startPos = new Vector2Int(gridPos.x - radius, gridPos.y - radius);
        Vector2Int endPos = new Vector2Int(gridPos.x + radius, gridPos.y + radius);
        _ = NetMsg.SendMsg($"requesttilerange;{1};{startPos.x};{endPos.x};{startPos.y};{endPos.y}");
    }

    private void EventManager_MsgReceived(string[] msg)
    {
        if (msg[0] != "tiledata") return;

        int world = int.Parse(msg[1]);
        int x = int.Parse(msg[2]);
        int y = int.Parse(msg[3]);
        int type = int.Parse(msg[4]);
        int city = int.Parse(msg[5]);

        TileMapGenerator.inst.SetTile(x, y, (TileType)type);
    }

    public void DownloadTiles()
    {
        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(centerOfViewport.position);



        Vector2Int startPos = new Vector2Int(gridPos.x - radius, gridPos.y - radius);
        Vector2Int endPos = new Vector2Int(gridPos.x + radius, gridPos.y + radius);
        _ = NetMsg.SendMsg($"requesttilerange;{1};{startPos.x};{endPos.x};{startPos.y};{endPos.y};");
    }
}
