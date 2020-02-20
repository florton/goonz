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
    public Button button1;
    public Button button2;
    public Button button3;
    public Text optext1;
    public Text optext2;

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
        // Next = Start node
        NextDialog(dialogContainer.NodeLinks.Find(node => node.PortName == "Next").TargetNodeGUID);
    }

    public void showButtons(bool visible) {
        if (options.Count > 0 || !visible) {
            button1.gameObject.SetActive(visible);
        }
        if (options.Count > 1 || !visible) {
            button2.gameObject.SetActive(visible);
        }
        if (options.Count > 2 || !visible) {
            button3.gameObject.SetActive(visible);
        }
    }

    public void NextDialog(string nextNodeId) {
        // hide options
        showButtons(false);        
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
        if (options.Count == 0) {
            EndDialog();
            return;
        }
        // display options
        string optionsString = "";
        for (int i = 0; i < options.Count; i++) {
            optionsString += options[i] + "\n";
        }
        //DailogText.text = optionsString;
        optext1.text = options[0];
        optext2.text = options[1];
        // show options
        showButtons(true);
    }
    // whatever index is chosen go to next dialog
    public void opt1()
    {
        NextDialog(optionTargetNodeIds[0]);
    }
    public void opt2() {
        NextDialog(optionTargetNodeIds[1]);
    }
    public void opt3() {
        NextDialog(optionTargetNodeIds[2]);
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
