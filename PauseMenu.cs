using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private Button continuarButton;
	private Button salirButton;
	private Button optionsButton;
	private Button menuButton;
	
	private bool flagOptions=false;
	
	private Vector2 position;
	private VBoxContainer contenedor;

	public override void _Ready(){
		contenedor=GetNode<VBoxContainer>("VBoxContainer");
		position=contenedor.Position;
		
		continuarButton = GetNode<Button>("VBoxContainer/Continuar");
		optionsButton = GetNode<Button>("VBoxContainer/Options");
		menuButton = GetNode<Button>("VBoxContainer/Menu");
		salirButton = GetNode<Button>("VBoxContainer/Salir");


		continuarButton.Pressed += OnContinuarPressed;
		optionsButton.Pressed += OnOptionsPressed;
		menuButton.Pressed += OnMenuPressed;
		salirButton.Pressed += OnSalirPressed;


		Visible = false; // Empieza oculto
		ProcessMode = ProcessModeEnum.Always; // Para que reciba input en pausa
	}

	public void ShowMenu()
	{
		Visible = true;
		GetTree().Paused = true;
	}

	public void HideMenu()
	{
		Visible = false;
		GetTree().Paused = false;
	}

	private void OnContinuarPressed(){
		HideMenu();
	}
	
	private void OnOptionsPressed(){
		if(flagOptions){
			contenedor.GlobalPosition=position;
			flagOptions=false;
		}else{
			var pos = contenedor.GlobalPosition;
			pos.X = position.X - 200;
			contenedor.GlobalPosition = pos;
			flagOptions=true;
		}
	}
	
	private void OnMenuPressed(){
		
	}
	
	private void OnSalirPressed(){
		GetTree().Quit();
	}
}
