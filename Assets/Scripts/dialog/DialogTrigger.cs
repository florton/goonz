using Subtegral.DialogueSystem.DataContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public bool inZone;
    public bool inTalks;

    public DialogueContainer dialogContainer;
    public Animator anim;
    public DialogMgnt dailogMgnt;
    public string npcName;

    void OnTriggerEnter2D(Collider2D col)
    {
        inZone = true;  
        anim.speed = 0f;
    }

    void OnTriggerExit2D(Collider2D col)
    {
        inZone = false;
        inTalks = false;
        anim.speed = 1f;
        dailogMgnt.EndDialog();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inZone && !inTalks ) 
        {
            dailogMgnt.StartDialog(dialogContainer, npcName);
            inTalks = true; 
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && inZone && inTalks)
        {
            dailogMgnt.DisplayNextSentence();
            return;
        }

    }
}
