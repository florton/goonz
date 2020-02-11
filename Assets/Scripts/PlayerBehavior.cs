using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    public float speed;
    public TileManager tileManager;
    public Material spriteMaterial;

    private Rigidbody2D player;
    private Dictionary<string, Sprite[]> allPlayerSprites = new Dictionary<string, Sprite[]>();
    private Sprite[] currentPlayerSprite;
    private Sprite[] cursorSprite;
    private BuildModeManager buildModeManager;

    private Vector3 change;
    private Vector3 playerDirection = Vector3.zero;
    private Vector3 prevPlayerDirection = Vector3.zero;
    private string selectedGroundType;
    private bool buildMode = false;
    private float hiddenPlayerX;
    private float hiddenPlayerY;


    // Start is called before the first frame update
    void Start(){
        player = GetComponent<Rigidbody2D>();
        buildModeManager = GetComponent<BuildModeManager>();
        selectedGroundType = tileManager.GetGroundTypes()[0];
        IndexPlayerSprites();
        SetPlayerSprite();
        buildModeManager.GenerateOverlaySquare();
        buildModeManager.HideOverlayTiles();
    }

    // player sprites
    void IndexPlayerSprites() {
        allPlayerSprites["idle"] = Resources.LoadAll<Sprite>("Sprites/Player/player_idle");
        allPlayerSprites["down"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_down");
        allPlayerSprites["up"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_up");
        allPlayerSprites["left"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_left");
        allPlayerSprites["right"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_right");
        cursorSprite = Resources.LoadAll<Sprite>("Sprites/BuildMode/cursor_white");
    }
    void SetPlayerSprite() {
        if (playerDirection == prevPlayerDirection) {
            if (currentPlayerSprite != null) {
                return;
            }
            else {
                currentPlayerSprite = allPlayerSprites["idle"];
            }
        } else if (playerDirection.x > 0) {
            currentPlayerSprite = allPlayerSprites["right"];
        } else if (playerDirection.x < 0) {
            currentPlayerSprite = allPlayerSprites["left"];
        } else if (playerDirection.y > 0) {
            currentPlayerSprite = allPlayerSprites["up"];
        } else if (playerDirection.y < 0) {
            currentPlayerSprite = allPlayerSprites["down"];
        } else {
            currentPlayerSprite = allPlayerSprites["idle"];
        }
    }

    public Sprite[] getCurrentPlayerSprite() {
        if (!buildMode) {
            return currentPlayerSprite;
        } else {
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
            hiddenPlayerX = playerX;
            hiddenPlayerY = playerY;
            buildModeManager.ShowOverlayTiles();
            MoveCursor(playerX, playerY, cursorIntX, cursorIntY);
        } else {
            // back to play mode
            transform.position = (new Vector3(hiddenPlayerX, hiddenPlayerY, transform.position.z));
            buildModeManager.HideOverlayTiles();
        }
        buildMode = !buildMode;
    }

    void MoveCursor(float cursorX, float cursorY, int cursorIntX, int cursorIntY) {
        // get player position & movement info
        // todo: add constrainment to build cursor movement
        if (change != Vector3.zero) {
            player.MovePosition(new Vector3(cursorX, cursorY, 0) + change * speed * 2f * Time.deltaTime);
        }
        // update tile overlay
        buildModeManager.MoveOverlayTiles(cursorIntX, cursorIntY);
    }

    // player movement
    void MovePlayer(float playerX, float playerY){
        // get player position & movement info
        if (change != Vector3.zero) {
            // keep player within map edges
            // and prevent sticking to edges
            float safetyOffset = 2f;
            float nextX = playerX + (change.x * speed * Time.deltaTime * safetyOffset);
            float nextY = playerY + (change.y * speed * Time.deltaTime * safetyOffset);
            bool canMoveForwardX = tileManager.IsOverTileOrEdgeQuadrant(nextX, playerY);
            bool canMoveForwardY = tileManager.IsOverTileOrEdgeQuadrant(playerX, nextY);

            // move player
            Vector3 movementVector = new Vector3(canMoveForwardX ? change.x : 0, canMoveForwardY ? change.y : 0, 0);
            Vector3 newPlayerPosition = new Vector3(playerX, playerY, 0) + movementVector * speed * Time.deltaTime;
            player.MovePosition(newPlayerPosition);
            if ((int)System.Math.Floor(newPlayerPosition.y) != (int)System.Math.Floor(playerY)) {
                player.GetComponentInParent<SpriteRenderer>().sortingOrder = tileManager.getMaxMapSize() - (int)System.Math.Floor(newPlayerPosition.y);
            }
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
                    selectedGroundType = tileManager.GetGroundTypes()[i - 1];
                }
            }
            if (Input.GetKey(KeyCode.Space)) {
                // place selected tile type with space
                buildModeManager.SetMapTilesFromBuildOverlay(cursorIntX, cursorIntY, selectedGroundType);
            }
            MoveCursor(cursorX, cursorY, cursorIntX, cursorIntY);
        }
        else {
            // player controls
            float playerX = transform.position.x;
            float cursorY = transform.position.y;
            MovePlayer(playerX, cursorY);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        SpriteRenderer renderer = col.GetComponentInParent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.65f);
    }

    void OnTriggerExit2D(Collider2D col) {
        SpriteRenderer renderer = col.GetComponentInParent<SpriteRenderer>();
        renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);
    }

}
