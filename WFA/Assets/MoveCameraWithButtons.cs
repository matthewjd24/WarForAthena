using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCameraWithButtons : MonoBehaviour
{
    [SerializeField] int distance;

    public void MoveCameraWithDirections(int dir)
    {
        Transform cam = Camera.main.transform;
        Vector3 newPos = new Vector3();

        if (dir == 1) {
            newPos = new Vector3(cam.position.x - distance, cam.position.y + (distance / 2), cam.position.z);
        }
        else if (dir == 2) {
            newPos = new Vector3(cam.position.x + distance, cam.position.y + (distance / 2), cam.position.z);
        }
        else if (dir == 3) {
            newPos = new Vector3(cam.position.x + distance, cam.position.y - (distance / 2), cam.position.z);
        }
        else if (dir == 4) {
            newPos = new Vector3(cam.position.x - distance, cam.position.y - (distance / 2), cam.position.z);
        }

        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(newPos);

        if (gridPos.x < 1) return;
        if (gridPos.x > TileData.maxXandY) return;
        if (gridPos.y < 1) return;
        if (gridPos.y > TileData.maxXandY) return;


        Camera.main.transform.position = newPos;
        AutomaticallyDownloadTiles.inst.DownloadTiles();
    }
}
