using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Dialogue {
	public enum TriggerType {
		WALK,
		FACE,
		ACTIVATE,
		GOTO,
		WAIT,
		OPTION,
		SAVEOPTION,
		LOADOPTION,
		PLAYSONG,
		PLAYSOUND
	}

    public enum DialogueLineType {
        TEXT,
        TRIGGER
    }

	public DialogueSpeaker NameToDialogueSpeaker (string name) {
		// if name does not correlate to a set dialogue speaker the new speaker's name will be in uppercase magenta
		return new DialogueSpeaker (name.ToUpper (), Color.magenta);
	}

	public Dictionary<string, List<DialogueLine>> dialogue;
	public TextAsset dialogueCSV;

	public Dialogue (TextAsset dialogueCSV_) {
		dialogueCSV = dialogueCSV_;
		UpdateDialogue (out dialogue);
	}

	static string SPLIT_RE = @";(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

	public void UpdateDialogue(out Dictionary<string, List<DialogueLine>> newDialogue) {
		newDialogue = new Dictionary<string, List<DialogueLine>> ();
		var lines = Regex.Split (dialogueCSV.text, LINE_SPLIT_RE);

		if (lines[lines.Length - 1] == "")
			Array.Resize (ref lines, lines.Length - 1);

		if (lines.Length <= 1) {
			Debug.LogError ("DialogueCSV did not contain any data.");
			return;
		}

		List<DialogueLine> tmpNewDialogueLines = new List<DialogueLine> ();
		string currentDialogueId = "";
		DialogueSpeaker currentDialogueSpeaker = NameToDialogueSpeaker("DEFAULT");
		for (int i = 0; i < lines.Length; i++) {
			var values = Regex.Split (lines[i], SPLIT_RE);
			
			if (values[0].ToUpper () == "ID") {
				currentDialogueId = values[1].ToUpper ();
			} else if (values[0].ToUpper () == "END") {
				newDialogue[currentDialogueId] = tmpNewDialogueLines;
				tmpNewDialogueLines = new List<DialogueLine> ();
				currentDialogueId = "";
			} else {
				if (values[0] != "") {
					currentDialogueSpeaker = NameToDialogueSpeaker (values[0]);
				}
				DialogueLine tmpNewDialogueLine = new DialogueLine (currentDialogueSpeaker);
				switch (values[1].ToUpper ()) {
					case "TRIGGER":
						tmpNewDialogueLine.dialogueLineType = DialogueLineType.TRIGGER;
						bool triggerIsDone = false;
						i++;
						int triggerIndex = 0;
						while (!triggerIsDone) {
							values = Regex.Split (lines[i], SPLIT_RE);
							if (!Enum.IsDefined (typeof (TriggerType), values[1].ToUpper ())) {
								triggerIsDone = true;
								tmpNewDialogueLines.Add (tmpNewDialogueLine);
								triggerIndex++;
								i--;
								break;
							} else {
								TriggerType tmpNewTriggerType = (TriggerType)Enum.Parse (typeof (TriggerType), values[1].ToUpper ());

								Trigger tmpNewTrigger = new Trigger {
									triggerType = tmpNewTriggerType
								};

								for (int k = 0; k <4; k++) {
									tmpNewTrigger.variables.Add (values[k + 2] == "" ? "NULL" : values[k + 2]);
								}

								tmpNewDialogueLine.triggers.Add (tmpNewTrigger);
							}

							var tmpNewValues = Regex.Split (lines[i + 1], SPLIT_RE);

							if (i >= lines.Length || tmpNewValues[1].ToUpper () == "TRIGGER" || tmpNewValues[0].ToUpper () == "END" || (tmpNewValues[0].ToUpper () != "" && tmpNewValues[0].ToUpper () != currentDialogueSpeaker.name.ToUpper ())) {
								triggerIsDone = true;
								tmpNewDialogueLines.Add (tmpNewDialogueLine);
							} else {
								i++;
							}
							triggerIndex++;
						}
						break;
					default:
						tmpNewDialogueLine.dialogueLineType = DialogueLineType.TEXT;
						int textIndex = 0;
						bool textIsDone = false;
						while (!textIsDone) {
							values = Regex.Split (lines[i], SPLIT_RE);
							tmpNewDialogueLine.texts.Add (values[1]);

							var tmpNewValues = Regex.Split (lines[i + 1], SPLIT_RE);

							if (i >= lines.Length || tmpNewValues[1].ToUpper () == "TRIGGER"  || tmpNewValues[0].ToUpper () == "END" || (tmpNewValues[0].ToUpper () != "" && tmpNewValues[0].ToUpper () != currentDialogueSpeaker.name.ToUpper ())) {
								textIsDone = true;
								tmpNewDialogueLines.Add (tmpNewDialogueLine);
							} else {
								i++;
							}
							textIndex++;
						}
						break;
				}
			}
		}
    }
}

public class Trigger {
	public Dialogue.TriggerType triggerType;
	public List<string> variables = new List<string> ();

	public Trigger () {
		triggerType = new Dialogue.TriggerType ();
		variables = new List<string> ();
	}

	public Trigger (Dialogue.TriggerType _triggerType, List<string> _variables) {
		triggerType = _triggerType;
		variables = _variables;
	}
}

public class DialogueLine {
	public Dialogue.DialogueLineType dialogueLineType;
	public DialogueSpeaker dialogueSpeaker;

	public List<string> texts;
	public List<float> lettersPerSeconds;

	public List<Trigger> triggers;

	// Pre-DialogueLine Struct
	public DialogueLine (DialogueSpeaker dialogueSpeaker_) {
		dialogueSpeaker = dialogueSpeaker_;
		dialogueLineType = new Dialogue.DialogueLineType ();
		texts = new List<string> ();
		lettersPerSeconds = new List<float> ();
		triggers = new List<Trigger> ();
	}

	// Normal TextLine Struct
	public DialogueLine (DialogueSpeaker dialogueSpeaker_, List<string> texts_, List<float> lettersPerSeconds_) {
		dialogueSpeaker = dialogueSpeaker_;
		texts = texts_;
		lettersPerSeconds = lettersPerSeconds_;
        dialogueLineType = Dialogue.DialogueLineType.TEXT;
	}
	
	// Trigger TextLine Struct
	public DialogueLine (DialogueSpeaker dialogueSpeaker_, List<Trigger> triggers_, List<List<string>> variables_) {
		dialogueSpeaker = dialogueSpeaker_;
		triggers = triggers_;
		dialogueLineType = Dialogue.DialogueLineType.TRIGGER;
	}
}

public class DialogueSpeaker {
    public string name;
    public Color color;
	public GameObject gameObjectReference;

	public int x;
	public int y;
	public int direction;

    public DialogueSpeaker (string name_, Color color_) {
        name = name_;
        color = color_;
		x = 0;
		y = 0;
		direction = 2;
    }

	public IEnumerator Walk (int newX, int newY, int newDirection, float speed) {
		x = newX;
		y = newY;
		direction = newDirection;
		yield return new WaitForSeconds (2f/speed);
	}
}
