using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class DialogueManager : Node
{
	[Export] public Label DialogueLabel;
	[Export] public Label NameLabel;
	[Export] public Sprite2D CharacterLeft;
	[Export] public Sprite2D CharacterRight;
	[Export] public TextureRect TheBackground;
	
	private static int indiceLinea=0; //local para cada escena
	private static int indiceGlobal=0; //global
	private static int[] lineasPorEscena={11, 23, 6, 7, 6, 13, 13};
	//linea en la que cambiamos de fondo en medio del diálogo. Si no lo hacemos, -1
	private static int[] cambioDeFondo={11, -1, 11, 11, 11, -1, 9};
	private static string[] fondoACambiar={"noche_estrellada.png", null, "noche_estrellada.png", "noche_estrellada.png", "noche_estrellada.png", null, "bosque.png"};
	private static string[] fondoInicial={"fuego.png", "noche_estrellada.png", "neoplasticismo.png", "surrealismo.png", "cubismo.png", "final.png", "bosque.png"};
	
	private static string[] siguienteEscena={"despertar", "mundo_1", "mundo_2", "mundo_3", "mundo_4", "mundo_5", "credits"};
	
	protected bool isTyping = false;
	protected string currentText = "";
	
	public override void _Input(InputEvent @event){
		if ((@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed) ||
			(@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Space))
		{
			if (isTyping)
				SkipTyping();
			else
				ShowNextDialogue();
		}
	}

	public override void _Ready() {
		indiceLinea=0;
		var newBackground = ResourceLoader.Load<Texture2D>("res://assets/fondos/"+fondoInicial[Estado.indiceNovela]);
		TheBackground.Texture = newBackground;
		ShowNextDialogue();
	}
	
	private void SkipTyping() {
		DialogueLabel.Text = currentText;
		isTyping = false;
	}
	
	// Método que cada subclase implementará
	private async void ShowNextDialogue() {
		if (indiceLinea >= lineasPorEscena[Estado.indiceNovela]) {
			GD.Print("Fin del diálogo "+Estado.indiceNovela);
			GetTree().ChangeSceneToFile("res://escenas/"+siguienteEscena[Estado.indiceNovela]+".tscn");
			Estado.indiceNovela++;
			return;
		}
		
		// Cambio de fondo en línea q toque
		if (indiceLinea == cambioDeFondo[Estado.indiceNovela]) {
			var newBackground = ResourceLoader.Load<Texture2D>("res://assets/fondos/"+fondoACambiar[Estado.indiceNovela]);
			if (newBackground == null) {
				GD.PrintErr("Error: no se cargó la textura de fondo");
			} else {
				TheBackground.Texture = newBackground;
				GD.Print("Fondo cambiado correctamente en línea "+indiceLinea+ " a "+fondoACambiar[Estado.indiceNovela]);
			}
		}
		
		var linea = CargarLinea();
		
		if(NameLabel==null){
			GD.Print("nameLabel es null");
		}
		
		NameLabel.Text = linea.Speaker;
		DialogueLabel.Text = "";
		currentText = linea.Text;
		
		if (!string.IsNullOrEmpty(linea.LeftSpritePath)) {
			CharacterLeft.Texture = GD.Load<Texture2D>(linea.LeftSpritePath);
			CharacterLeft.Visible = true;
		} else {
			CharacterLeft.Visible = false;
		}

		if (!string.IsNullOrEmpty(linea.RightSpritePath)) {
			CharacterRight.Texture = GD.Load<Texture2D>(linea.RightSpritePath);
			CharacterRight.Visible = true;
		} else {
			CharacterRight.Visible = false;
		}

		CharacterLeft.Modulate = linea.LeftActive ? Colors.White : new Color(1, 1, 1, 0.75f);
		CharacterRight.Modulate = linea.RightActive ? Colors.White : new Color(1, 1, 1, 0.75f);

		isTyping = true;
		foreach (char c in currentText) {
			DialogueLabel.Text += c;
			await ToSignal(GetTree().CreateTimer(0.03f), "timeout");
			if (!isTyping) break;
		}

		DialogueLabel.Text = currentText;
		isTyping = false;
		indiceLinea++;
	}
	
	private DialogueLine CargarLinea()
	{
		// Ruta al archivo según idioma actual
		string path = "res://assets/dialogos/" + Estado.idioma + ".jsonl";

		using (var file = FileAccess.Open(path, FileAccess.ModeFlags.Read))
		{
			int currentLine = 0;

			while (!file.EofReached())
			{
				string line = file.GetLine();

				if (currentLine == indiceGlobal)
				{
					// Parseamos la línea como Dictionary<string, JsonElement>
					var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(line);

					// Extraemos cada campo usando los métodos correctos de JsonElement
					string speaker = dict["speaker"].GetString();
					string text = dict["text"].GetString();
					bool leftActive = dict["leftActive"].GetBoolean();
					bool rightActive = dict["rightActive"].GetBoolean();

					string leftSprite = dict.ContainsKey("leftSprite") && dict["leftSprite"].ValueKind != JsonValueKind.Null
						? dict["leftSprite"].GetString()
						: null;

					string rightSprite = dict.ContainsKey("rightSprite") && dict["rightSprite"].ValueKind != JsonValueKind.Null
						? dict["rightSprite"].GetString()
						: null;

					indiceGlobal++;
					return new DialogueLine(speaker, text, leftActive, rightActive, leftSprite, rightSprite);
				}

				currentLine++;
				
			}
		}

		return null; // Si no se encuentra la línea
	}

	protected class DialogueLine {
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
