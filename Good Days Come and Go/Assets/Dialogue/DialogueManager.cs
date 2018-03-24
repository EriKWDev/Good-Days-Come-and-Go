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
			StartCoroutine (ReadDialogue (debugDialogueID));
		}
	}

	public IEnumerator ReadDialogue (string dialogueId) {
		int index = 0;
        foreach (DialogueLine dialogueLine in dialogue.dialogue[dialogueId.ToUpper ()]) {
			yield return DialogueLine (dialogueLine, index);
			index++;
        }
    }

    public IEnumerator DialogueLine (DialogueLine dialogueLine, int index) {
        switch (dialogueLine.dialogueLineType) {
            case Dialogue.DialogueLineType.TRIGGER:
				yield return StartCoroutine (DialogueTrigger (dialogueLine, index));
				break;
            default:
            case Dialogue.DialogueLineType.TEXT:
				yield return StartCoroutine (DialogueText (dialogueLine));
                break;
        }
    }

    public IEnumerator DialogueText (DialogueLine dialogueLine) {
		foreach (string text in dialogueLine.texts) {
			print (dialogueLine.dialogueSpeaker.name + ": " + text);
			yield return new WaitForSeconds (1.5f);
		}
    }

    public IEnumerator DialogueTrigger (DialogueLine dialogueLine, int index) {
        foreach (Dialogue.TriggerType triggerType in dialogueLine.triggerTypes) {
			print (dialogueLine.dialogueSpeaker.name + " - TRIGGER: " + triggerType.ToString ());
			foreach (string variable in dialogueLine.variables[index])
				print (variable);
			switch (triggerType) {
				case Dialogue.TriggerType.WAIT:
					//yield return new WaitForSeconds (float.Parse(dialogueLine.variables[index][0]));
					break;
				case Dialogue.TriggerType.ACTIVATE:
					
					break;
				case Dialogue.TriggerType.GOTO:
					ReadDialogue (dialogueLine.variables[index][0]);
					break;
				default:

					break;
			}
			yield return new WaitForSeconds (0.5f);
		}
    }

	void Update () {
		
	}
}
