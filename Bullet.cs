using Godot;
using System;

public partial class Bullet : Area2D
{
	private Texture2D nuevaTextura;
	public int color=0;
	
	public Player player;
	public Vector2 Velocity = Vector2.Zero;
	public float Speed = 600f;
	public float maxDistance=400;
	bool flag=false;
	private Vector2 originalPosition;

	public override void _Ready()
	{
		
		switch(color){
			case 0:
				nuevaTextura = GD.Load<Texture2D>("res://assets/gota_red.png");
				break;
			case 1:
				nuevaTextura = GD.Load<Texture2D>("res://assets/gota_orange.png");
				break;
			case 2:
				nuevaTextura = GD.Load<Texture2D>("res://assets/gota_yellow.png");
				break;
			case 3:
				nuevaTextura = GD.Load<Texture2D>("res://assets/gota_green.png");
				break;					
			case 4:
				nuevaTextura = GD.Load<Texture2D>("res://assets/gota_blue.png");
				break;
			default:
				nuevaTextura = GD.Load<Texture2D>("res://assets/gota_purple.png");
				break;
		}
		
		// Obtener el nodo Sprite2D 
		Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");

		// Cambiar la textura
		sprite.Texture = nuevaTextura;
		
		originalPosition=this.Position;
		GD.Print("Bullet creada en: " + Position);
		GD.Print("Velocidad: " + Velocity);
		BodyEntered += OnBodyEntered;
		
	}


	public override void _PhysicsProcess(double delta)
	{
		Position += Velocity * Speed * (float)delta;

		// Destruir si se sale de pantalla (opcional)
		if (!GetViewportRect().HasPoint(GlobalPosition)||this.Position.DistanceTo(this.originalPosition)>maxDistance)
			QueueFree();
	}
	
	private void OnBodyEntered(Node body)
	{
		if (body.HasMethod("OnHitByBullet"))
		{
			body.Call("OnHitByBullet", this, this.player); // Puedes pasar la bala como referencia
		}

		if(flag){
			QueueFree(); // Elimina la bala		
		}
		flag=true;
	}
	
}
