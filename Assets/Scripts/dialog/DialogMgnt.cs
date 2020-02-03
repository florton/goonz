using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMgnt : MonoBehaviour
{

    public Text nameText;
    public Text DailogText;
    public Animator textbox;
    public int sent;

    private Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialog(Dialog dialog)
    {
        nameText.text = dialog.name;
        textbox.SetBool("isOpen", true);
        sentences.Clear();
    
        if (dialog.friend == 1)
        {
            foreach (string sentence in dialog.sentences)
            {
                dialog.friend += 1;
                sentences.Enqueue(sentence);
            }
        }
        DisplayNextSentences();
        if (dialog.friend >= 2 || dialog.friend <= 5)
        {
            foreach (string sentence in dialog.sentences2)
            {
                sentences.Enqueue(sentence);
            }
        }
    }
    
    public void DisplayNextSentences()
    {
        if (sentences.Count == 0) 
        {
            EndDialog();
            return;
        }
        string sentence = sentences.Dequeue();
        DailogText.text = sentence;

    }

    public void EndDialog()
    {
        
        textbox.SetBool("isOpen", false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
