using Godot;
using System;

public partial class Circulo : RigidBody2D
{
	private Texture2D nuevaTextura;

	private Vector2 originalPos;
	public int color;

	public override void _Ready()
	{
		switch(color){
			case 0:
				nuevaTextura = GD.Load<Texture2D>("res://assets/circulo.png");
				break;
			case 1:
				nuevaTextura = GD.Load<Texture2D>("res://assets/circulo_red.png");
				break;
			default:
				nuevaTextura = GD.Load<Texture2D>("res://assets/circulo_green.png");
				break;
		}

		// Obtener el nodo Sprite2D
		Sprite2D sprite = GetNode<Sprite2D>("Circulo");

		// Cambiar la textura
		sprite.Texture = nuevaTextura;
		
		originalPos = GlobalPosition;
	}

	public void OnHitByBullet(Node bullet, Player player)
	{
		if (bullet is Bullet bullet1)
		{
			if(bullet1.color!=this.color){
			Vector2 retroceso = bullet1.Velocity.Normalized() * 10;

			// Mueve TODO el nodo, no solo el sprite
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(this, "global_position", GlobalPosition + retroceso, 0.1f);
			}else{
				QueueFree();
				
				if(player.color<2) player.color++;
				else player.color=0;
			
			}
		}
	}
}
