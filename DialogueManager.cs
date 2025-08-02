using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class DialogueManager : Node {
	[Export] public Label DialogueLabel;
	[Export] public Label NameLabel;
	[Export] public Sprite2D CharacterLeft;
	[Export] public Sprite2D CharacterRight;

	private int currentIndex = 0;
	private bool isTyping = false;
	private string currentText = "";

	private List<DialogueLine> dialogueLines = new List<DialogueLine> {
		new DialogueLine(null, "They say the Devil bows to no one. He rules with pride and defiance, and he needs neither permission nor praise.", false, false, null, null),
		new DialogueLine(null, "For ages, nothing stirred him: no prayer, no curse, no soul worth a second glance.", false, false, null, null),
		new DialogueLine(null, "But eventually, his boredom and curiosity drew him to a mortal unlike any other. She came with no crown, no army, just a simple brush.\nA painter whose strokes flayed deception, leaving only the raw, bleeding truth beneath.", false, false, null, null),
		new DialogueLine(null, "Fascinated, the Devil demanded a portrait. And she delivered.", false, false, null, null),
		new DialogueLine(null, "What stared back at him from the canvas was no majestic demon lord, no master of sin. It was him, unmasked, monstrous, and small.", false, false, null, null),
		new DialogueLine("Devil", "What is the meaning of this?!", true, false, "res://assets/VN/angry_demon.png", null),
		new DialogueLine("Iris", "This is your true self. I am faithful to the strokes of my brush.", false, true, "res://assets/VN/angry_demon.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Devil", "That... That hideous creature cannot be me!", true, false, "res://assets/VN/angry_demon.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Iris", "My apologies. Yet, please understand that my brush only speaks truth.", false, true, "res://assets/VN/angry_demon.png", "res://assets/VN/sadMC.png"),
		new DialogueLine(null, "Ashamed and enraged, he banished the painter to the depths of her own personal Hell.", false, false, "res://assets/VN/angry_demon.png", "res://assets/VN/sadMC.png"),
		new DialogueLine("Devil", "Foolish kid. That will teach her.", true, false, "res://assets/VN/default_demon.png", null),
		
		
		new DialogueLine("Iris", "Where am I?", false, true, null, "res://assets/VN/sadMC.png"),
		new DialogueLine(null, "The artist moved around the strange room. There were no windows, just a long, narrow corridor.", false, false, null, "res://assets/VN/sadMC.png"),
		new DialogueLine(null, "Suddenly, the crackling sound of a small flame catched her attention.", false, false, null, "res://assets/VN/defaultMC.png"),
		new DialogueLine("Iris", "Who goes there?!", false, true, null, "res://assets/VN/angryMC.png"),
		new DialogueLine(null, "Before her eyes, from the fire below, appeared some kind of spirit, no bigger than a basket.", false, false, null, "res://assets/VN/defaultMC.png"),
		new DialogueLine("???", "Just a small flame, ma'am. A fellow condemned soul, just like you.", true, false, "res://assets/VN/surprised_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Kenneth", "The name is Kenneth.", true, false, "res://assets/VN/default_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Iris", "Well, Kenneth, you can call me Iris.", false, true, "res://assets/VN/default_spirit.png", "res://assets/VN/happyMC.png"),
		new DialogueLine("Iris", "Any idea how to get out of here?", false, true, "res://assets/VN/default_spirit.png", "res://assets/VN/motivatedMC.png"),
		
		new DialogueLine("MC", "The mother of diosito, how lot of calor is there in hell", false, true, null, "res://assets/VN/defaultMC.png"),
		new DialogueLine("???", "We don't have AC, you'll have to manage", true, false, "res://assets/VN/default_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("MC", "What is this guy saying, ma dude", false, true, "res://assets/VN/default_spirit.png", "res://assets/VN/sadMC.png"),
		new DialogueLine("Spirit", "I'm a spirit from this realm. CALA NENA! Vai sachar o campo", true, false, "res://assets/VN/disappointed_spirit.png", "res://assets/VN/sadMC.png"),	
		new DialogueLine("MC", "Respeta, bro", false, true, "res://assets/VN/disappointed_spirit.png", "res://assets/VN/angryMC.png"),
		new DialogueLine("Spirit", "Turip", true, false, "res://assets/VN/surprised_spirit.png", "res://assets/VN/angryMC.png"),	
		new DialogueLine("MC", "ip ip ip", false, true, "res://assets/VN/surprised_spirit.png", "res://assets/VN/motivatedMC.png"),
	};

	public override void _Ready() {
		ShowNextDialogue();
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) {
			if (isTyping) {
				SkipTyping();
			}
			else {
				ShowNextDialogue();
			}
		}
	}

	private async void ShowNextDialogue() {
		if (currentIndex >= dialogueLines.Count) {
			GD.Print("Fin del diálogo.");
			return;
		}

		var line = dialogueLines[currentIndex];
		NameLabel.Text = line.Speaker;
		DialogueLabel.Text = "";
		currentText = line.Text;

		if (!string.IsNullOrEmpty(line.LeftSpritePath)) {
			CharacterLeft.Texture = GD.Load<Texture2D>(line.LeftSpritePath);
			CharacterLeft.Visible = true;
		}
		else {
			CharacterLeft.Visible = false;
		}

		if (!string.IsNullOrEmpty(line.RightSpritePath)) {
			CharacterRight.Texture = GD.Load<Texture2D>(line.RightSpritePath);
			CharacterRight.Visible = true;
		}
		else {
			CharacterRight.Visible = false;
		}

		// Ajustar opacidad según quién habla
		CharacterLeft.Modulate = line.LeftActive ? Colors.White : new Color(1, 1, 1, 0.5f);
		CharacterRight.Modulate = line.RightActive ? Colors.White : new Color(1, 1, 1, 0.5f);

		isTyping = true;

		foreach (char c in currentText) {
			DialogueLabel.Text += c;
			await ToSignal(GetTree().CreateTimer(0.03f), "timeout");

			if (!isTyping)
				break; 
		}

		DialogueLabel.Text = currentText;
		isTyping = false;
		currentIndex++;
	}

	private void SkipTyping() {
		DialogueLabel.Text = currentText;
		isTyping = false;
	}

	private class DialogueLine {
		public string Speaker;
		public string Text;
		public bool LeftActive;
		public bool RightActive;
		public string LeftSpritePath;
		public string RightSpritePath;

		public DialogueLine(string speaker, string text, bool leftActive, bool rightActive,
							string leftSpritePath = null, string rightSpritePath = null) {
			Speaker = speaker;
			Text = text;
			LeftActive = leftActive;
			RightActive = rightActive;
			LeftSpritePath = leftSpritePath;
			RightSpritePath = rightSpritePath;
		}
	}
}
