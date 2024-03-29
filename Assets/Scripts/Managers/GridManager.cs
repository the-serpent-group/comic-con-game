using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{

    private Tilemap[] tilemaps;
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    [SerializeField]
    private Vector2Int[] mapSizes; // X = Width ; Y = height ; Should be `n % 2 = 0`


    //Tiers
    public int maxTier = 5;
    public int minTier = 0;
    private int currentTier = 0;

    //Map Generation
    public TileBase[] smartFloorArr;
    public TileBase[] smartRoofArr;
    public TileBase[] smartWallArr;
    public Vector3Int tilemapOrigin;
    public Vector3Int tilemapSize;


    // Start is called before the first frame update
    [ExecuteInEditMode]
    void Start()
    {
        if (maxTier < minTier) throw new Exception("maxTier cannot be less than minTier.");
        if (mapSizes.Length != maxTier) throw new Exception("maxTier is larger than array of mapSizes");
        if (smartFloorArr.Length != maxTier) throw new Exception("smartFloor is larger than array of mapSizes");
        if (smartRoofArr.Length != maxTier) throw new Exception("smartRoof is larger than array of mapSizes");
        if (smartWallArr.Length != maxTier) throw new Exception("smartWall is larger than array of mapSizes");

        findTilemaps();
        tilemapOrigin = floorTilemap.origin;
        tilemapSize = floorTilemap.size;
        regenerateMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void findTilemaps()
    {
        tilemaps = GetComponentsInChildren<Tilemap>();
        for (int i = 0; i < tilemaps.Length; i++)
        {
            Tilemap tilemap = tilemaps[i];
            if (tilemap.name == "Floor") floorTilemap = tilemap;
            if (tilemap.name == "Walls") wallTilemap = tilemap;

        }
    }

    void setTiles(Tilemap target, Vector2Int sCoords, Vector2Int eCoords, TileBase tile)
    {

        BetterBoxFill(target, tile, new Vector3Int(sCoords.x, sCoords.y), new Vector3Int(eCoords.x - 1, eCoords.y - 1));
        //target.origin = tilemapOrigin;
        //target.size = tilemapSize;
        //target.ResizeBounds();
    }

    /// <summary>
    /// General function to regenerate the play map.
    /// The offsets were made via trial and error.
    /// </summary>
    private void regenerateMap()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        Vector2Int topCorner = -new Vector2Int(mapSizes[currentTier].x / 2, -mapSizes[currentTier].y / 2);
        Vector2Int botCorner = new Vector2Int(mapSizes[currentTier].x / 2, -mapSizes[currentTier].y / 2);

        TileBase smartFloor = smartFloorArr[currentTier];
        TileBase smartWall = smartWallArr[currentTier];
        TileBase smartRoof = smartRoofArr[currentTier];

        int wallHeight = 3;
        int wallWidth = 2;

        //Main - Floor
        setTiles(
            floorTilemap,
            new Vector2Int(topCorner.x, topCorner.y - 1),
            new Vector2Int(botCorner.x, botCorner.y - 1),
            smartFloor
        );

        //Top - Roof
        setTiles(
            wallTilemap,
            new Vector2Int(topCorner.x - wallWidth, topCorner.y + wallHeight * 2 - 2),
            new Vector2Int(botCorner.x + wallWidth, topCorner.y + wallHeight),
            smartRoof
        );

        //Top - Wall
        setTiles(
            wallTilemap,
            new Vector2Int(topCorner.x, topCorner.y + wallHeight - 1),
            new Vector2Int(botCorner.x, topCorner.y + 1),
            smartWall
        );

        //Right - Roof
        setTiles(
            wallTilemap,
            new Vector2Int(topCorner.x - wallWidth, topCorner.y + wallHeight),
            new Vector2Int(topCorner.x, botCorner.y + wallHeight),
            smartRoof
        );
        //Right - Wall
        setTiles(
            wallTilemap,
            new Vector2Int(topCorner.x - wallWidth, botCorner.y + wallHeight - 1),
            new Vector2Int(topCorner.x, botCorner.y + 2),
            smartWall
        );
        //Right - Floor
        setTiles(
            floorTilemap,
            new Vector2Int(topCorner.x - wallWidth, botCorner.y),
            new Vector2Int(topCorner.x, botCorner.y - 1),
            smartFloor
        );

        //Left - Roof
        setTiles(
            wallTilemap,
            new Vector2Int(botCorner.x, topCorner.y + wallHeight),
            new Vector2Int(botCorner.x + wallWidth, botCorner.y + wallHeight),
            smartRoof
        );
        //Left - Wall
        setTiles(
            wallTilemap,
            new Vector2Int(botCorner.x, botCorner.y + wallHeight - 1),
            new Vector2Int(botCorner.x + wallWidth, botCorner.y + 2),
            smartWall
        );
        //Left - Floor
        setTiles(
            floorTilemap,
            new Vector2Int(botCorner.x, botCorner.y),
            new Vector2Int(botCorner.x + wallWidth, botCorner.y - 1),
            smartFloor
        );
        //Bot - Roof
        setTiles(
            wallTilemap,
            new Vector2Int(topCorner.x - wallWidth, botCorner.y - 3),
            new Vector2Int(botCorner.x + wallWidth, botCorner.y - wallHeight - 1),
            smartRoof
        );



    }

    public bool upgradeMap()
    {
        if (currentTier < maxTier - 1)
        {
            currentTier++;
            regenerateMap();
            return true;
        }
        return false;
    }

    public bool downgradeMap()
    {
        if (currentTier > maxTier)
        {
            currentTier--;
            regenerateMap();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Yoinked from https://forum.unity.com/threads/tilemap-boxfill-is-horrible.502864/
    /// Fills a rectangle on a tilemap
    /// </summary>
    /// <param name="self">The tilemap to be filled</param>
    /// <param name="tile">The tile to fill the box with</param>
    /// <param name="startPosition">The lower left corner of the box</param>
    /// <param name="endPosition">The upper right corner of the box</param>
    public static void BetterBoxFill(Tilemap map, TileBase tile, Vector3Int start, Vector3Int end)
    {
        //Determine directions on X and Y axis
        var xDir = start.x < end.x ? 1 : -1;
        var yDir = start.y < end.y ? 1 : -1;
        //How many tiles on each axis?
        int xCols = 1 + Mathf.Abs(start.x - end.x);
        int yCols = 1 + Mathf.Abs(start.y - end.y);
        //Start painting
        for (var x = 0; x < xCols; x++)
        {
            for (var y = 0; y < yCols; y++)
            {
                var tilePos = start + new Vector3Int(x * xDir, y * yDir, 0);
                map.SetTile(tilePos, tile);
            }
        }
    }
}
