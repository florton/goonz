using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public GameObject palmTree;
    public TileManager tileManager;

    private GameObject[,] structuresMap;
    private int mapMaxSize;

    // Start is called before the first frame update
    void Start()
    {
        mapMaxSize = tileManager.getMaxMapSize();
        structuresMap = new GameObject[mapMaxSize, mapMaxSize];
    }

    public void GenerateStartingStructures() {
        for (int x = 0; x < mapMaxSize; x++) {
            for (int y = 0; y < mapMaxSize; y++) {
                string type = tileManager.GetMapTileType(x, y);
                if(type == "grass") {
                    if (UnityEngine.Random.Range(0f, 1f) > 0.98) {
                        GameObject tree = Instantiate(palmTree, new Vector3(x + 0.5f, y + 0.5f, 1), Quaternion.identity);
                        structuresMap[x, y] = tree;
                    }
                }                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
