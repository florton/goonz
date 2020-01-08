using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{   
    public string spriteSheetName;
    public Material spriteMaterial;

    static public int startingMapSize = 10;
    static public int maxSize = 100;
    static public string startingTileType = "sand";
    public string[] groundTypeNames = new string[] {"grass", "sand", "dirt"};
    private GameObject[,] mapTiles = new GameObject[maxSize, maxSize];
    private string[,] mapTileTypes = new string[maxSize, maxSize];
    private GameObject[, ,] mapTileEdges = new GameObject[maxSize, maxSize, 8];
    private string[, ,] mapTileEdgeTypes = new string[maxSize, maxSize, 8];
    private Dictionary<string, Dictionary<string, Sprite[]>> groundTypes = new Dictionary<string, Dictionary<string, Sprite[]>>();

    Sprite GetRandomSpriteOfTypeAndPosition(string type, string position){
        int randSpriteIndex = Random.Range(0, groundTypes[type][position].Length);
        Sprite sprite = groundTypes[type][position][randSpriteIndex];
        return sprite;
    }

    GameObject CreateTile(int x, int y, string type, string position){
        // position either "full", "edge", or "corner"
        GameObject tile = new GameObject();
        tile.transform.position = new Vector3(x + (float)0.5, y + (float)0.5);
        tile.transform.localScale = new Vector3((float)6.4, (float)6.4, 0);
        tile.transform.parent = transform;
        SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
        renderer.material = spriteMaterial;
        renderer.sprite = GetRandomSpriteOfTypeAndPosition(type, position);
        BoxCollider2D boxCollider2D = tile.AddComponent<BoxCollider2D>();
        return tile;
    }

    public void CreateMapTile(int x, int y, string type, string position){
        GameObject tile = CreateTile(x, y, type, position);
        tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
        tile.layer = 18; //map layer
        mapTiles[x, y] = tile;
        mapTileTypes[x, y] = type;
        ClearEdgeTilesAtPosition(x, y);
        GenerateTileEdges(x, y);
    }

    bool AllEdgesAtPositionType(int x, int y, string type){
        bool allEdgesSameType = true;
        bool tileIsNotEmpty = mapTileTypes[x, y] != null;
        for (int z = 0; z < 8; z++){
            string edgeType = mapTileEdgeTypes[x, y, z];
            if(edgeType != null && mapTileEdgeTypes[x, y, z] != type){
                allEdgesSameType = false;
            }
        }
        return allEdgesSameType && tileIsNotEmpty;
    }

    void CreateOrSetEdgeTile(int x, int y, string type, string position, Vector3 rotation){
        // rotation z must be 0, 90, 180, or 270       
        int edgeIndex = System.Convert.ToInt32(rotation.z / 90);
        edgeIndex = position == "corner" ? edgeIndex + 4 : edgeIndex;
        GameObject edgeTile = mapTileEdges[x, y, edgeIndex];
        SpriteRenderer renderer = edgeTile  ? edgeTile.GetComponent<SpriteRenderer>() : null;
        // dont make edges of same type and postion
        if(mapTileEdgeTypes[x, y, edgeIndex] == type){
            return;
        }
        // if all edge tiles are same type as map tile clear edges
        if(AllEdgesAtPositionType(x, y, mapTileTypes[x, y])) {
            ClearEdgeTilesAtPosition(x, y);
            if(mapTileTypes[x, y] == type){
                return;
            }
        }
        // create or set edge
        if (renderer){
            // if edge z already exists
            renderer.sprite = GetRandomSpriteOfTypeAndPosition(type, position);
        } else {
            // new edge z
            edgeTile = CreateTile(x, y, type, position);
            edgeTile.layer = 17; //map edge layer
            edgeTile.transform.Rotate(rotation);
            renderer = edgeTile.GetComponent<SpriteRenderer>();
        } 
        // lower previous edge zs sorting order by one 
        for (int z = 0; z < 8; z++){
            GameObject previousEdgeTile = mapTileEdges[x, y, z];
            if (previousEdgeTile){
                SpriteRenderer prevEdgeRenderer = previousEdgeTile.GetComponent<SpriteRenderer>();
                if (prevEdgeRenderer.sortingOrder > 2){
                    prevEdgeRenderer.sortingOrder = prevEdgeRenderer.sortingOrder - 1;               
                }
            }                
        }
        // set current edge tile sorting later
        renderer.sortingOrder = 10;
        mapTileEdgeTypes[x, y, edgeIndex] = type;
        mapTileEdges[x, y, edgeIndex] = edgeTile;
    }

    void ClearEdgeTilesAtPosition (int x, int y){
        for (int z = 0; z < 8; z++){
            GameObject edgeTile = mapTileEdges[x, y, z];
            if(edgeTile){
                edgeTile.GetComponent<SpriteRenderer>().sprite = null;
                mapTileEdgeTypes[x, y, z] = null;
            }
        }   
    }

    public void SetMapTileType(int x, int y, string type){
        if (mapTileTypes[x, y] != type){
            mapTileTypes[x, y] = type;
            int randSpriteIndex = Random.Range(0, groundTypes[type]["full"].Length);
            mapTiles[x, y].GetComponent<SpriteRenderer>().sprite = groundTypes[type]["full"][randSpriteIndex];
            ClearEdgeTilesAtPosition(x, y);     
            GenerateTileEdges(x, y);
        }else{
            ClearEdgeTilesAtPosition(x, y); 
            GenerateTileEdges(x, y);
        }
    }

    void GenerateTileEdges(int x, int y){
        string currentTileType =  mapTileTypes[x, y];
        CreateOrSetEdgeTile(x - 1, y, currentTileType, "edge", new Vector3(0, 0, 90));
        CreateOrSetEdgeTile(x + 1, y, currentTileType, "edge", new Vector3(0, 0, 270));
        CreateOrSetEdgeTile(x, y - 1, currentTileType, "edge", new Vector3(0, 0, 180));
        CreateOrSetEdgeTile(x, y + 1, currentTileType, "edge", new Vector3(0, 0, 0));
        CreateOrSetEdgeTile(x + 1, y + 1, currentTileType, "corner", new Vector3(0, 0, 270));
        CreateOrSetEdgeTile(x + 1, y - 1, currentTileType, "corner", new Vector3(0, 0, 180));
        CreateOrSetEdgeTile(x - 1, y - 1, currentTileType, "corner", new Vector3(0, 0, 90));
        CreateOrSetEdgeTile(x - 1, y + 1, currentTileType, "corner", new Vector3(0, 0, 0));
    }

    void indexSpriteSheet(){
        // load sprites into dictionary by type and position
        // each ground type has 15 total sprites on the spriteSheet 
        Sprite[] groundSprites = Resources.LoadAll<Sprite>("Sprites/" + spriteSheetName);
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
    }

    // Start is called before the first frame update
    void Start(){
        indexSpriteSheet();

        // intialize initial island map tiles
        int startingCoord = (maxSize / 2);
        for (int x = startingCoord; x < startingCoord + startingMapSize; x++){
            for (int y = startingCoord; y < startingCoord + startingMapSize; y++){
                CreateMapTile(x, y, startingTileType, "full");
            }
        }
    }

    public string GetMapTileType(int x, int y){
        return mapTileTypes[x, y];
    }

    private Vector4 GetEdgeQuadrant(int x, int y, int z){
        // returns {x: xMin, y: xMax, z: yMin, w: yMax] 
        if (!mapTileEdges[x, y, z]){
            return Vector4.zero;
        }
        switch(z){
           // edges
           case 0: return new Vector4(x, x + 1, y, y + (float) 0.5);
           case 1: return new Vector4(x + (float) 0.5, x + 1, y, y + 1);
           case 2: return new Vector4(x, x + 1, y + (float) 0.5, y + 1);
           case 3: return new Vector4(x, x + (float) 0.5, y, y + 1);
           // corners
           case 4: return new Vector4(x + (float) 0.5, x + 1, y, y + (float) 0.5);
           case 5: return new Vector4(x + (float) 0.5, x + 1, y + (float) 0.5, y + 1);
           case 6: return new Vector4(x, x + (float) 0.5, y + (float) 0.5, y + 1);
           case 7: return new Vector4(x, x + (float) 0.5, y, y + (float) 0.5);
           default: return Vector4.zero;
        }
    }

    public bool IsOverTileOrEdgeQuadrant(float x, float y){
        int intX = (int) System.Math.Floor(x);
        int intY = (int) System.Math.Floor(y);
        if (mapTileTypes[intX, intY] != null){
            return true;
        }
        for(int z = 0; z < 8; z++){
            Vector4 quadrant = GetEdgeQuadrant(intX, intY, z);
            if (quadrant == Vector4.zero){
                continue;
            }
            if (x >= quadrant.x && x <= quadrant.y &&
                y >= quadrant.z && y <= quadrant.w
            ){
                return true;
            }

        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
