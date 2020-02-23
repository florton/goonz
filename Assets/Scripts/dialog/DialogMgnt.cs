using Subtegral.DialogueSystem.DataContainers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

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
    public Text optext3;
    public GameEvents gameEvents;

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

    public void showOptions() {
        if (options.Count > 0) {
            optext1.text = options[0];
            button1.gameObject.SetActive(true);
        }
        if (options.Count > 1) {
            optext2.text = options[1];
            button2.gameObject.SetActive(true);
        }
        if (options.Count > 2) {
            optext3.text = options[2];
            button3.gameObject.SetActive(true);
        }
    }

    public void hideOptions() {
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        button3.gameObject.SetActive(false);
    }

    private void ParseDialog(DialogueNodeData nextDialogNode) {
        string nextDialog = nextDialogNode.DialogueText;
        List<string> sentances = new List<string>(nextDialogNode.DialogueText.Split('|'));
        if (nextDialog.Contains("[check~")) {
            string condition = Regex.Matches(nextDialog, @"\[check~([^\)]*)\]")[0].Groups[1].Value;
            MatchCollection dialogPossibilities = Regex.Matches(nextDialog, @"\(([^\)]*)\).*\(([^\)]*)\)");
            if (gameEvents.checkCondition(condition)) {
                sentances = new List<string>(dialogPossibilities[0].Groups[1].Value.Split('|'));
            } else {
                sentances = new List<string>(dialogPossibilities[0].Groups[2].Value.Split('|'));
            }
        }
        foreach (string sentence in sentances) {
            sentences.Enqueue(sentence);
        }
        DisplayNextSentence();
    }

    public void NextDialog(string nextNodeId) {
        hideOptions();        
        currentDialogNodeId = nextNodeId;
        DialogueNodeData nextDialogNode = dialogueContainer.DialogueNodeData.Find(node => node.NodeGUID == currentDialogNodeId);
        ParseDialog(nextDialogNode);
    }
    
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0){
            DisplayNextOptions();
            return;
        }
        string sentence = sentences.Dequeue();
        if (sentence.Contains("[event~")) {
            string eventString = Regex.Matches(sentence, @"\[event~([^\)]*)\]")[0].Groups[1].Value;
            gameEvents.triggerEvent(eventString);
            DisplayNextOptions();
            return;
        }
        else {
            DailogText.text = sentence;
        }
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
        // show options
        showOptions();
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
