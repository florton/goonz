using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{   
    static public int mapSize = 10;

    private GameObject[,] mapTileSprites = new GameObject[mapSize, mapSize];
    private string[,] mapTileTypes = new string[mapSize, mapSize];
    private Dictionary<string, Dictionary<string, Sprite[]>> groundTypes = new Dictionary<string, Dictionary<string, Sprite[]>>();
    private bool mapDidChange = false;

    GameObject CreateTile(Grid grid, int x, int y, string type, string position){
        GameObject tile = new GameObject();
        tile.transform.position = new Vector3(x + (float)0.5, y + (float)0.5, 1);
        tile.transform.localScale = new Vector3((float)6.25, (float)6.25, 0);
        tile.transform.parent = grid.transform;
        SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
        Sprite sprite = groundTypes[type][position][0];
        renderer.sprite = sprite;
        return tile;
    }

    // Start is called before the first frame update
    void Start(){
        // load sprites into dictionary by type and position
        Sprite[] groundSprites = Resources.LoadAll<Sprite>("Sprites/Ground");
        string[] groundTypeNames = new string[] {"grass", "sand", "dirt", "water"};
        for (int x = 0; x < groundTypeNames.Length; x++){
            groundTypes.Add(groundTypeNames[x], new Dictionary<string, Sprite[]>());
            // edges
            groundTypes[groundTypeNames[x]].Add("edge", new Sprite[8]);
            for(int y = 0; y<= 7; y++){
                groundTypes[groundTypeNames[x]]["edge"][y] = groundSprites[y];
            }
            // full
            groundTypes[groundTypeNames[x]].Add("full", new Sprite[3]);
            for(int y = 8; y<= 10; y++){
                groundTypes[groundTypeNames[x]]["full"][y - 8] = groundSprites[y];
            }
            // corners
            groundTypes[groundTypeNames[x]].Add("corner", new Sprite[4]);
            for(int y = 11; y<= 14; y++){
                groundTypes[groundTypeNames[x]]["corner"][y - 11] = groundSprites[y];
            }
        }


        // intialize initial sprite postions
        Grid grid = GetComponent<Grid>();
        for (int x = 0; x < mapSize; x++){
            for (int y = 0; y < mapSize; y++){
                mapTileSprites[x, y] = CreateTile(grid, x, y, "grass", "full");
                mapTileTypes[x, y] = "grass";
            }
        }
        mapDidChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        // calculate edges
        if (mapDidChange) {
            Grid grid = GetComponent<Grid>();
            for (int x = 0; x < mapSize; x++){
                for (int y = 0; y < mapSize; y++){
                    // check 4 edges for same type
                    if(x <= 0 || mapTileTypes[x - 1 , y] != mapTileTypes[x, y]){
                        GameObject tile = CreateTile(grid, x - 1, y, "grass", "edge");
                        tile.transform.Rotate(new Vector3(0, 0, 90));
                    }
                    if(x >= mapSize - 1 || mapTileTypes[x + 1 , y] != mapTileTypes[x, y]){
                        GameObject tile = CreateTile(grid, x + 1, y, "grass", "edge");
                        tile.transform.Rotate(new Vector3(0, 0, -90));
                    }
                    if(y <= 0 || mapTileTypes[x , y - 1] != mapTileTypes[x, y]){
                        GameObject tile = CreateTile(grid, x, y - 1, "grass", "edge");
                        tile.transform.Rotate(new Vector3(0, 0, 180));
                    }
                    if(y >= mapSize - 1|| mapTileTypes[x, y + 1] != mapTileTypes[x, y]){
                        GameObject tile = CreateTile(grid, x, y + 1, "grass", "edge");
                        // tile.transform.Rotate(new Vector3(0, 0, 0));
                    }
                }
            }
        }
    }
}
