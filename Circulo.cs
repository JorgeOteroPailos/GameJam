using Godot;
using System;

public partial class Circulo : RigidBody2D
{
	private Vector2 originalPos;

	public override void _Ready()
	{
		originalPos = GlobalPosition;
	}

	public void OnHitByBullet(Node bullet)
	{
		if (bullet is Bullet bullet1)
		{
			Vector2 retroceso = bullet1.Velocity.Normalized() * 10;

			// Mueve TODO el nodo, no solo el sprite
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(this, "global_position", GlobalPosition + retroceso, 0.1f);
		}
	}
}
