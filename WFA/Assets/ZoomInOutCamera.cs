using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOutCamera : MonoBehaviour
{
    public static bool isZoomedOut = false;

    [SerializeField] Transform viewportRect;

    public void Toggle()
    {
        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(viewportRect.position);

        if (isZoomedOut) {
            Camera.main.orthographicSize = 6;
            isZoomedOut = false;
            AutomaticallyDownloadTiles.inst.radius = 14;
        }
        else {
            Camera.main.orthographicSize = 10;
            isZoomedOut = true;
            AutomaticallyDownloadTiles.inst.radius = 28;
        }

        GoToCoordScript.inst.GoToCoords(new Vector2Int(gridPos.x, gridPos.y));
        AutomaticallyDownloadTiles.inst.DownloadTiles();
    }
}
