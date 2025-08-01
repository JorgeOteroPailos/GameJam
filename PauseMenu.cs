using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private AudioStreamPlayer2D musica;
	
	private Button continuarButton;
	private Button salirButton;
	private Button optionsButton;
	private Button menuButton;
	
	private Button muchoSonidoButton;
	private Button sonidoParcialButton;
	private Button pocoSonidoButton;
	private Button mutedButton;
	private Button ajustesButton;
	private Button logrosButton;
	
	private bool flagOptions=false;
	
	private Vector2 position;
	private VBoxContainer contenedor;
	private VBoxContainer contenedor2;
	private VBoxContainer contenedor3;

	public override void _Ready(){
		musica = GetParent().GetNode<AudioStreamPlayer2D>("Musica");
		
		contenedor=GetNode<VBoxContainer>("VBoxContainer");
		contenedor2=GetNode<VBoxContainer>("VBoxContainer2");
		contenedor3=GetNode<VBoxContainer>("VBoxContainer3");
		
		contenedor2.Visible=false;
		contenedor3.Visible=false;
		
		position=contenedor.Position;
		
		continuarButton = GetNode<Button>("VBoxContainer/Continuar");
		optionsButton = GetNode<Button>("VBoxContainer/Options");
		menuButton = GetNode<Button>("VBoxContainer/Menu");
		salirButton = GetNode<Button>("VBoxContainer/Salir");
		
		muchoSonidoButton = GetNode<Button>("VBoxContainer2/muchoSonido");
		sonidoParcialButton = GetNode<Button>("VBoxContainer2/sonidoParcial");
		pocoSonidoButton = GetNode<Button>("VBoxContainer2/pocoSonido");
		mutedButton = GetNode<Button>("VBoxContainer3/muted");
		ajustesButton = GetNode<Button>("VBoxContainer3/ajustes");
		logrosButton = GetNode<Button>("VBoxContainer3/logros");

		continuarButton.Pressed += OnContinuarPressed;
		optionsButton.Pressed += OnOptionsPressed;
		menuButton.Pressed += OnMenuPressed;
		salirButton.Pressed += OnSalirPressed;

		muchoSonidoButton.Pressed += OnMuchoVolumenPressed;
		sonidoParcialButton.Pressed += OnMedioVolumenPressed;
		pocoSonidoButton.Pressed += OnPocoVolumenPressed;
		mutedButton.Pressed += OnMutedPressed;
		ajustesButton.Pressed += OnAjustesPressed;
		logrosButton.Pressed += OnLogrosPressed;
		
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
			contenedor2.Visible=false;
			contenedor3.Visible=false;
			
			flagOptions=false;
		}else{
			var pos = contenedor.GlobalPosition;
			pos.X = position.X - 200;
			contenedor.GlobalPosition = pos;
			
			contenedor2.Visible=true;
			contenedor3.Visible=true;
			
			flagOptions=true;
		}
	}
	
	private void OnMenuPressed(){
		//De momento nada jaja
	}
	
	private void OnSalirPressed(){
		GetTree().Quit();
	}
	
	private void OnMuchoVolumenPressed(){
		musica.VolumeDb=0;
	}
	private void OnMedioVolumenPressed(){
		musica.VolumeDb=-6;
	}
	private void OnPocoVolumenPressed(){
		musica.VolumeDb=-12;
	}
	private void OnMutedPressed(){
		musica.VolumeDb=-80;
	}
	private void OnAjustesPressed(){
		GetTree().Quit();
	}
	private void OnLogrosPressed(){
		GetTree().Quit();
	}
}
