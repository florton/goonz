using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{   
    public Material spriteMaterial;

    static public int mapSize = 10;
    static public string startingTileType = "sand";
    private GameObject[,] mapTiles = new GameObject[mapSize, mapSize];
    private string[,] mapTileTypes = new string[mapSize, mapSize];
    private GameObject[, ,] mapTileEdges = new GameObject[mapSize + 2, mapSize + 2, 8];
    private Dictionary<string, Dictionary<string, Sprite[]>> groundTypes = new Dictionary<string, Dictionary<string, Sprite[]>>();
    private Grid grid;

    Sprite GetRandomSpriteOfTypeAndPosition(string type, string position){
        int randSpriteIndex = Random.Range(0, groundTypes[type][position].Length);
        Sprite sprite = groundTypes[type][position][randSpriteIndex];
        return sprite;
    }

    GameObject CreateTile(int x, int y, string type, string position){
        GameObject tile = new GameObject();
        tile.transform.position = new Vector3(x + (float)0.5, y + (float)0.5);
        tile.transform.localScale = new Vector3((float)6.5, (float)6.5, 0);
        tile.transform.parent = grid.transform;
        SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
        renderer.material = spriteMaterial;
        renderer.sprite = GetRandomSpriteOfTypeAndPosition(type, position);
        BoxCollider2D boxCollider2D = tile.AddComponent<BoxCollider2D>();
        return tile;
    }

    void CreateMapTile(int x, int y, string type, string position){
        GameObject tile = CreateTile(x, y, type, position);
        tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
        tile.layer = 18; //map layer
        mapTiles[x, y] = tile;
        mapTileTypes[x, y] = type;
        ClearEdgeTilesAtPosition(x, y);
        GenerateTileEdges(x, y);
    }

    void CreateOrSetEdgeTile(int x, int y, string type, string position, Vector3 rotation){
        // rotation z must be 0, 90, 180, or 270       
        int edgeIndex = System.Convert.ToInt32(rotation.z / 90);
        GameObject edgeTile = mapTileEdges[x + 1, y + 1, edgeIndex];
        edgeIndex = position == "corner" ? edgeIndex + 4 : edgeIndex;
        if (!edgeTile){
            edgeTile = CreateTile(x, y, type, position);
            edgeTile.GetComponent<SpriteRenderer>().sortingOrder = 2;
            edgeTile.layer = 17; //map edge layer
            edgeTile.transform.Rotate(rotation);
        } else {
            edgeTile.GetComponent<SpriteRenderer>().sprite = GetRandomSpriteOfTypeAndPosition(type, position);
        }
        mapTileEdges[x + 1, y + 1, edgeIndex] = edgeTile;  
    }

    void ClearEdgeTilesAtPosition (int x, int y){
        for (int z = 0; z < 8; z++){
            GameObject edgeTile = mapTileEdges[x + 1, y + 1, z];
            if(edgeTile){
                edgeTile.GetComponent<SpriteRenderer>().sprite = null;
            }
        }   
    }

    public void SetMapTileType(int x, int y, string type){
        mapTileTypes[x, y] = type;
        int randSpriteIndex = Random.Range(0, groundTypes[type]["full"].Length);
        mapTiles[x, y].GetComponent<SpriteRenderer>().sprite = groundTypes[type]["full"][randSpriteIndex];
        ClearEdgeTilesAtPosition(x, y);     
        GenerateTileEdges(x, y);
    }

    public string GetMapTileType(int x, int y){
        return mapTileTypes[x, y];
    }

    void GenerateTileEdges(int x, int y){
        string currentTileType =  mapTileTypes[x, y];
        bool emptyN = y >= mapSize - 1|| mapTileTypes[x, y + 1] != currentTileType;
        bool emptyE = x >= mapSize - 1 || mapTileTypes[x + 1 , y] != currentTileType;
        bool emptyS = y <= 0 || mapTileTypes[x , y - 1] != currentTileType;
        bool emptyW = x <= 0 || mapTileTypes[x - 1 , y] != currentTileType;
        // check 4 edges for same type
        if(emptyW){
            CreateOrSetEdgeTile(x - 1, y, currentTileType, "edge", new Vector3(0, 0, 90));
        }
        if(emptyE){
            CreateOrSetEdgeTile(x + 1, y, currentTileType, "edge", new Vector3(0, 0, 270));
        }
        if(emptyS){
            CreateOrSetEdgeTile(x, y - 1, currentTileType, "edge", new Vector3(0, 0, 180));
        }
        if(emptyN){
            CreateOrSetEdgeTile(x, y + 1, currentTileType, "edge", Vector3.zero);
        }
        if(emptyN && emptyE){
            CreateOrSetEdgeTile(x + 1, y + 1, currentTileType, "corner", new Vector3(0, 0, 270));
        }
        if(emptyE && emptyS){
            CreateOrSetEdgeTile(x + 1, y - 1, currentTileType, "corner", new Vector3(0, 0, 180));
        }
        if(emptyS && emptyW){
            CreateOrSetEdgeTile(x - 1, y - 1, currentTileType, "corner", new Vector3(0, 0, 90));
        }
        if(emptyW && emptyN){
            CreateOrSetEdgeTile(x - 1, y + 1, currentTileType, "corner", Vector3.zero);
        }
    }

    // Start is called before the first frame update
    void Start(){
        grid = GetComponent<Grid>();

        // load sprites into dictionary by type and position
        // each ground type has 15 total sprites on the spritesheet 
        Sprite[] groundSprites = Resources.LoadAll<Sprite>("Sprites/Ground");
        string[] groundTypeNames = new string[] {"grass", "sand", "dirt"};
        for (int x = 0; x < groundTypeNames.Length; x++){
            groundTypes.Add(groundTypeNames[x], new Dictionary<string, Sprite[]>());
            // edges 0 - 7 on sprite sheet
            groundTypes[groundTypeNames[x]].Add("edge", new Sprite[8]);
            for(int y = 0; y<= 7; y++){
                groundTypes[groundTypeNames[x]]["edge"][y] = groundSprites[y + (x * 15)];
            }
            // full 8 - 10 on sprite sheet
            groundTypes[groundTypeNames[x]].Add("full", new Sprite[3]);
            for(int y = 8; y<= 10; y++){
                groundTypes[groundTypeNames[x]]["full"][y - 8] = groundSprites[y + (x * 15)];
            }
            // corners 11 - 14 on sprite sheet
            groundTypes[groundTypeNames[x]].Add("corner", new Sprite[4]);
            for(int y = 11; y<= 14; y++){
                groundTypes[groundTypeNames[x]]["corner"][y - 11] = groundSprites[y + (x * 15)];
            }
        }

        // intialize initial sprite postions
        for (int x = 0; x < mapSize; x++){
            for (int y = 0; y < mapSize; y++){
                CreateMapTile(x, y, startingTileType, "full");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
