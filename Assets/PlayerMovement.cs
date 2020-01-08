using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D player;
    private Vector3 change;
    public TileManager tileManager;

    //private int prevPlayerIntX;
    //private int prevPlayerIntY;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // move character
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        change.y = Input.GetAxisRaw("Vertical");
        int playerIntX = System.Convert.ToInt32(transform.position.x);
        int playerIntY = System.Convert.ToInt32(transform.position.y);
        if (change != Vector3.zero) {
            MoveCharacter();
        }
        // place "dirt" tile with space
        if(Input.GetKey(KeyCode.Space)){
            //if(playerIntX != prevPlayerIntX || playerIntY != prevPlayerIntY){
                // RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.back);
                string currentTileType = tileManager.GetMapTileType(playerIntX, playerIntY);
                if(currentTileType != null){
                    tileManager.SetMapTileType(playerIntX,playerIntY,"grass");
                } else {
                    tileManager.CreateMapTile(playerIntX,playerIntY,"grass", "full");
                }
            //}
        }

        //prevPlayerIntX = System.Convert.ToInt32(transform.position.x);
        //prevPlayerIntY = System.Convert.ToInt32(transform.position.y);
    }

    void MoveCharacter() 
    {
        player.MovePosition(
            transform.position + change * speed * Time.deltaTime
        );
    }
}
