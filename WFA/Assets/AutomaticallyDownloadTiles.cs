using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticallyDownloadTiles : MonoBehaviour
{
    public static AutomaticallyDownloadTiles inst;
    public int radius = 5;

    private void Awake()
    {
        inst = this;
    }

    private void OnEnable()
    {
        Vector2 cameraPos = Camera.main.transform.position;
        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(cameraPos);

        Vector2Int startPos = new Vector2Int(gridPos.x - radius, gridPos.y - radius);
        Vector2Int endPos = new Vector2Int(gridPos.x + radius, gridPos.y + radius);
        SendMessages.RequestTiles(startPos, endPos);
    }

    public void DownloadTiles()
    {
        Vector2 cameraPos = Camera.main.transform.position;
        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(cameraPos);



        Vector2Int startPos = new Vector2Int(gridPos.x - radius, gridPos.y - radius);
        Vector2Int endPos = new Vector2Int(gridPos.x + radius, gridPos.y + radius);
        SendMessages.RequestTiles(startPos, endPos);
    }
}
