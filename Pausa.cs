using Godot;
using System;

public partial class Pausa : Node2D{
	private PauseMenu pauseMenu;
	private AudioStreamPlayer2D musica;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		pauseMenu = GetParent().GetNode<PauseMenu>("PauseMenu");
		musica = GetParent().GetNode<AudioStreamPlayer2D>("Musica");
		
		ProcessMode = Node.ProcessModeEnum.Always;	
		musica.ProcessMode = Node.ProcessModeEnum.Always;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta){
	
		if(Input.IsActionJustPressed("Escape")){
			if (pauseMenu.Visible){
				pauseMenu.HideMenu();
			}else{
				musica.VolumeDb-=6;
				pauseMenu.ShowMenu();
			}
		}
	}
}
