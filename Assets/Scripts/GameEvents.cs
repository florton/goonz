using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{

    public PlayerStats playerStats;
    private Dictionary<string, bool> events;

    // Start is called before the first frame update
    void Start()
    {
        events = new Dictionary<string, bool>();
    }

    public bool eventHasTriggered(string eventString) {
        return events.ContainsKey(eventString) && events[eventString];
    }

    private void saveOneTimeEvent(string eventString) {
        events.Add(eventString, true);
    }

    public bool checkCondition(string condition) {
        switch (condition) {
            case "friendshipIsOver1":
                return playerStats.getFriendship() > 1;
            case "didGainStartingGold":
                return eventHasTriggered("gainStartingGold");
            default:
                Debug.Log("could not find condition " + condition);
                return false;                
        }
    }

    public bool triggerEvent(string eventString) {
        switch (eventString) {
            case "add1Friendship":
                playerStats.setFriendship(playerStats.getFriendship() + 1);
                return true;
            case "subtract1Friendship":
                playerStats.setFriendship(playerStats.getFriendship() - 1);
                return true;
            case "gainStartingGold":
                // one time event
                if (eventHasTriggered(eventString)) { return false; }
                playerStats.setGold(playerStats.getGold() + 50);
                saveOneTimeEvent(eventString);
                return true;
            default:
                Debug.Log("could not find event " + eventString);
                return false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
