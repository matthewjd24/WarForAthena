using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoToCoordScript : MonoBehaviour
{
    public static GoToCoordScript inst;

    [SerializeField] TMP_InputField xcoordInp;
    [SerializeField] TMP_InputField ycoordInp;

    [SerializeField] Transform viewportCenter;

    [SerializeField] Vector2 viewportCenterOffsetSmall = new Vector2(-1.6f, 0);
    [SerializeField] Vector2 viewportCenterOffsetBig = new Vector2(-2.7f, 0);

    [SerializeField] Vector2 camPosAtTile1 = new Vector2(.58f, -11.9f);

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(viewportCenter.position);
        SetCoords(gridPos.x, gridPos.y);

        Vector2 offset = new Vector2(Camera.main.transform.position.x - viewportCenter.position.x, 
            Camera.main.transform.position.y - viewportCenter.position.y);
    }

    public void SetCoords(int x, int y)
    {
        xcoordInp.text = x.ToString();
        ycoordInp.text = y.ToString();
    }

    void ClampInpFields()
    {
        if (!xcoordInp.isFocused) {
            bool success = int.TryParse(xcoordInp.text, out int x);
            if (success) {
                x = Mathf.Clamp(x, 1, TileData.maxXandY - 1);
                xcoordInp.text = x.ToString();
            }
        }
        if (!ycoordInp.isFocused) {
            bool success = int.TryParse(ycoordInp.text, out int y);
            if (success) {
                y = Mathf.Clamp(y, 1, TileData.maxXandY - 1);
                ycoordInp.text = y.ToString();
            }
        }
    }

    private void Update()
    {
        ClampInpFields();
    }

    public void GoToCoords()
    {
        ClampInpFields();

        bool success = int.TryParse(xcoordInp.text, out int x);
        if (!success) return;
        success = int.TryParse(ycoordInp.text, out int y);
        if (!success) return;

        x -= 1;
        y -= 1;


        Vector2 adjustmentForCenterOfViewport;
        if (ZoomInOutCamera.isZoomedOut == true) adjustmentForCenterOfViewport = viewportCenterOffsetBig;
        else adjustmentForCenterOfViewport = viewportCenterOffsetSmall;


        //go from (1,1) to (x, 1)
        //each tile has a width of 1
        float tileXPos = x + camPosAtTile1.x - adjustmentForCenterOfViewport.x;
        //each tile has a height of 1/2
        float tileYPos = (x / 2f) + camPosAtTile1.y - adjustmentForCenterOfViewport.y;



        //go from (x,1) to (x,y)
        Vector2 finalPos = new Vector2((y * -1) + tileXPos, y / 2f + tileYPos);


        Camera.main.transform.position = new Vector3(finalPos.x, finalPos.y, Camera.main.transform.position.z);
    }
    public void GoToCoords(Vector2Int pos)
    {
        ClampInpFields();

        pos.x -= 1;
        pos.y -= 1;


        Vector2 adjustmentForCenterOfViewport;
        if (ZoomInOutCamera.isZoomedOut == true) adjustmentForCenterOfViewport = viewportCenterOffsetBig;
        else adjustmentForCenterOfViewport = viewportCenterOffsetSmall;


        //go from (1,1) to (x, 1)
        //each tile has a width of 1
        float tileXPos = pos.x + camPosAtTile1.x - adjustmentForCenterOfViewport.x;
        //each tile has a height of 1/2
        float tileYPos = (pos.x / 2f) + camPosAtTile1.y - adjustmentForCenterOfViewport.y;



        //go from (x,1) to (x,y)
        Vector2 finalPos = new Vector2((pos.y * -1) + tileXPos, pos.y / 2f + tileYPos);


        Camera.main.transform.position = new Vector3(finalPos.x, finalPos.y, Camera.main.transform.position.z);
    }
}
