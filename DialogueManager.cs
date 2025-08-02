using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class DialogueManager : Node {
	[Export] public Label DialogueLabel;
	[Export] public Label NameLabel;
	[Export] public Sprite2D CharacterLeft;
	[Export] public Sprite2D CharacterRight;
	[Export] public TextureRect TheBackground;

	private int currentIndex = 0;
	private bool isTyping = false;
	private string currentText = "";

	private List<DialogueLine> dialogueLines = new List<DialogueLine> {
		new DialogueLine(null, "They say the Devil bows to no one. He rules with pride and defiance, and he needs neither permission nor praise.", false, false, null, null),
		new DialogueLine(null, "For ages, nothing stirred him: no prayer, no curse, no soul worth a second glance.", false, false, null, null),
		new DialogueLine(null, "But eventually, his boredom and curiosity drew him to a mortal unlike any other. She came with no crown, no army, just a simple brush. A painter whose strokes flayed deception, leaving only the raw, bleeding truth beneath.", false, false, null, null),
		new DialogueLine(null, "Fascinated, the Devil demanded a portrait. And she delivered.", false, false, null, null),
		new DialogueLine(null, "What stared back at him from the canvas was no majestic demon lord, no master of sin. It was him, unmasked, monstrous, and small.", false, false, null, null),
		new DialogueLine("Devil", "What is the meaning of this?!", true, false, "res://assets/VN/angry_demon.png", null),
		new DialogueLine("Iris", "This is your true self. I am faithful to the strokes of my brush.", false, true, "res://assets/VN/angry_demon.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Devil", "That... That hideous creature cannot be me!", true, false, "res://assets/VN/angry_demon.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Iris", "My apologies. Yet, please understand that my brush only speaks truth.", false, true, "res://assets/VN/angry_demon.png", "res://assets/VN/sadMC.png"),
		new DialogueLine(null, "Ashamed and enraged, he banished the painter to the depths of her own personal Hell.", false, false, "res://assets/VN/angry_demon.png", "res://assets/VN/sadMC.png"),
		new DialogueLine("Devil", "Foolish kid. That will teach her.", true, false, "res://assets/VN/default_demon.png", null),

		new DialogueLine("Iris", "Where am I?", false, true, null, "res://assets/VN/sadMC.png"),
		new DialogueLine(null, "The artist moved around the strange room. There were no windows, just 4 walls and a strange hatchway.", false, false, null, "res://assets/VN/sadMC.png"),
		new DialogueLine(null, "Suddenly, the crackling sound of a small flame catched her attention.", false, false, null, "res://assets/VN/defaultMC.png"),
		new DialogueLine("Iris", "Who goes there?!", false, true, null, "res://assets/VN/angryMC.png"),
		new DialogueLine(null, "Before her eyes, from the fire below, appeared some kind of spirit, not bigger than a cat.", false, false, null, "res://assets/VN/defaultMC.png"),
		new DialogueLine("???", "Just a small flame, ma'am. A fellow condemned soul, just like you.", true, false, "res://assets/VN/surprised_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Kenneth", "The name is Kenneth.", true, false, "res://assets/VN/default_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Iris", "Well, Kenneth, you can call me Iris.", false, true, "res://assets/VN/default_spirit.png", "res://assets/VN/happyMC.png"),
		new DialogueLine("Iris", "Any idea how to get out of here?", false, true, "res://assets/VN/default_spirit.png", "res://assets/VN/motivatedMC.png"),
		new DialogueLine("Kenneth", "For now, it's better to focus on keeping you alive...", true, false, "res://assets/VN/disappointed_spirit.png", "res://assets/VN/happyMC.png"),
		new DialogueLine("Kenneth", "Let me think...", true, false, "res://assets/VN/disappointed_spirit.png", "res://assets/VN/happyMC.png"),
		new DialogueLine("Kenneth", "...", true, false, "res://assets/VN/disappointed_spirit.png", "res://assets/VN/happyMC.png"),
		new DialogueLine("Kenneth", "...", true, false, "res://assets/VN/disappointed_spirit.png", "res://assets/VN/happyMC.png"),
		new DialogueLine("Kenneth", "...", true, false, "res://assets/VN/disappointed_spirit.png", "res://assets/VN/happyMC.png"),
		new DialogueLine("Kenneth", "...", true, false, "res://assets/VN/disappointed_spirit.png", "res://assets/VN/sadMC.png"),
		new DialogueLine("Kenneth", "Oh, I know! Maybe we can do something with that art supplies of yours.", true, false, "res://assets/VN/surprised_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Kenneth", "Look around. There is blue, there is yellow... You could use those pigments on your benefit, you are an artist!", true, false, "res://assets/VN/surprised_spirit.png", "res://assets/VN/motivatedMC.png"),
		new DialogueLine("Iris", "But... how?", false, true, "res://assets/VN/default_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Kenneth", "Just shoot your paint to those demons. If you hit one with the correct color, it will disappear!", true, false, "res://assets/VN/surprised_spirit.png", "res://assets/VN/happyMC.png"),
		new DialogueLine("Kenneth", "Although, you shall be careful. If they touch you, you won’t come out unscathed.", true, false, "res://assets/VN/default_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Kenneth", "If you get rid of them, we'll be able to get to the hatchway.", true, false, "res://assets/VN/default_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Kenneth", "Good luck, Iris!", true, false, "res://assets/VN/surprised_spirit.png", "res://assets/VN/defaultMC.png"),
		new DialogueLine("Iris", "Bring it on!", false, true, null, "res://assets/VN/angryMC.png"),
	};

	public override void _Ready() {
		ShowNextDialogue();
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) {
			if (isTyping) {
				SkipTyping();
			} else {
				ShowNextDialogue();
			}
		}
	}

	private async void ShowNextDialogue() {
		if (currentIndex >= dialogueLines.Count) {
			GD.Print("Fin del diálogo.");
			GetTree().ChangeSceneToFile("res://mundo.tscn");
			return;
		}

		var line = dialogueLines[currentIndex];

		// Cambio de fondo en línea 11
		if (currentIndex == 11) {
			var newBackground = ResourceLoader.Load<Texture2D>("res://assets/Fondo_Noche_Estrellada.png");
			if (newBackground == null) {
				GD.PrintErr("Error: no se cargó la textura de fondo");
			} else {
				TheBackground.Texture = newBackground;
				GD.Print("Fondo cambiado correctamente en línea 11");
			}
		}

		NameLabel.Text = line.Speaker;
		DialogueLabel.Text = "";
		currentText = line.Text;

		if (!string.IsNullOrEmpty(line.LeftSpritePath)) {
			CharacterLeft.Texture = GD.Load<Texture2D>(line.LeftSpritePath);
			CharacterLeft.Visible = true;
		} else {
			CharacterLeft.Visible = false;
		}

		if (!string.IsNullOrEmpty(line.RightSpritePath)) {
			CharacterRight.Texture = GD.Load<Texture2D>(line.RightSpritePath);
			CharacterRight.Visible = true;
		} else {
			CharacterRight.Visible = false;
		}

		CharacterLeft.Modulate = line.LeftActive ? Colors.White : new Color(1, 1, 1, 0.75f);
		CharacterRight.Modulate = line.RightActive ? Colors.White : new Color(1, 1, 1, 0.75f);

		isTyping = true;
		foreach (char c in currentText) {
			DialogueLabel.Text += c;
			await ToSignal(GetTree().CreateTimer(0.03f), "timeout");
			if (!isTyping) break;
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
