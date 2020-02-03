using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
public class TileManager : MonoBehaviour
{
    public Material spriteMaterial;

    static public int startingMapSize = 12;
    static public int maxSize = 100;

    private GameObject[,] mapTiles = new GameObject[maxSize, maxSize];
    private string[,] mapTileTypes = new string[maxSize, maxSize];
    private GameObject[,,] mapTileEdges = new GameObject[maxSize, maxSize, 8];
    private string[,,] mapTileEdgeTypes = new string[maxSize, maxSize, 8];
    private Dictionary<string, Dictionary<string, List<Sprite>>> groundTypes = new Dictionary<string, Dictionary<string, List<Sprite>>>();

    private Vector2 lastSetTileCoords;
    private string lastSetTileType;
    Sprite GetRandomSpriteOfTypeAndPosition(string type, string position){   
        int randSpriteIndex = UnityEngine.Random.Range(0, groundTypes[type][position].Count);
        Sprite sprite = groundTypes[type][position][randSpriteIndex];
        return sprite;
    }

    GameObject CreateTile(int x, int y, string type, string position){
        // position either "full", "edge", or "corner"
        GameObject tile = new GameObject();
        tile.transform.position = new Vector3(x + (float)0.5, y + (float)0.5);
        // tile.transform.localScale = new Vector3((float)6.5, (float)6.5, 1);
        tile.transform.parent = transform;
        SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
        renderer.material = spriteMaterial;
        renderer.sprite = GetRandomSpriteOfTypeAndPosition(type, position);
        // BoxCollider2D boxCollider2D = tile.AddComponent<BoxCollider2D>();
        return tile;
    }

    public string[] GetGroundTypes(){
        string[] keyList = groundTypes.Keys.ToArray();
        return keyList;
    }

    public void CreateMapTile(int x, int y, string type, string position){
        GameObject tile = CreateTile(x, y, type, position);
        tile.name = "map tile";
        tile.GetComponent<SpriteRenderer>().sortingOrder = 1;
        // tile.layer = 18; //map layer
        mapTiles[x, y] = tile;
        mapTileTypes[x, y] = type;
        ClearEdgeTilesAtPosition(x, y);
        GenerateTileEdges(x, y);
    }

    bool AllEdgesSameTypeAsBase(int x, int y, string type){
        bool allEdgesSameType = true;
        bool tileIsNotEmpty = mapTileTypes[x, y] != null;
        for (int z = 0; z < 8; z++){
            string edgeType = mapTileEdgeTypes[x, y, z];
            if (edgeType != null && mapTileEdgeTypes[x, y, z] != type){
                allEdgesSameType = false;
            }
        }
        return allEdgesSameType && tileIsNotEmpty;
    }

    void LowerSortOrderOfAllEdges(int x, int y){
        // lower previous edge zs sorting order by one 
        for (int z = 0; z < 8; z++) {
            GameObject previousEdgeTile = mapTileEdges[x, y, z];
            if (previousEdgeTile) {
                SpriteRenderer prevEdgeRenderer = previousEdgeTile.GetComponent<SpriteRenderer>();
                if (prevEdgeRenderer.sortingOrder > 2) {
                    prevEdgeRenderer.sortingOrder = prevEdgeRenderer.sortingOrder - 1;
                }
            }
        }
    }

    void CreateOrSetEdgeTile(int x, int y, string type, string position, Vector3 rotation){
        // rotation z must be 0, 90, 180, or 270       
        int edgeIndex = System.Convert.ToInt32(rotation.z / 90);
        edgeIndex = position == "corner" ? edgeIndex + 4 : edgeIndex;
        GameObject edgeTile = mapTileEdges[x, y, edgeIndex];
        SpriteRenderer renderer = edgeTile ? edgeTile.GetComponent<SpriteRenderer>() : null;
        // if all edge tiles are same type as map tile under it clear edges
        if (AllEdgesSameTypeAsBase(x, y, mapTileTypes[x, y])){
            ClearEdgeTilesAtPosition(x, y);
            if (mapTileTypes[x, y] == type){
                 return;
            }
        }
        // dont make edges of same type and postion, just bring to front
        if (mapTileEdgeTypes[x, y, edgeIndex] == type){
            LowerSortOrderOfAllEdges(x, y);
            renderer.sortingOrder = 20;
            return;
        }
        // create or set edge
        if (renderer){
            // if edge z already exists
            renderer.sprite = GetRandomSpriteOfTypeAndPosition(type, position);
        }
        else{
            // new edge z
            edgeTile = CreateTile(x, y, type, position);
            edgeTile.name = "tile edge";
            edgeTile.layer = 17; //map edge layer
            edgeTile.transform.Rotate(rotation);
            renderer = edgeTile.GetComponent<SpriteRenderer>();
        }
        LowerSortOrderOfAllEdges(x, y);
        // set new edge tile sorting later
        renderer.sortingOrder = 20;
        mapTileEdgeTypes[x, y, edgeIndex] = type;
        mapTileEdges[x, y, edgeIndex] = edgeTile;
    }

