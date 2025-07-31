using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public float Speed = 200f;

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
