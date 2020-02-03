using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public bool inzone;
    public bool inTalks;

    public Dialog dialog;
    public Animator anim;
    public DialogMgnt dailogMgnt;

    void OnTriggerEnter2D(Collider2D col)
    {
        inzone = (true);
   
        anim.speed = 0f;



    }



    void OnTriggerExit2D(Collider2D col)
    {
        inzone = (false);
        inTalks = (false);
        anim.speed = 1f;
        dailogMgnt.EndDialog();


    }

    public void startConvo (Dialog dialog) 
    {
        dailogMgnt.StartDialog(dialog);
       
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inzone &! inTalks ) 
        {
            startConvo(dialog);
            inTalks = (true); 
            return;
            }

        if (Input.GetKeyDown(KeyCode.E) && inzone && inTalks)
        {
            dailogMgnt.DisplayNextSentences();
    
            return;
        }

    }
}
