using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue {
	public enum TriggerType {
		WALK,
		ACTIVATE,
		GOTO,
		OPTION,
		REMEMBEROPTION,
		RECALLOPTION,
		PLAYSONG,
		PLAYSOUND
	}

	public Dictionary<string, List<DialogueLine>> dialogue;
	public TextAsset dialogueCSV;

	public Dialogue (TextAsset dialogueCSV_) {
		dialogueCSV = dialogueCSV_;
		UpdateDialogue (out dialogue);
	}

	public void UpdateDialogue (out Dictionary<string, List<DialogueLine>> newDialogue) {
		newDialogue = null;
	}
}

public class DialogueLine {
	public enum DialogueLineType {
		TEXT,
		TRIGGER
	}
	public DialogueLineType dialogueLineType;
	public DialogueSpeaker dialogueSpeaker;

	public string text;
	public float lettersPerSecond;

	public Dialogue.TriggerType triggerType;
	public string variable1;
	public string variable2;
	public string variable3;
	public string variable4;
	
	// Normal TextLine Struct
	public DialogueLine (DialogueSpeaker dialogueSpeaker_, string text_, float lettersPerSecond_) {
		dialogueSpeaker = dialogueSpeaker_;
		text = text_;
		lettersPerSecond = lettersPerSecond_;
		dialogueLineType = DialogueLineType.TEXT;
	}
	
	// Trigger TextLine Struct
	public DialogueLine (DialogueSpeaker dialogueSpeaker_, Dialogue.TriggerType triggerType_, string variable1_, string variable2_, string variable3_, string variable4_) {
		dialogueSpeaker = dialogueSpeaker_;
		triggerType = triggerType_;
		variable1 = variable1_;
		variable2 = variable2_;
		variable3 = variable3_;
		variable4 = variable4_;
		dialogueLineType = DialogueLineType.TRIGGER;
	}
}

public class DialogueSpeaker {

}
