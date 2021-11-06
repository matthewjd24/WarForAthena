using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRefreshTiles : MonoBehaviour
{
    public float secsLookingAtThisTile;
    Vector2 cameraPos;
    [SerializeField] Transform cam;

    private void Update()
    {
        Vector2 currentCamPos = cam.position;

        if(currentCamPos == cameraPos) {
            secsLookingAtThisTile += Time.deltaTime;
        }
        else {
            cameraPos = currentCamPos;
            secsLookingAtThisTile = 0;
            return;
        }

        if(secsLookingAtThisTile > 15f) {
            AutomaticallyDownloadTiles.inst.DownloadTiles();
            secsLookingAtThisTile = 0;
        }
    }
}
