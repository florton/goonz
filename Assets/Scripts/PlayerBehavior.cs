using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerBehavior : MonoBehaviour
{
    public float speed;
    private Rigidbody2D player;
    public TileManager tileManager;

    private string groundTypeSelected;
    private Vector3 playerDirection = Vector3.zero;
    private Vector3 prevPlayerDirection = Vector3.zero;
    
    private Dictionary<string, Sprite[]> allPlayerSprites = new Dictionary<string, Sprite[]>();
    private Sprite[] currentPlayerSprite;
    private Sprite[] cursorSprite;

    // build mode
    private bool buildMode = false;
    private Vector3 hiddenPlayerPosition;

    // Start is called before the first frame update
    void Start(){
        player = GetComponent<Rigidbody2D>();
        groundTypeSelected = tileManager.GetGroundTypes()[0];
        IndexPlayerSprites();
        SetPlayerSprite();
    }

    void ToggleBuildMode() {
        if(buildMode == false) {
            // enter build mode
            // save player position
            hiddenPlayerPosition = transform.position;
        }
        else {
            // back to play mode
            // put player back in position
            player.MovePosition(hiddenPlayerPosition);
        }
        buildMode = !buildMode;
    }

    void IndexPlayerSprites() {
        allPlayerSprites["idle"] = Resources.LoadAll<Sprite>("Sprites/Player/player_idle");
        allPlayerSprites["down"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_down");
        allPlayerSprites["up"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_up");
        allPlayerSprites["left"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_left");
        allPlayerSprites["right"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_right");
        cursorSprite = Resources.LoadAll<Sprite>("Sprites/Player/cursor_white");
    }

    void SetPlayerSprite() {
        if (playerDirection == prevPlayerDirection) {
            if (currentPlayerSprite != null) {
                return;
            } else {
                currentPlayerSprite = allPlayerSprites["idle"];
            }
        } else if(playerDirection.x > 0) {
            currentPlayerSprite = allPlayerSprites["right"];
        } else if(playerDirection.x < 0) {
            currentPlayerSprite = allPlayerSprites["left"];
        } else if(playerDirection.y > 0) {
            currentPlayerSprite = allPlayerSprites["up"];
        } else if(playerDirection.y < 0) {
            currentPlayerSprite = allPlayerSprites["down"];
        } else {
            currentPlayerSprite = allPlayerSprites["idle"];
        }
    }

    public Sprite[] getCurrentPlayerSprite() {
        if (!buildMode) {
            return currentPlayerSprite;
        }else {
            return cursorSprite;
        }
    }

    void MoveCursor(float playerX, float playerY) {
        // get player position & movement info
        Vector3 change = new Vector3(
            Input.GetKey("right") ? 1 : (Input.GetKey("left") ? -1 : 0),
            Input.GetKey("up") ? 1 : (Input.GetKey("down") ? -1 : 0),
            0
        );
        // todo: add constrainment to build cursor movement
        if (change != Vector3.zero) {
            player.MovePosition(new Vector3(playerX, playerY, 0) + change * speed * Time.fixedDeltaTime);
        }
    }

    void MovePlayer(float playerX, float playerY){
        // get player position & movement info
        Vector3 change = new Vector3(
            Input.GetKey("right") ? 1 : (Input.GetKey("left") ? -1 : 0),
            Input.GetKey("up") ? 1 : (Input.GetKey("down") ? -1 : 0),
            0
        );
        if (change != Vector3.zero) {
            // keep player within map edges
            // and prevent sticking to edges
            float safetyOffset = (float) 5;
            float nextX = playerX + (change.x * speed * Time.deltaTime * safetyOffset);
            float nextY = playerY + (change.y * speed * Time.deltaTime * safetyOffset);
            bool canMoveForwardX = tileManager.IsOverTileOrEdgeQuadrant(nextX, playerY);
            bool canMoveForwardY = tileManager.IsOverTileOrEdgeQuadrant(playerX, nextY);
            // move player
            Vector3 movementVector = new Vector3(canMoveForwardX ? change.x : 0, canMoveForwardY ? change.y : 0, 0);
            player.MovePosition( new Vector3(playerX , playerY, 0) + movementVector * speed * Time.fixedDeltaTime);
        }
        prevPlayerDirection = playerDirection;
        playerDirection = change;
        SetPlayerSprite();
    }

    // Update is called once per frame
    void Update(){
        float playerX = transform.position.x;
        float playerY = transform.position.y;
        int playerIntX = (int)System.Math.Floor(playerX);
        int playerIntY = (int)System.Math.Floor(playerY);

        if (buildMode) {
            // build mode cursor controls
            string[] groundTypes = tileManager.GetGroundTypes();
            for (int i = 1; i <= groundTypes.Length; i++) {
                if (Input.GetKeyDown(i.ToString())) {
                    groundTypeSelected = tileManager.GetGroundTypes()[i - 1];
                }
            }
            if (Input.GetKey(KeyCode.Space)) {
                // place selected tile type with space
                string currentTileType = tileManager.GetMapTileType(playerIntX, playerIntY);
                if (currentTileType != null) {
                    tileManager.SetMapTileType(playerIntX, playerIntY, groundTypeSelected);
                }
                else {
                    tileManager.CreateMapTile(playerIntX, playerIntY, groundTypeSelected, "full");
                }
            }
            MoveCursor(playerX, playerY);
        }
        else {
            // player controls
            MovePlayer(playerX, playerY);
        }
        // toggle build mode
        if (Input.GetKeyDown(KeyCode.B)) {
            ToggleBuildMode();
        }

    }
}

/*
if(Input.GetKey(KeyCode.Space)){
    if(currentTileType != null){
        tileManager.SetMapTileType(playerIntX, playerIntY, groundTypeSelected);
    } else {
        tileManager.CreateMapTile(playerIntX, playerIntY, groundTypeSelected, "full");
    }
}
*/
