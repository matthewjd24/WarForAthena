using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Data;
using System.Threading.Tasks;

public class TileMapGenerator : MonoBehaviour
{
    public static TileMapGenerator inst;

    [SerializeField] Tilemap themap;
    public static Tilemap map;

    [SerializeField] int xMin = 0;
    [SerializeField] int xMax = 30;
    [SerializeField] int yMin = 0;
    [SerializeField] int yMax = 30;


    Dictionary<TileType, float> tileChances = new Dictionary<TileType, float>();

    [SerializeField] float forestChance = .2f;
    [SerializeField] float mountainChance = .12f;
    [SerializeField] float swampChance = .06f;
    [SerializeField] float grasslandChance = .17f;
    [SerializeField] float mineralDepositChance = .02f;
    [SerializeField] float wastelandChance = .04f;

    private void Awake()
    {
        map = themap;
        inst = this;
    }



    void GenerateChances()
    {
        tileChances.Clear();
        map = themap;

        tileChances.Add(TileType.Forest, forestChance);
        tileChances.Add(TileType.Mountain, mountainChance);
        tileChances.Add(TileType.Swamp, swampChance);
        tileChances.Add(TileType.Grassland, grasslandChance);
        tileChances.Add(TileType.MineralDeposit, mineralDepositChance);
        tileChances.Add(TileType.Wasteland, wastelandChance);

        float percentUsed = 0;
        foreach (var e in tileChances) {
            percentUsed += e.Value;
        }

        tileChances.Add(TileType.Plain, 1f - percentUsed);
    }

    public void SetTile(int x, int y, TileType type)
    {
        TileBase newTile = PickRandomFromTypeList(type);
        TileBase PickRandomFromTypeList(TileType type)
        {
            TileData.tileListsByType.TryGetValue(type, out List<TileBase> list);

            if (list == null || list.Count == 0) {
                Debug.LogError("list is null for " + type);
                return null;
            }
            else {
                int index = UnityEngine.Random.Range(0, list.Count - 1);
                return list[index];
            }
        }

        TileInfo theInfo = new TileInfo(type, newTile);

        Vector3Int pos = new Vector3Int(x, y, 0);

        map.SetTile(pos, newTile);


        if (TileData.tilesInfo.ContainsKey(pos)) return;

        TileData.tilesInfo.Add(pos, theInfo);
    }

    [ContextMenu("GenerateTiles")]
    void Generate()
    {
        GenerateChances();

        TileData.tilesInfo.Clear();
        TileData.inst.MakeTileDefinitions();

        for (int x = xMin; x < xMax; x++) {
            for (int y = yMin; y < yMax; y++) {
                TileType newTileType = selectTileType();
                TileType selectTileType()
                {
                    float rand = UnityEngine.Random.Range(0, 1f);

                    float percUsed = 0;

                    foreach (var e in tileChances) {
                        if (rand < percUsed + e.Value) {
                            return e.Key;
                        }
                        else {
                            percUsed += e.Value;
                        }
                    }

                    return TileType.Plain;
                }
                SetTile(x, y, newTileType);
            }
        }

    }

    

    [ContextMenu("LogTiles")]
    async Task RecordSet()
    {
        int i = 0;
        foreach (var position in map.cellBounds.allPositionsWithin) {
            if (!map.HasTile(position)) {
                continue;
            }

            i++;

            TileBase tbase = map.GetTile(position);
            Debug.Log($"{position}, {tbase.name}");

            TileData.tilesInfo.TryGetValue(position, out TileInfo value);

            if (value != null) {
                Debug.Log("step " + i + ", pos " + position + ", " + value.type + ", " + value.id);
                SendMessages.WriteTile((Vector2Int)position, (int)value.type);
            }

            Task.Delay(10);
        }

        Debug.Log("Done!");
    }
}
