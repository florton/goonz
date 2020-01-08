using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D player;
    public TileManager tileManager;

    private float edgeTolerance = (float) -0; // must be > -1 
    private float playerYOffset = (float) -0.3;

    // Start is called before the first frame update
    void Start(){
        player = GetComponent<Rigidbody2D>();
    }

    int CalculateNextIntPositionForAxis(float change, float playerPos, float offSet = 0){
        return (int) System.Math.Floor(
            (change > 0 ? change + offSet : change - offSet) + playerPos
        ); 
    }

    bool tileAtPositionExists(int x, int y, bool includeEdge = false){
        bool result = tileManager.GetMapTileType(x, y) != null;
        return includeEdge ? tileManager.IsMapEdgeAtPosition(x,y) || result : result;
    }

    void MovePlayer(float playerX, float playerY, int playerIntX, int playerIntY){
        // get player position & movement info
        Vector3 change = new Vector3(
            Input.GetAxisRaw("Horizontal"), 
            Input.GetAxisRaw("Vertical")
        );
        // keep player within map edges
        int nextIntX = CalculateNextIntPositionForAxis(change.x, playerX, edgeTolerance);
        int nextIntY = CalculateNextIntPositionForAxis(change.y > 0 ? change.y + playerYOffset : change.y, playerY, edgeTolerance);
        bool canMoveForwardX = tileAtPositionExists(nextIntX, playerIntY, true);
        bool canMoveForwardY = tileAtPositionExists(playerIntX, nextIntY, true);
        // move player
        if (change != Vector3.zero) {
            player.MovePosition(
                new Vector3(playerX , playerY, 0) +
                new Vector3(canMoveForwardX ? change.x : 0, canMoveForwardY ? change.y : 0, 0) * 
                speed * Time.deltaTime
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
