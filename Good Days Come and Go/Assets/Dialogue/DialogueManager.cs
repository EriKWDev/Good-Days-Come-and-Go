using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    [Header ("Dialogue Settings")]
    public TextAsset dialogueCSV;
    Dialogue dialogue;
	public bool debugDialogue = false;
	public string debugDialogueID;

    [Header("Dialogue Speaker Settings")]
    public float defaultTextSpeed = 4f;
	
	void Awake () {
        dialogue = new Dialogue (dialogueCSV);   
	}

	void Start () {
		if (debugDialogue) {
			ReadDialogue (debugDialogueID);
		}
	}

	public void ReadDialogue (string dialogueId) {
        foreach (DialogueLine dialogueLine in dialogue.dialogue[dialogueId]) {
            DialogueLine (dialogueLine);
        }
    }

    public void DialogueLine (DialogueLine dialogueLine) {
        switch (dialogueLine.dialogueLineType) {
            case Dialogue.DialogueLineType.TRIGGER:
                DialogueTrigger (dialogueLine);
                break;
            default:
            case Dialogue.DialogueLineType.TEXT:
                DialogueLine (dialogueLine);
                break;
        }
    }

    public void DialogueText (DialogueLine dialogueLine) {
        
    }

    public void DialogueTrigger (DialogueLine dialogueLine) {
        foreach (Dialogue.TriggerType triggerType in dialogueLine.triggerTypes) {
			switch (triggerType) {
				default:
				case Dialogue.TriggerType.NULL:

					break;
			}
		}
    }

	void Update () {
		
	}
}
