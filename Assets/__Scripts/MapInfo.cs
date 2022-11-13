using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;


public class MapInfo : MonoBehaviour
{
    static public int W { get; private set; }

    static public int H { get; private set; }
    static public int[,] MAP { get; private set; }
    static public Vector3 OFFSET = new Vector3(0.5f, 0.5f, 0); // c

    static public string COLLISIONS { get; private set; }
    static public string GRAP_TILES { get; private set; }

    [Header("Inscribed")]
    public TextAsset delverLevel;
    public TextAsset delverCollisions;
    public TextAsset delverGrapTiles;
    void Start()
    {
        LoadMap();
        // Loading the COLLISIONS information is simplerthan a whole map
        COLLISIONS = Utils.RemoveLineEndings(delverCollisions.text); // b
        Debug.Log("COLLISIONS contains " + COLLISIONS.Length + " chars");
        GRAP_TILES = Utils.RemoveLineEndings(delverGrapTiles.text); // b
        Debug.Log("GRAP_TILES contains " + GRAP_TILES.Length + " chars");
    }

    /// <summary>
    /// Load map data from the delverLevel TextAsset (e.g.,DelverLevel_Eagle)
    /// </summary>
    void LoadMap()
    {
        // Read in the map data as an array of lines
        string[] lines = delverLevel.text.Split('\n');
        // d
        H = lines.Length;
        string[] tileNums = lines[0].Trim().Split(' ');// Aspace between ’ ’ // e
        W = tileNums.Length;

        // Place the map data into a 2D Array for very fastaccess
        MAP = new int[W, H]; // Generate a 2Darray of the right size 
        for (int j = 0; j < H; j++
    )
        { // Iterate over every line in lines
            tileNums = lines[j].Trim().Split(' '); // Aspace between ’ ’ // f
            for (int i = 0; i < W; i++)
            { // Iterate overevery tileNum string
                if (tileNums[i] == "..")
                {

                    MAP[i, j] = 0;
                }
                else
                {
                    MAP[i, j] = int.Parse(tileNums[i],
                    NumberStyles.HexNumber);
                }
            }
        }
        TileSwapManager.SWAP_TILES(MAP);
        Debug.Log("Map size: " + W + " wide by " + H + " high");
    }

    /// <summary>
    /// Used by TilemapManager to get the bounds of the map
    /// </summary>
    /// <returns></returns>
    public static BoundsInt GET_MAP_BOUNDS()
    {
        // i
        BoundsInt bounds = new BoundsInt(0, 0, 0, W, H, 1
       );
        return bounds;
    }

    /// <summary>
    /// Returns the tileNum at specific coordinates.
    /// </summary>
    /// <param name="pos">The position to check as aVector2</param>
    /// <returns>The tileNum at that location of theMAP</returns>
    public static int GET_MAP_AT_VECTOR2(Vector2 pos)
    {
        // c
        Vector2Int posInt = Vector2Int.FloorToInt(pos);
        return MAP[posInt.x, posInt.y];
    }

    /// <summary>
    /// Checks whether the tile at pos is unsafe (e.g., lavatiles)
    /// </summary>
    /// <param name="pos">The position to check as aVector2</param>
    /// <returns>True if the tile at that location isunsafe</returns>
    public static bool UNSAFE_TILE_AT_VECTOR2(Vector2 pos)
    { // d
        int tileNum = GET_MAP_AT_VECTOR2(pos);
        return (GRAP_TILES[tileNum] == 'U');
    }

}