    void ClearEdgeTilesAtPosition(int x, int y){
        for (int z = 0; z < 8; z++){
            GameObject edgeTile = mapTileEdges[x, y, z];
            if (edgeTile){
                edgeTile.GetComponent<SpriteRenderer>().sprite = null;
                mapTileEdgeTypes[x, y, z] = null;
                mapTileEdges[x, y, z] = null;
                // Destroy(edgeTile);
            }
        }
    }

    public void SetMapTileType(int x, int y, string type){
        if(x == lastSetTileCoords.x && y == lastSetTileCoords.y && type == lastSetTileType) {
            return;
        }
        if (mapTileTypes[x, y] != type){
            mapTileTypes[x, y] = type;
            int randSpriteIndex = UnityEngine.Random.Range(0, groundTypes[type]["full"].Count);
            mapTiles[x, y].GetComponent<SpriteRenderer>().sprite = groundTypes[type]["full"][randSpriteIndex];
        }
        ClearEdgeTilesAtPosition(x, y);
        GenerateTileEdges(x, y);
        lastSetTileCoords = new Vector2(x, y);
        lastSetTileType = type;
    }

    void GenerateTileEdges(int x, int y){
        string currentTileType = mapTileTypes[x, y];
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
        Sprite[] groundSprites = Resources.LoadAll<Sprite>("Sprites/Terrain");
        foreach (var sprite in groundSprites){
            // name = "type_position_index"
            string[] spriteNames = sprite.name.Split('_');
            if (!groundTypes.ContainsKey(spriteNames[0])){
                groundTypes.Add(spriteNames[0], new Dictionary<string, List<Sprite>>());
            }
            if (!groundTypes[spriteNames[0]].ContainsKey(spriteNames[1])){
                groundTypes[spriteNames[0]].Add(spriteNames[1], new List<Sprite>());
            }
            groundTypes[spriteNames[0]][spriteNames[1]].Add(sprite);

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
        switch (z){
            // edges
            case 0: return new Vector4(x, x + 1, y, y + (float)0.5);
            case 1: return new Vector4(x + (float)0.5, x + 1, y, y + 1);
            case 2: return new Vector4(x, x + 1, y + (float)0.5, y + 1);
            case 3: return new Vector4(x, x + (float)0.5, y, y + 1);
            // corners
            case 4: return new Vector4(x + (float)0.5, x + 1, y, y + (float)0.5);
            case 5: return new Vector4(x + (float)0.5, x + 1, y + (float)0.5, y + 1);
            case 6: return new Vector4(x, x + (float)0.5, y + (float)0.5, y + 1);
            case 7: return new Vector4(x, x + (float)0.5, y, y + (float)0.5);
            default: return Vector4.zero;
        }
    }

    public bool IsOverTileOrEdgeQuadrant(float x, float y){
        int intX = (int)System.Math.Floor(x);
        int intY = (int)System.Math.Floor(y);
        if (mapTileTypes[intX, intY] != null){
            return true;
        }
        for (int z = 0; z < 8; z++){
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

    void setOrCreateMapTile(int x, int y, string type) {
        if (mapTileTypes[x, y] == null) {
            CreateMapTile(x, y, type, "full");
        }
        else {
            SetMapTileType(x, y, type);
        }
    }

    void createBlockSquare(int x1, int y1, int x2, int y2, string type) {
        for (int x = x1; x < x2; x++) {
            for (int y = y1; y < y2; y++) {
                setOrCreateMapTile(x, y, type);
            }
        }
    }

    void createBlockCircle(int x, int y, int r, string type, float xStretch = 1, float yStretch = 1) {
        const double PI = Mathf.PI;
        double i, angle, x1, y1;

        for (i = 0; i < 360; i += 1) {
            angle = i;
            x1 = r * Mathf.Cos((float)(angle * PI / 180)) * xStretch;
            y1 = r * Mathf.Sin((float)(angle * PI / 180)) * yStretch;
            setOrCreateMapTile((int) Math.Round(x + x1), (int) Math.Round(y + y1), type);
        }
        if (r > 1) {
            createBlockCircle(x, y, r - 1, type, xStretch, yStretch);
        }
    }

    // Start is called before the first frame update
    void Start() {
        indexSpriteSheet();
        // intialize initial island map tiles
        int startingCoord = (maxSize / 2);
        int xyStart = startingCoord;
        int xyEnd = startingCoord + startingMapSize;
        int halfSize = (int)(startingMapSize / 2);
        //createBlockSquare(xyStart, xyStart, xyEnd, xyEnd, "sand");
        //createBlockSquare(xyStart + 1, xyStart + 1, xyEnd - 1, xyEnd - 1, "dirt");
        //createBlockSquare(xyStart + 2, xyStart + 2, xyEnd - 2, xyEnd - 2, "grass");
        Vector2Int circle2Coords = new Vector2Int(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5));
        Vector2 circle2Shape = new Vector2(UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(0.5f, 1.5f));
        Vector2Int circle3Coords = new Vector2Int(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5));
        Vector2 circle3Shape = new Vector2(UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(0.5f, 1.5f));
        Vector2Int circle4Coords = new Vector2Int(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(-5, 5));
        Vector2 circle4Shape = new Vector2(UnityEngine.Random.Range(0.5f, 1.5f), UnityEngine.Random.Range(0.5f, 1.5f));


        createBlockCircle(xyStart + halfSize, xyEnd - halfSize, halfSize + 1, "sand");
        createBlockCircle(xyStart + halfSize + circle2Coords.x, xyEnd - halfSize + circle2Coords.y, halfSize + 1, "sand", circle2Shape.x, circle2Shape.y);
        createBlockCircle(xyStart + halfSize + circle3Coords.x, xyEnd - halfSize + circle3Coords.y, halfSize + 1, "sand", circle3Shape.x, circle3Shape.y);
        createBlockCircle(xyStart + halfSize + circle4Coords.x, xyEnd - halfSize + circle4Coords.y, halfSize + 1, "sand", circle4Shape.x, circle4Shape.y);

        createBlockCircle(xyStart + halfSize, xyEnd - halfSize, halfSize - 1, "dirt");
        createBlockCircle(xyStart + halfSize + circle2Coords.x, xyEnd - halfSize + circle2Coords.y, halfSize - 1, "dirt", circle2Shape.x, circle2Shape.y);
        createBlockCircle(xyStart + halfSize + circle3Coords.x, xyEnd - halfSize + circle3Coords.y, halfSize - 1, "dirt", circle3Shape.x, circle3Shape.y);
        createBlockCircle(xyStart + halfSize + circle4Coords.x, xyEnd - halfSize + circle4Coords.y, halfSize - 1, "dirt", circle4Shape.x, circle4Shape.y);

        createBlockCircle(xyStart + halfSize, xyEnd - halfSize, halfSize - 2, "grass");
        createBlockCircle(xyStart + halfSize + circle2Coords.x, xyEnd - halfSize + circle2Coords.y, halfSize - 2, "grass", circle2Shape.x, circle2Shape.y);
        createBlockCircle(xyStart + halfSize + circle3Coords.x, xyEnd - halfSize + circle3Coords.y, halfSize - 2, "grass", circle3Shape.x, circle3Shape.y);
        createBlockCircle(xyStart + halfSize + circle4Coords.x, xyEnd - halfSize + circle4Coords.y, halfSize - 2, "grass", circle4Shape.x, circle4Shape.y);

    }

    // Update is called once per frame
    void Update(){

    }

}
