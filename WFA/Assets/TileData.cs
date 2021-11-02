using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType
{
    Plain,
    Forest,
    Mountain,
    Swamp,
    Grassland,
    Wasteland,
    MineralDeposit,
}

public class TileInfo
{
    public TileInfo(TileType typ, TileBase til)
    {
        id = TileData.inst.nextID;
        TileData.inst.nextID++;

        type = typ;
    }

    public TileType type { get; private set; }
    public int id { get; private set; }
    public TileBase tileBase { get; private set; }
}

public class TileData : MonoBehaviour
{
    public static TileData inst;
    public static Dictionary<Vector3Int, TileInfo> tilesInfo = new Dictionary<Vector3Int, TileInfo>();
    public int nextID = 0;

    public List<TileBase> forestTiles = new List<TileBase>();
    public List<TileBase> plainTiles = new List<TileBase>();
    public List<TileBase> mountainTiles = new List<TileBase>();
    public List<TileBase> swampTiles = new List<TileBase>();
    public List<TileBase> grasslandTiles = new List<TileBase>();
    public List<TileBase> wastelandTiles = new List<TileBase>();
    public List<TileBase> mineralDepositTiles = new List<TileBase>();

    public static Dictionary<TileType, List<TileBase>> tileListsByType;

    public static Dictionary<int, TileType> tileTypes = new Dictionary<int, TileType>();

    public static int maxXandY = 70;

    private void Awake()
    {
        inst = this;
        MakeTileDefinitions();
    }

    public void MakeTileDefinitions()
    {
        nextID = 0;
        tileListsByType = new Dictionary<TileType, List<TileBase>>() {
            { TileType.Plain, plainTiles },
            { TileType.Forest, forestTiles },
            { TileType.Grassland, grasslandTiles },
            { TileType.MineralDeposit, mineralDepositTiles },
            { TileType.Mountain, mountainTiles },
            { TileType.Swamp, swampTiles },
            { TileType.Wasteland, wastelandTiles },
        };

        tileTypes.Clear();
        foreach(var list in tileListsByType) {
            foreach(var tile in list.Value) {
                string name = tile.name.Substring(tile.name.Length - 2);
                int.TryParse(name, out int num);

                tileTypes.Add(num, list.Key);
            }
        }

        foreach(var e in tileTypes) {
            tileTypes.TryGetValue(e.Key, out TileType result);
        }
    }
}

