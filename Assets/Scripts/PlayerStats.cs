using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    public Text goldUI;

    private int gold;
    private int friendship;

    // Start is called before the first frame update
    void Start()
    {
        gold = 0;
        friendship = 1;
    }

    public int getGold() { return gold; }
    public void setGold(int value) { 
        gold = value;
        goldUI.text = value + " Gold";
    }
    public int getFriendship() { return friendship; }
    public void setFriendship(int value) { friendship = value; }

    // Update is called once per frame
    void Update()
    {
        
    }
}
