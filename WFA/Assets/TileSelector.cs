using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    Vector3Int lastselectedcell;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector2 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPos = TileMapGenerator.map.WorldToCell(mousPos);

            //Debug.Log(gridPos);
            return;

            lastselectedcell = gridPos;
            TileData.mapTiles.TryGetValue(gridPos, out TileInfo value);

            if (value != null) {
                Debug.Log(gridPos + ", " + value.type + ", " + value.id);
                //SendMessages.inst.WriteTile((Vector2Int)gridPos, (int)value.type);
            }
            else {
                //Debug.Log("This tile isn't in the dict");
            }
        }
    }
}
