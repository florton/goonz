﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D player;
    public TileManager tileManager;

    // Start is called before the first frame update
    void Start(){
        player = GetComponent<Rigidbody2D>();
    }

    bool playerWillBeOverLand(float nextX, float nextY){
        bool result = tileManager.IsOverTileOrEdgeQuadrant(nextX, nextY);
        return result;
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
            bool canMoveForwardX = playerWillBeOverLand(nextX, playerY);
            bool canMoveForwardY = playerWillBeOverLand(playerX, nextY);
            // move player
            player.MovePosition(
                new Vector3(playerX , playerY, 0) +
                (new Vector3(canMoveForwardX ? change.x : 0, canMoveForwardY ? change.y : 0, 0) * 
                speed * Time.deltaTime)
            );
        }
    }

    // Update is called once per frame
    void Update(){
        float playerX = transform.position.x;
        float playerY = transform.position.y;
        int playerIntX = (int) System.Math.Floor(playerX);
        int playerIntY = (int) System.Math.Floor(playerY);
        MovePlayer(playerX, playerY, playerIntX, playerIntY);

        // place "dirt" tile with space
        string currentTileType = tileManager.GetMapTileType(playerIntX, playerIntY);
        if(Input.GetKey(KeyCode.Space)){
            // RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.back);
            if(currentTileType != null){
                tileManager.SetMapTileType(playerIntX,playerIntY,"grass");
            } else {
                tileManager.CreateMapTile(playerIntX,playerIntY,"grass", "full");
            }
        }
    }
}
