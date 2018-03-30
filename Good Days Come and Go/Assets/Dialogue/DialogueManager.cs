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
        foreach (DialogueLine dialogueLine in dialogue.dialogue[dialogueId.ToUpper ()]) {
			yield return DialogueLine (dialogueLine);
        }
    }

    public IEnumerator DialogueLine (DialogueLine dialogueLine) {
        switch (dialogueLine.dialogueLineType) {
            case Dialogue.DialogueLineType.TRIGGER:
				yield return StartCoroutine (DialogueTrigger (dialogueLine));
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
			yield return new WaitForSeconds (1f);
			while (!Input.GetKeyUp (KeyCode.Space)) {
				yield return null;
			}
		}
    }

    public IEnumerator DialogueTrigger (DialogueLine dialogueLine) {
        foreach (Trigger trigger in dialogueLine.triggers) {
			print (dialogueLine.dialogueSpeaker.name + " - TRIGGER: " + trigger.triggerType.ToString ());

			switch (trigger.triggerType) {
				case Dialogue.TriggerType.WAIT:
					yield return new WaitForSeconds (float.Parse(trigger.variables[0]));
					break;
				case Dialogue.TriggerType.ACTIVATE:
					
					break;
				case Dialogue.TriggerType.WALK:
					print ("*" + dialogueLine.dialogueSpeaker.name + " walks to x: " + trigger.variables[0] + " y: " + trigger.variables[1] + " facing: " + trigger.variables[2] + " at speed: " + trigger.variables[3] + "*");
					break;
				case Dialogue.TriggerType.FACE:
					print ("*" + dialogueLine.dialogueSpeaker.name + " turns to: " + trigger.variables[2] + "*");
					break;
				case Dialogue.TriggerType.GOTO:
					ReadDialogue (trigger.variables[0]);
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
