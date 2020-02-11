using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeManager : MonoBehaviour
{
    public Material spriteMaterial;
    public GameObject buildModeOverlay;
    public TileManager tileManager;

    private Sprite tileOverlaySprite;
    private static int buildModeSize = 1; // 0 = 1x1, 1 = 3x3, 2 = 5x5
    private static int buildModeSquareSize = (buildModeSize * 2) + 1;
    private GameObject[,] overlayTiles;
    private Vector2Int overlayCenter;

    // Start is called before the first frame update
    void Start() {
        overlayTiles = new GameObject[buildModeSquareSize, buildModeSquareSize];
        tileOverlaySprite = Resources.Load<Sprite>("Sprites/BuildMode/tile_overlay");
    }
    void AddOverlayTile(int x, int y) {
        GameObject overlayTile = new GameObject();
        overlayTile.name = "tile overlay";
        overlayTile.transform.position = new Vector3(x + 0.5f, y + 0.5f);
        overlayTile.transform.parent = buildModeOverlay.transform;
        SpriteRenderer renderer = overlayTile.AddComponent<SpriteRenderer>();
        renderer.material = spriteMaterial;
        renderer.sprite = tileOverlaySprite;
        renderer.sortingOrder = 100;
        renderer.color = new Color(0, 255, 14, 0.5f);
        overlayTiles[x, y] = overlayTile;
    }
    public void GenerateOverlaySquare() {
        for (int x = 0; x < buildModeSquareSize; x++) {
            for (int y = 0; y < buildModeSquareSize; y++) {
                AddOverlayTile(x, y);
            }
        }
        overlayCenter = new Vector2Int(0, 0);
    }
    public void ShowOverlayTiles() {
        for (int x = 0; x < buildModeSquareSize; x++) {
            for (int y = 0; y < buildModeSquareSize; y++) {
                // show overlay
                if(overlayTiles[x, y]) {
                    overlayTiles[x, y].SetActive(true);
                }
            }
        }
    }
    public void HideOverlayTiles() {
        for (int x = 0; x < buildModeSquareSize; x++) {
            for (int y = 0; y < buildModeSquareSize; y++) {
                // hide overlay
                if (overlayTiles[x, y]) {
                    overlayTiles[x, y].SetActive(false);
                }
            }
        }
    }
    void MoveOverlay(int centerX, int centerY) {
        for (int x = 0; x < buildModeSquareSize; x++) {
            for (int y = 0; y < buildModeSquareSize; y++) {
                if (overlayTiles[x, y]) {
                    overlayTiles[x, y].transform.position = new Vector3(
                        (centerX - buildModeSize) + x + 0.5f,
                        (centerY - buildModeSize) + y + 0.5f,
                        overlayTiles[x, y].transform.position.z
                    );
                }
            }
        }
    }
    public void MoveOverlayTiles(int cursorIntX, int cursorIntY) {
        // update tile overlay
        if (overlayCenter.x != cursorIntX || overlayCenter.y != cursorIntY) {
            MoveOverlay(cursorIntX, cursorIntY);
        }
        overlayCenter = new Vector2Int(cursorIntX, cursorIntY);
    }

    public void SetMapTilesFromBuildOverlay(int cursorIntX, int cursorIntY, string type) {
        for (int x = cursorIntX - buildModeSize; x <= cursorIntX + buildModeSize; x++) {
            for (int y = cursorIntY - buildModeSize; y <= cursorIntY + buildModeSize; y++) {
                string currentTileType = tileManager.GetMapTileType(x, y);
                if (currentTileType != null) {
                    tileManager.SetMapTileType(x, y, type);
                }
                else {
                    tileManager.CreateMapTile(x, y, type, "full");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
