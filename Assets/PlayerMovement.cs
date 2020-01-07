using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D player;
    private Vector3 change;

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
        // place tile with space
        // todo
    }

    void MoveCharacter() 
    {
        player.MovePosition(
            transform.position + change * speed * Time.deltaTime
        );
    }
}
