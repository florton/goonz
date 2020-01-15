﻿using System.Collections;
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
    private Sprite[] currentPlayerSprites;

    // Start is called before the first frame update
    void Start(){
        player = GetComponent<Rigidbody2D>();
        groundTypeSelected = tileManager.GetGroundTypes()[0];
        IndexPlayerSprites();
        SetPlayerSprite();
    }

    void IndexPlayerSprites() {
        allPlayerSprites["idle"] = Resources.LoadAll<Sprite>("Sprites/Player/player_idle");
        allPlayerSprites["down"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_down");
        allPlayerSprites["up"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_up");
        allPlayerSprites["left"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_left");
        allPlayerSprites["right"] = Resources.LoadAll<Sprite>("Sprites/Player/player_walk_right");
    }

    void SetPlayerSprite() {
        if (playerDirection == prevPlayerDirection) {
            if (currentPlayerSprites != null) {
                return;
            } else {
                currentPlayerSprites = allPlayerSprites["idle"];
            }
        } else if(playerDirection.x > 0) {
            currentPlayerSprites = allPlayerSprites["right"];
        } else if(playerDirection.x < 0) {
            currentPlayerSprites = allPlayerSprites["left"];
        } else if(playerDirection.y > 0) {
            currentPlayerSprites = allPlayerSprites["up"];
        } else if(playerDirection.y < 0) {
            currentPlayerSprites = allPlayerSprites["down"];
        } else {
            currentPlayerSprites = allPlayerSprites["idle"];
        }
    }

    public Sprite[] getCurrentPlayerSprites() {
        return currentPlayerSprites;
    }

    void MovePlayer(float playerX, float playerY, int playerIntX, int playerIntY){
        // get player position & movement info
        Vector3 change = new Vector3(
            Input.GetAxisRaw("Horizontal"), 
            Input.GetAxisRaw("Vertical")
        );
        if (change != Vector3.zero) {
            // keep player within map edges
            float nextX = playerX + (change.x * speed * Time.deltaTime);
            float nextY = playerY + (change.y * speed * Time.deltaTime);
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
        int playerIntX = (int) System.Math.Floor(playerX);
        int playerIntY = (int) System.Math.Floor(playerY);
        MovePlayer(playerX, playerY, playerIntX, playerIntY);

        string[] groundTypes = tileManager.GetGroundTypes();
        for (int i = 1; i <= groundTypes.Length; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                groundTypeSelected = tileManager.GetGroundTypes()[i];
            }
        }

        // place selected tile type with space
        string currentTileType = tileManager.GetMapTileType(playerIntX, playerIntY);
        if(Input.GetKey(KeyCode.Space)){
            if(currentTileType != null){
                tileManager.SetMapTileType(playerIntX,playerIntY, groundTypeSelected);
            } else {
                tileManager.CreateMapTile(playerIntX,playerIntY, groundTypeSelected , "full");
            }
        }
    }
}