using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D player;
    private Vector3 change;
    public TileManager tileManager;

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
        if (change != Vector3.zero) {
            MoveCharacter();
        }
        // place "dirt" tile with space
        if(Input.GetKeyDown("space")){
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.back);
            GameObject tile = hit.collider.gameObject;
            tileManager.SetMapTileType(
                System.Convert.ToInt32(tile.transform.position.x - 0.5),
                System.Convert.ToInt32(tile.transform.position.y - 0.5),
                "dirt"
            );
        }

        // todo
    }

    void MoveCharacter() 
    {
        player.MovePosition(
            transform.position + change * speed * Time.deltaTime
        );
    }
}
