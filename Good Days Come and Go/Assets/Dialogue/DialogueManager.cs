using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    [Header ("Dialogue Settings")]
    public TextAsset dialogueCSV;
    Dialogue dialogue;

    [Header("Dialogue Speaker Settings")]
    public float defaultTextSpeed = 4f;

	void Awake () {
        dialogue = new Dialogue (dialogueCSV);   
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
        switch (dialogueLine.triggerType) {
            default:
                break;
        }
    }

	void Update () {
		
	}
}
