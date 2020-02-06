using Subtegral.DialogueSystem.DataContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogMgnt : MonoBehaviour
{
    public Text nameText;
    public Text DailogText;
    public Animator textbox;

    private DialogueContainer dialogueContainer;
    private Queue<string> sentences;
    private string currentDialogNodeId;
    private List<string> options;
    private List<string> optionTargetNodeIds;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        options = new List<string>();
        optionTargetNodeIds = new List<string>();
    }

    public void StartDialog(DialogueContainer dialogContainer, string npcName)
    {
        nameText.text = npcName;
        textbox.SetBool("isOpen", true);
        sentences.Clear();
        dialogueContainer = dialogContainer;
        NextDialog(dialogContainer.DialogueNodeData[0].NodeGUID);
    }

    public void NextDialog(string nextNodeId) {
        currentDialogNodeId = nextNodeId;
        DialogueNodeData nextDialogNode = dialogueContainer.DialogueNodeData.Find(node => node.NodeGUID == currentDialogNodeId);
        foreach (string sentence in nextDialogNode.DialogueText.Split('|')) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }
    
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0) 
        {
            DisplayNextOptions();
            return;
        }
        string sentence = sentences.Dequeue();
        DailogText.text = sentence;
    }

    public void DisplayNextOptions() {
        options = new List<string>();
        optionTargetNodeIds = new List<string>();
        foreach (NodeLinkData nodeLink in dialogueContainer.NodeLinks) {
            if(nodeLink.BaseNodeGUID == currentDialogNodeId) {
                options.Add(nodeLink.PortName);
                optionTargetNodeIds.Add(nodeLink.TargetNodeGUID);
            }
        }
        if (options.Count < 1) {
            EndDialog();
        }
        // display options
        for (int i = 0; i < options.Count; i++) {
            Debug.Log(options[i]);
        }
        // whatever index is chosen go to next dialog
        // NextDialog(optionTargetNodeIds[index])
        EndDialog(); // temp
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
