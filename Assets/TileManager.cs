using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{   
    static public int mapSize = 10;
    static public string startingTileType = "sand";
    private GameObject[,] mapTileSprites = new GameObject[mapSize, mapSize];
    // private GameObject[,] mapTileEdgeSprites = new GameObject[mapSize + 2, mapSize + 2];
    private string[,] mapTileTypes = new string[mapSize, mapSize];
    private Dictionary<string, Dictionary<string, Sprite[]>> groundTypes = new Dictionary<string, Dictionary<string, Sprite[]>>();
    private bool mapDidChange = false;

    GameObject CreateTile(Grid grid, int x, int y, string type, string position){
        GameObject tile = new GameObject();
        tile.transform.position = new Vector3(x + (float)0.5, y + (float)0.5, 1);
        tile.transform.localScale = new Vector3((float)6.5, (float)6.5, 0);
        tile.transform.parent = grid.transform;
        SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
        int randSpriteIndex = Random.Range(0, groundTypes[type][position].Length);
        Sprite sprite = groundTypes[type][position][randSpriteIndex];
        renderer.sprite = sprite;
        return tile;
    }

    // Start is called before the first frame update
    void Start(){
        // load sprites into dictionary by type and position
        Sprite[] groundSprites = Resources.LoadAll<Sprite>("Sprites/Ground");
        string[] groundTypeNames = new string[] {"grass", "sand", "dirt"};
        for (int x = 0; x < groundTypeNames.Length; x++){
            groundTypes.Add(groundTypeNames[x], new Dictionary<string, Sprite[]>());
            // edges
            groundTypes[groundTypeNames[x]].Add("edge", new Sprite[8]);
            for(int y = 0; y<= 7; y++){
                groundTypes[groundTypeNames[x]]["edge"][y] = groundSprites[y + (x * 15)];
            }
            // full
            groundTypes[groundTypeNames[x]].Add("full", new Sprite[3]);
            for(int y = 8; y<= 10; y++){
                groundTypes[groundTypeNames[x]]["full"][y - 8] = groundSprites[y + (x * 15)];
            }
            // corners
            groundTypes[groundTypeNames[x]].Add("corner", new Sprite[4]);
            for(int y = 11; y<= 14; y++){
                groundTypes[groundTypeNames[x]]["corner"][y - 11] = groundSprites[y + (x * 15)];
            }
        }


        // intialize initial sprite postions
        Grid grid = GetComponent<Grid>();
        for (int x = 0; x < mapSize; x++){
            for (int y = 0; y < mapSize; y++){
                mapTileSprites[x, y] = CreateTile(grid, x, y, startingTileType, "full");
                mapTileTypes[x, y] = startingTileType;
            }
        }
        mapDidChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        // calculate edges
        if (mapDidChange) {
            mapDidChange = false;
            Grid grid = GetComponent<Grid>();
            for (int x = 0; x < mapSize; x++){
                for (int y = 0; y < mapSize; y++){
                    bool emptyN = false;
                    bool emptyE = false;
                    bool emptyS = false;
                    bool emptyW = false;
                    // check 4 edges for same type
                    if(x <= 0 || mapTileTypes[x - 1 , y] != mapTileTypes[x, y]){
                        GameObject tile = CreateTile(grid, x - 1, y, startingTileType, "edge");
                        tile.transform.Rotate(new Vector3(0, 0, 90));
                        emptyW = true;
                    }
                    if(x >= mapSize - 1 || mapTileTypes[x + 1 , y] != mapTileTypes[x, y]){
                        GameObject tile = CreateTile(grid, x + 1, y, startingTileType, "edge");
                        tile.transform.Rotate(new Vector3(0, 0, -90));
                        emptyE = true;
                    }
                    if(y <= 0 || mapTileTypes[x , y - 1] != mapTileTypes[x, y]){
                        GameObject tile = CreateTile(grid, x, y - 1, startingTileType, "edge");
                        tile.transform.Rotate(new Vector3(0, 0, 180));
                        emptyS = true;
                    }
                    if(y >= mapSize - 1|| mapTileTypes[x, y + 1] != mapTileTypes[x, y]){
                        GameObject tile = CreateTile(grid, x, y + 1, startingTileType, "edge");
                        // tile.transform.Rotate(new Vector3(0, 0, 0));
                        emptyN = true;
                    }
                    if(emptyN && emptyE){
                        GameObject tile = CreateTile(grid, x + 1, y + 1, startingTileType, "corner");
                        tile.transform.Rotate(new Vector3(0, 0, -90));
                    }
                    if(emptyE && emptyS){
                        GameObject tile = CreateTile(grid, x + 1, y - 1, startingTileType, "corner");
                        tile.transform.Rotate(new Vector3(0, 0, 180));
                    }
                    if(emptyS && emptyW){
                        GameObject tile = CreateTile(grid, x - 1, y - 1, startingTileType, "corner");
                        tile.transform.Rotate(new Vector3(0, 0, 90));
                    }
                    if(emptyW && emptyN){
                        GameObject tile = CreateTile(grid, x -1, y + 1, startingTileType, "corner");
                        // tile.transform.Rotate(new Vector3(0, 0, 0));
                    }
                }
            }
        }
    }
}
