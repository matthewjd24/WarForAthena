using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraWithButtons : MonoBehaviour
{
    [SerializeField] int distance;
    [SerializeField] Transform viewportCenter;

    public void MoveCameraWithDirections(int dir)
    {
        Transform cam = Camera.main.transform;

        Vector2 movement;

        if (dir == 1) {
            movement = new Vector2(distance * -1, distance / 2);
        }
        else if (dir == 2) {
            movement = new Vector2(distance, distance / 2);
        }
        else if (dir == 3) {
            movement = new Vector2(distance, distance / -2);
        }
        else {
            movement = new Vector2(distance * -1, distance / -2);
        }


        Vector3 newPos = new Vector3(cam.position.x + movement.x, cam.position.y + movement.y, cam.position.z);
        Vector3 newCenterPos = new Vector3(viewportCenter.position.x + movement.x, viewportCenter.position.y + movement.y,
            viewportCenter.position.z);

        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(newCenterPos);

        if (gridPos.x < 1) return;
        if (gridPos.x >= TileData.maxXandY) return;
        if (gridPos.y < 1) return;
        if (gridPos.y >= TileData.maxXandY) return;


        Camera.main.transform.position = newPos;
        GoToCoordScript.inst.SetCoords(gridPos.x, gridPos.y);
        AutomaticallyDownloadTiles.inst.DownloadTiles();
    }
}
