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
		// Debug.Log (dialogueCSV.text);
		dialogue = new Dialogue (dialogueCSV);
	}

	void Start () {
		if (debugDialogue) {
			ReadDialogue (debugDialogueID);
		}
	}

	public void ReadDialogue (string dialogueId) {
        foreach (DialogueLine dialogueLine in dialogue.dialogue[dialogueId.ToUpper ()]) {
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
                DialogueText (dialogueLine);
                break;
        }
    }

    public void DialogueText (DialogueLine dialogueLine) {
		foreach (string text in dialogueLine.texts) {
			print (dialogueLine.dialogueSpeaker.name + ": " + text);
		}
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
