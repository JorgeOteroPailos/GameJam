using Godot;
using System;

public partial class Despertar : Node2D{
	private AnimatedSprite2D personaje;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		personaje=GetNode<AnimatedSprite2D>("personaje");
		personaje.Play("despertar");
		
		personaje.AnimationFinished += () => GetTree().ChangeSceneToFile("res://escenas/visual_novel.tscn");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
	}
}
