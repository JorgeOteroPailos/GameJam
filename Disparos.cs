using Godot;
using System;

public partial class Bullet : Area2D
{
	public Vector2 Velocity = Vector2.Zero;
	public float Speed = 600f;

	public override void _PhysicsProcess(double delta)
	{
		Position += Velocity * Speed * (float)delta;
	}
}
