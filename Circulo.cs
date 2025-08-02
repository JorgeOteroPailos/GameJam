using Godot;
using System;

public partial class Circulo : RigidBody2D
{
	private Texture2D nuevaTextura;
	
	private const float RETROCESO_BALA=10;
	private const float RETROCESO_CHOQUE=100;
	private const float VELOCIDAD=50;

	private Vector2 originalPos;
	public int color=4;
	
	public Player player;

	public override void _Ready()
	{
		if(player == null){
			GD.PrintErr("No se encontró el nodo Player(clase circulo, ready)!");
		}
		
		switch(color){
			case 0:
				nuevaTextura = GD.Load<Texture2D>("res://assets/circulo_red.png");
				break;
			case 1:
				nuevaTextura = GD.Load<Texture2D>("res://assets/nespri.jpg");		
				break;
			case 2:
				nuevaTextura = GD.Load<Texture2D>("res://assets/nespri.jpg");		
				break;
			case 3:
				nuevaTextura = GD.Load<Texture2D>("res://assets/circulo_green.png");
				break;					
			case 4:
				nuevaTextura = GD.Load<Texture2D>("res://assets/circulo_blue.png");
				break;
			default:
				nuevaTextura = GD.Load<Texture2D>("res://assets/nespri.jpg");		
				
				break;
		}

		// Obtener el nodo Sprite2D
		Sprite2D sprite = GetNode<Sprite2D>("Circulo");

		// Cambiar la textura
		sprite.Texture = nuevaTextura;
		
		originalPos = GlobalPosition;
		
		var area = GetNode<Area2D>("DamageArea");
		area.BodyEntered += OnBodyEntered;
	}
	
	public void inicializar(int nuevoColor){
		color=nuevoColor;
	}

	public void OnHitByBullet(Node bullet, Player player){
		if (bullet is Bullet bullet1)
		{
			if(bullet1.color!=this.color){
			Vector2 retroceso = bullet1.Velocity.Normalized() * RETROCESO_BALA;

			// Mueve TODO el nodo, no solo el sprite
			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(this, "global_position", GlobalPosition + retroceso, 0.1f);
			}else{
				QueueFree();
				
				if(player.color<5) player.color++;
				else player.color=0;
			
			}
		}
	}
	
	private void OnBodyEntered(Node body){
		if (body is Player jugador){
			GD.Print("Jugador tocado por enemigo");
			jugador.takeDamage();
			
			Vector2 retroceso = (GlobalPosition - jugador.GlobalPosition).Normalized() * RETROCESO_CHOQUE;

			Tween tween = GetTree().CreateTween();
			tween.TweenProperty(this, "global_position", GlobalPosition + retroceso, 0.1f);
		}
	}
	
	public override void _PhysicsProcess(double delta){
		// Vector dirección hacia el jugador
		Vector2 direction = (player.Position - Position).Normalized();

		// Mover hacia el jugador
		Position += direction * VELOCIDAD * (float)delta;
		
		// TODO: Meter la animación de los demonios en base a las coordenadas X e Y de Position 
	}
	
	
	
}
