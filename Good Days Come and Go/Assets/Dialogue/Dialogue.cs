using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Dialogue {
	public enum TriggerType {
		NULL,
		WALK,
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
		NULL,
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

	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";

	public void UpdateDialogue(out Dictionary<string, List<DialogueLine>> newDialogue) {
		newDialogue = new Dictionary<string, List<DialogueLine>> ();

		var lines = Regex.Split (dialogueCSV.text, LINE_SPLIT_RE);
		if (lines.Length <= 1) {
			Debug.LogError ("DialogueCSV did not contain any data.");
			return;
		}

		// Name first line of CSV "header"
		// var header = Regex.Split (lines[0], SPLIT_RE);
		List<DialogueLine> tmpNewDialogueLines = new List<DialogueLine> ();
		string currentDialogueId = "";

		for (int i = 1; i < lines.Length; i++) {
			var values = Regex.Split (lines[i], SPLIT_RE);
			if (values[0].ToUpper () == "ID") {
				currentDialogueId = values[1].ToUpper ();
			} else if (values[0].ToUpper () == "END") {
				newDialogue[currentDialogueId] = tmpNewDialogueLines;
			} else {
				DialogueLine tmpNewDialogueLine = new DialogueLine ((values[0] == "" ? tmpNewDialogueLines[i - 1].dialogueSpeaker : NameToDialogueSpeaker (values[0])));
				switch (values[1].ToUpper ()) {
					case "TRIGGER":
						tmpNewDialogueLine.dialogueLineType = DialogueLineType.TRIGGER;
						bool triggerIsDone = false;
						i++;
						int triggerIndex = 0;
						while (!triggerIsDone) {
							values = Regex.Split (lines[i], SPLIT_RE);
							TriggerType tmpNewTriggerType = (TriggerType)Enum.Parse (typeof (TriggerType), values[0].ToUpper ());

							switch (tmpNewTriggerType) {
								case TriggerType.NULL:
									triggerIsDone = true;
									i--;
									break;
								default:
									tmpNewDialogueLine.triggerTypes.Add (tmpNewTriggerType);
									tmpNewDialogueLine.variables[triggerIndex][0] = values[2];
									tmpNewDialogueLine.variables[triggerIndex][1] = values[3];
									tmpNewDialogueLine.variables[triggerIndex][2] = values[4];
									tmpNewDialogueLine.variables[triggerIndex][3] = values[5];
									break;
							}

							if (i < lines.Length) {
								i++;
							} else {
								triggerIsDone = true;
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

							tmpNewDialogueLine.lettersPerSeconds[textIndex] = (values[2] == "" ? 0f : float.Parse (values[2]));

							if (i >= lines.Length || Regex.Split (lines[i + 1], SPLIT_RE)[0].ToUpper () != tmpNewDialogueLine.dialogueSpeaker.name.ToUpper ()) {
								textIsDone = true;
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

public class DialogueLine {
	public Dialogue.DialogueLineType dialogueLineType;
	public DialogueSpeaker dialogueSpeaker;

	public List<string> texts;
	public List<float> lettersPerSeconds;

	public List<Dialogue.TriggerType> triggerTypes;
	public List<List<string>> variables;

	// Pre-DialogueLine Struct
	public DialogueLine (DialogueSpeaker dialogueSpeaker_) {
		dialogueSpeaker = dialogueSpeaker_;
		dialogueLineType = new Dialogue.DialogueLineType ();
		texts = new List<string> ();
		lettersPerSeconds = new List<float> ();
		triggerTypes = new List<Dialogue.TriggerType>();
		variables = new List<List<string>> ();
	}

	// Normal TextLine Struct
	public DialogueLine (DialogueSpeaker dialogueSpeaker_, List<string> texts_, List<float> lettersPerSeconds_) {
		dialogueSpeaker = dialogueSpeaker_;
		texts = texts_;
		lettersPerSeconds = lettersPerSeconds_;
        dialogueLineType = Dialogue.DialogueLineType.TEXT;
	}
	
	// Trigger TextLine Struct
	public DialogueLine (DialogueSpeaker dialogueSpeaker_, List<Dialogue.TriggerType> triggerTypes_, List<List<string>> variables_) {
		dialogueSpeaker = dialogueSpeaker_;
		triggerTypes = triggerTypes_;
		variables = variables_;
		dialogueLineType = Dialogue.DialogueLineType.TRIGGER;
	}
}

public class DialogueSpeaker {
    public string name;
    public Color color;

    public DialogueSpeaker (string name_, Color color_) {
        name = name_;
        color = color_;
    }
}
