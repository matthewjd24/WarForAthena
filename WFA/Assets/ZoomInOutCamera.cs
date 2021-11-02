using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOutCamera : MonoBehaviour
{
    public static bool isZoomedOut = false;

    public void Toggle()
    {
        if (isZoomedOut) {
            Camera.main.orthographicSize = 5;
            isZoomedOut = false;
            AutomaticallyDownloadTiles.inst.radius = 14;
        }
        else {
            Camera.main.orthographicSize = 10;
            isZoomedOut = true;
            AutomaticallyDownloadTiles.inst.radius = 28;
        }

        AutomaticallyDownloadTiles.inst.DownloadTiles();
    }
}
