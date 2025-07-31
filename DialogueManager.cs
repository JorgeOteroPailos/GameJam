using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class DialogueManager : Node
{
	[Export] public Label DialogueLabel;
	[Export] public Label NameLabel;
	[Export] public Sprite2D CharacterLeft;
	[Export] public Sprite2D CharacterRight;

	private int currentIndex = 0;
	private bool isTyping = false;
	private string currentText = "";

	private List<DialogueLine> dialogueLines = new List<DialogueLine>
	{
		new DialogueLine("MC", "The mother of diosito, how lot of calor is there in hell", false, true, null, "res://assets/VN/defaultMC.png"),
		new DialogueLine("???", "We don't have AC, you'll have to manage", true, false, "res://assets/VN/default_demon.png", null),
		new DialogueLine("MC", "What is this guy saying, ma dude", false, true, null, "res://assets/VN/sadMC.png"),
		new DialogueLine("???", "CALA NENA! Vai sachar o campo", true, false, "res://assets/VN/angry_demon.png", null),	
		new DialogueLine("MC", "Respeta, bro", false, true, null, "res://assets/VN/angryMC.png"),
		new DialogueLine("???", "Turip", true, false, "res://assets/VN/amused_demon.png", null),	
		new DialogueLine("MC", "ip ip ip", false, true, null, "res://assets/VN/motivatedMC.png"),
	};

	public override void _Ready()
	{
		ShowNextDialogue();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if (isTyping)
			{
				SkipTyping();
			}
			else
			{
				ShowNextDialogue();
			}
		}
	}

	private async void ShowNextDialogue()
	{
		if (currentIndex >= dialogueLines.Count)
		{
			GD.Print("Fin del diálogo.");
			return;
		}

		var line = dialogueLines[currentIndex];
		NameLabel.Text = line.Speaker;
		DialogueLabel.Text = "";
		currentText = line.Text;

		// Cargar texturas de personajes si se han definido
		if (!string.IsNullOrEmpty(line.LeftSpritePath))
			CharacterLeft.Texture = GD.Load<Texture2D>(line.LeftSpritePath);

		if (!string.IsNullOrEmpty(line.RightSpritePath))
			CharacterRight.Texture = GD.Load<Texture2D>(line.RightSpritePath);

		// Ajustar opacidad según quién habla
		CharacterLeft.Modulate = line.LeftActive ? Colors.White : new Color(1, 1, 1, 0.5f);
		CharacterRight.Modulate = line.RightActive ? Colors.White : new Color(1, 1, 1, 0.5f);

		isTyping = true;

		foreach (char c in currentText)
		{
			DialogueLabel.Text += c;
			await ToSignal(GetTree().CreateTimer(0.03f), "timeout");

			if (!isTyping)
				break; // Si se salta el tipeo, se rompe el bucle
		}

		DialogueLabel.Text = currentText;
		isTyping = false;
		currentIndex++;
	}

	private void SkipTyping()
	{
		DialogueLabel.Text = currentText;
		isTyping = false;
		currentIndex++;
	}

	private class DialogueLine
	{
		public string Speaker;
		public string Text;
		public bool LeftActive;
		public bool RightActive;
		public string LeftSpritePath;
		public string RightSpritePath;

		public DialogueLine(string speaker, string text, bool leftActive, bool rightActive,
							string leftSpritePath = null, string rightSpritePath = null)
		{
			Speaker = speaker;
			Text = text;
			LeftActive = leftActive;
			RightActive = rightActive;
			LeftSpritePath = leftSpritePath;
			RightSpritePath = rightSpritePath;
		}
	}
}
