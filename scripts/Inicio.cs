using Godot;
using System;

public partial class Inicio : Node
{
	private CanvasLayer _pauseMenu;
	
	private Button paintButton;
	private Button salirButton;
	private Button optionsButton;
	
	private Pausa pausa;
	
	private VBoxContainer contenedor;
	private VBoxContainer contenedor2;
	private VBoxContainer contenedor3;
	
	private bool seve=false;

	public override void _Ready()
	{
		// Obtener referencia al menú de pausa
		_pauseMenu = GetNode<CanvasLayer>("optionsMenu");
		_pauseMenu.Visible = false;
		
		pausa = GetNode<Pausa>("/root/Pausa");
		
		paintButton = GetNode<Button>("paint");
		salirButton = GetNode<Button>("exit");
		optionsButton = GetNode<Button>("opts");
		
		paintButton.Icon = GD.Load<Texture2D>($"res://assets/menus/{Estado.idioma}/boton_pinta.png");
		salirButton.Icon = GD.Load<Texture2D>($"res://assets/menus/{Estado.idioma}/boton_salir.png");
		optionsButton.Icon = GD.Load<Texture2D>($"res://assets/menus/{Estado.idioma}/boton_opciones.png");

		optionsButton.Pressed += OnBotonPausaPressed; // <- ButtonDown es inmediato al click
		paintButton.Pressed += OnBotonPaintPressed; // <- ButtonDown es inmediato al click
		salirButton.Pressed += OnBotonExitPressed; // <- ButtonDown es inmediato al click
		

	}

	private void OnBotonPausaPressed()
	{
		pausa.TogglePause();
	}
	
	private void OnBotonExitPressed()
	{
		GetTree().Quit();
	}
	
	private void OnBotonPaintPressed()
	{
		// Cambiar escena a novelaVisual0
		Estado.nivel=1;
		Estado.indiceNovela=0;
		GetTree().ChangeSceneToFile("res://escenas/visual_novel.tscn");
			return;
		
	}
	
	private void OnScreenOptionSelected(long index){
	switch (index)
	{
		case 1: // 1280x720
			DisplayServer.WindowSetSize(new Vector2I(1280, 720));
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			break;
		case 2: // 1920x1080
			DisplayServer.WindowSetSize(new Vector2I(1920, 1080));
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			break;
		case 3: // Pantalla completa
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
			break;
	}
	}
}
