using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float speed;
    public TileManager tileManager;
    public Material spriteMaterial;
    public GameObject buildModeOverlay;

    private string groundTypeSelected;
    private Vector3 playerDirection = Vector3.zero;
    private Vector3 prevPlayerDirection = Vector3.zero;

    private Dictionary<string, Sprite[]> allPlayerSprites = new Dictionary<string, Sprite[]>();
    private Sprite[] currentPlayerSprite;
    private Sprite[] cursorSprite;
    private Sprite tileOverlaySprite;

    private float prevPlayerX;
    private float prevPlayerY;
    private bool buildMode = false;
    private Vector3 change;
    private int buildModeSize = 1; // 0 = 1x1, 1 = 3x3, 2 = 5x5
    private int buildModeSquareSize;
    private GameObject[,] overlayTiles;
    private Vector2Int overlayCenter;

    // Start is called before the first frame update
    void Start(){
        buildModeSquareSize = (buildModeSize * 2) + 1;
        groundTypeSelected = tileManager.GetGroundTypes()[0];
        overlayTiles = new GameObject[buildModeSquareSize, buildModeSquareSize];
        IndexPlayerSprites();
        SetPlayerSprite();
        GenerateOverlaySquare();
        HideOverlayTiles();
    }

    // player sprites
    void IndexPlayerSprites() {
        allPlayerSprites["idle"] = Resources.LoadAll<Sprite>("Sprites/Player/player_idle");
        allPlayerSprites["down"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_down");
        allPlayerSprites["up"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_up");
        allPlayerSprites["left"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_left");
        allPlayerSprites["right"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_right");
        cursorSprite = Resources.LoadAll<Sprite>("Sprites/BuildMode/cursor_white");
        tileOverlaySprite = Resources.Load<Sprite>("Sprites/BuildMode/tile_overlay");
    }
    void SetPlayerSprite() {
        if (playerDirection == prevPlayerDirection) {
            if (currentPlayerSprite != null) {
                return;
            }
            else {
                currentPlayerSprite = allPlayerSprites["idle"];
            }
        }
        else if (playerDirection.x > 0) {
            currentPlayerSprite = allPlayerSprites["right"];
        }
        else if (playerDirection.x < 0) {
            currentPlayerSprite = allPlayerSprites["left"];
        }
        else if (playerDirection.y > 0) {
            currentPlayerSprite = allPlayerSprites["up"];
        }
        else if (playerDirection.y < 0) {
            currentPlayerSprite = allPlayerSprites["down"];
        }
        else {
            currentPlayerSprite = allPlayerSprites["idle"];
        }
    }

    public Sprite[] getCurrentPlayerSprite() {
        if (!buildMode) {
            return currentPlayerSprite;
        }
        else {
            return cursorSprite;
        }
    }

    // build mode
    void ToggleBuildMode() {
        if (buildMode == false) {
            // enter build mode
            float playerX = transform.position.x;
            float playerY = transform.position.y;
            int cursorIntX = (int)System.Math.Floor(playerX);
            int cursorIntY = (int)System.Math.Floor(playerY);
                        prevPlayerX = playerX;
            prevPlayerY = playerY;
            ShowOverlayTiles();
            MoveCursor(playerX, playerY, cursorIntX, cursorIntY);
        }
        else {
            // back to play mode
            transform.position = (new Vector3(prevPlayerX, prevPlayerY, transform.position.z));
            HideOverlayTiles();
        }
        buildMode = !buildMode;
    }
    void AddOverlayTile(int x, int y) {
        GameObject overlayTile = new GameObject();
        overlayTile.name = "tile overlay";
        overlayTile.transform.position = new Vector3(x + (float)0.5, y + (float)0.5);
        overlayTile.transform.parent = buildModeOverlay.transform;
        SpriteRenderer renderer = overlayTile.AddComponent<SpriteRenderer>();
        renderer.material = spriteMaterial;
        renderer.sprite = tileOverlaySprite;
        renderer.sortingOrder = 100;
        renderer.color = new Color(0, 255, 14, (float) 0.5);
        overlayTiles[x, y] = overlayTile;
    }
    void GenerateOverlaySquare() {
        for (int x = 0; x < buildModeSquareSize; x++) {
            for (int y = 0; y < buildModeSquareSize; y++) {
                AddOverlayTile(x, y);
            }
        }
        overlayCenter = new Vector2Int(0, 0);
    }
    void ShowOverlayTiles() {
        for (int x = 0; x < buildModeSquareSize; x++) {
            for (int y = 0; y < buildModeSquareSize; y++) {
                overlayTiles[x, y].SetActive(true);
            }
        }
    }
    void HideOverlayTiles() {
        for (int x = 0; x < buildModeSquareSize; x++) {
            for (int y = 0; y < buildModeSquareSize; y++) {
                overlayTiles[x, y].SetActive(false);
            }
        }
    }
    void MoveOverlayTiles(int centerX, int centerY) {
        for (int x = 0; x < buildModeSquareSize; x++) {
            for (int y = 0; y < buildModeSquareSize; y++) {
                overlayTiles[x, y].transform.position = new Vector3(
                    (centerX - buildModeSize) + x + 0.5f,
                    (centerY - buildModeSize) + y + 0.5f,
                    overlayTiles[x, y].transform.position.z
                );
            }
        }
    }
    void MoveCursor(float cursorX, float cursorY, int cursorIntX, int cursorIntY) {
        // get player position & movement info
        // todo: add constrainment to build cursor movement
        if (change != Vector3.zero) {
            transform.position = (new Vector3(cursorX, cursorY, 0) + change * speed  * Time.fixedDeltaTime);
        }
        // update tile overlay
        if (overlayCenter.x != cursorIntX || overlayCenter.y != cursorIntY) {
            MoveOverlayTiles(cursorIntX, cursorIntY);
        }
        overlayCenter = new Vector2Int(cursorIntX, cursorIntY);
    }
    void PlaceTilesFromOverlay(int cursorIntX, int cursorIntY) {
        for (int x = cursorIntX - buildModeSize; x <= cursorIntX + buildModeSize; x++) {
            for (int y = cursorIntY - buildModeSize; y <= cursorIntY + buildModeSize; y++) {
                string currentTileType = tileManager.GetMapTileType(x, y);
                if (currentTileType != null) {
                    tileManager.SetMapTileType(x, y, groundTypeSelected);
                }
                else {
                    tileManager.CreateMapTile(x, y, groundTypeSelected, "full");
                }
            }
        }
    }
    // player movement

    void MovePlayer(float playerX, float playerY){
        // get player position & movement info
        if (change != Vector3.zero) {
            // keep player within map edges
            // and prevent sticking to edges
            float safetyOffset = (float)5;
            float nextX = playerX + (change.x * speed * Time.deltaTime * safetyOffset);
            float nextY = playerY + (change.y * speed * Time.deltaTime * safetyOffset);
            bool canMoveForwardX = tileManager.IsOverTileOrEdgeQuadrant(nextX, playerY);
            bool canMoveForwardY = tileManager.IsOverTileOrEdgeQuadrant(playerX, nextY);

            // move player
            Vector3 movementVector = new Vector3(canMoveForwardX ? change.x : 0, canMoveForwardY ? change.y : 0, 0);
            transform.position = ( new Vector3(playerX , playerY, 0) + movementVector * speed * Time.fixedDeltaTime);
        }
        prevPlayerDirection = playerDirection;
        playerDirection = change;
        SetPlayerSprite();
    }


    // Update is called once per frame
    void Update(){
        change = new Vector3(
            Input.GetKey("right") ? 1 : (Input.GetKey("left") ? -1 : 0),
            Input.GetKey("up") ? 1 : (Input.GetKey("down") ? -1 : 0),
            0
        );
        // toggle build mode
        if (Input.GetKeyDown(KeyCode.B)) {
            ToggleBuildMode();
            return;
        }
        if (buildMode) {
            // build mode cursor controls
            float cursorX = transform.position.x;
            float cursorY = transform.position.y;
            int cursorIntX = (int)System.Math.Floor(cursorX);
            int cursorIntY = (int)System.Math.Floor(cursorY);
            string[] groundTypes = tileManager.GetGroundTypes();
            for (int i = 1; i <= groundTypes.Length; i++) {
                if (Input.GetKeyDown(i.ToString())) {
                    groundTypeSelected = tileManager.GetGroundTypes()[i - 1];
                }
            }
            if (Input.GetKey(KeyCode.Space)) {
                // place selected tile type with space
                PlaceTilesFromOverlay(cursorIntX, cursorIntY);
            }
            MoveCursor(cursorX, cursorY, cursorIntX, cursorIntY);
        }
        else {
            // player controls
            float playerX = transform.position.x;
            float cursorY = transform.position.y;
            MovePlayer(playerX, cursorY);
        }

      //transform.rotation = newQuaternion.Quaternion ;

    }

}
