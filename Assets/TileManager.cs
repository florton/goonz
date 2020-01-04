using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{   
    // Start is called before the first frame update
    void Start()
    {
        Grid grid = GetComponent<Grid>();
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {

                GameObject tile = new GameObject();
                tile.transform.position = new Vector3(x, y, 0);
                tile.transform.parent = grid.transform;
                SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
                Sprite sprite = Resources.Load("Sprites/test", typeof(Sprite)) as Sprite;
                renderer.sprite = sprite;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
