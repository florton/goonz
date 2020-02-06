using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public bool inZone;
    public bool inTalks;

    public Dialog dialog;
    public Animator anim;
    public DialogMgnt dailogMgnt;

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
            dailogMgnt.StartDialog(dialog);
            inTalks = true; 
            return;
        }

        if (Input.GetKeyDown(KeyCode.E) && inZone && inTalks)
        {
            dailogMgnt.DisplayNextSentences();
            return;
        }

    }
}
