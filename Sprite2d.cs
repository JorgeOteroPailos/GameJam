using Godot;
using System;

public partial class Sprite2d : Node2D
{
	public float Speed = 200f;

	public override void _Process(double delta)
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

		if (velocity != Vector2.Zero)
		{
			Position += velocity.Normalized() * Speed * (float)delta;
			GD.Print($"Position: {Position}");
		}
	}
}
