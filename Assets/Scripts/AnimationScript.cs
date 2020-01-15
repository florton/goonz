using UnityEngine;

public class AnimationScript : MonoBehaviour {
    public bool loop;
    public float frameSeconds = 1;
    public PlayerBehavior playerBehavior;

    private SpriteRenderer spr;
    private Sprite[] sprites;
    private int frame = 0;
    private float deltaTime = 0;

    // Use this for initialization
    void Start() {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        sprites = playerBehavior.getCurrentPlayerSprites();
        if (sprites == null) {
            return;
        }
        //Keep track of the time that has passed
        deltaTime += Time.deltaTime;

        /*Loop to allow for multiple sprite frame 
         jumps in a single update call if needed
         Useful if frameSeconds is very small*/
        while (deltaTime >= frameSeconds) {
            deltaTime -= frameSeconds;
            frame++;
            if (loop) {
                frame %= sprites.Length;
            }
        }
        //Max limit
        if (frame >= sprites.Length) {
            frame = sprites.Length - 1;
        }
        //Animate sprite with selected frame
        spr.sprite = sprites[frame];
    }
}
