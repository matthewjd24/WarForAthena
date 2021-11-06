using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TileTooltip : MonoBehaviour
{
    [SerializeField] RectTransform tooltip;

    [SerializeField] TextMeshProUGUI coordText;
    [SerializeField] TextMeshProUGUI typeText;

    [SerializeField] Vector2 lastMousePos;
    [SerializeField] float timeMouseStill = 0;

    private void Update()
    {
        Vector2 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPos = TileMapGenerator.map.WorldToCell(mousPos);
        bool isInBounds = TileData.CheckIfInsideBoundaries(gridPos.x, gridPos.y);

        bool isOverUI = EventSystem.current.IsPointerOverGameObject();

        if (isOverUI || !isInBounds) {
            if (tooltip.gameObject.activeSelf) {
                tooltip.gameObject.SetActive(false);
            }
            return;
        }
        else {
            if (!tooltip.gameObject.activeSelf) {
                tooltip.gameObject.SetActive(true);
            }
        }


        Vector2 mousePos = Input.mousePosition;
        float xDiff = Mathf.Abs(mousePos.x - lastMousePos.x);
        float yDiff = Mathf.Abs(mousePos.y - lastMousePos.y);

        /*if (xDiff < .05f && yDiff < .05f) {
            timeMouseStill += Time.deltaTime;
        }
        else {
            timeMouseStill = 0;
            lastMousePos = mousePos;

            if (tooltip.gameObject.activeSelf) {
                tooltip.gameObject.SetActive(false);
            }
            return;
        }

        if (!tooltip.gameObject.activeSelf) {
            
            if(timeMouseStill > .2f) {

                tooltip.gameObject.SetActive(true);
            }
            else {
                return;
            }
        }*/

        
        coordText.text = $"({gridPos.x}, {gridPos.y})";

        bool success = TileData.mapTiles.TryGetValue(gridPos, out TileInfo info);
        string type = "";
        if (success) {
            type = info.type.ToString();
        }
        typeText.text = type;


        mousePos.x += 25;
        mousePos.y -= Screen.height / 25;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        tooltip.position = mousePos;
        
    }
}
