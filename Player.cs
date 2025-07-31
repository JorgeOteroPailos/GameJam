using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	private List<TextureRect> heartList = new List<TextureRect>();
	private int health = 3;
	public float Speed = 200f;

	public override void _Ready()
	{
		Node heartsParent = GetNode("barra_vida/HBoxContainer");

		foreach (Node child in heartsParent.GetChildren())
		{
			if (child is TextureRect heart)
			{
				heartList.Add(heart);
			}
		}

		GD.Print($"Total corazones: {heartList.Count}");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsKeyPressed(Key.W))
			velocity.Y -= 1;
		if (Input.IsKeyPressed(Key.S))
			velocity.Y += 1;
		if (Input.IsKeyPressed(Key.A))
			velocity.X -= 1;
		if (Input.IsKeyPressed(Key.D))
			velocity.X += 1;

		Velocity = velocity.Normalized() * Speed;
		MoveAndSlide();
	}
}
