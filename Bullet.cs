using Godot;
using System;

public partial class Bullet : Area2D
{
	private Texture2D nuevaTextura;
	public int color=0;
	public String default_="";
	public String hit="";
	private AnimatedSprite2D sprite;
	
	public Player player;
	public Vector2 Velocity = Vector2.Zero;
	public float Speed = 600f;
	public float maxDistance=400;
	bool flag=false;
	private Vector2 originalPosition;

	public override void _Ready(){
		// 0: Rojo-Red
		// 1: Naranja-Orange
		// 2: Amarillo-Yellow
		// 3: Verde-Green
		// 4: Azul-Blue
		// 5: Morado-Purple
	
		switch(color){
			case 0:
				default_="default_red";
				hit="hit_red";
				break;
			case 1:
				default_="default_orange";
				hit="hit_orange";
				break;
			case 2:
				default_="default_yellow";
				hit="hit_yellow";
				break;
			case 3:
				default_="default_green";
				hit="hit_green";
				break;					
			case 4:
				default_="default_blue";
				hit="hit_blue";
				break;
			default:
				default_="default_purple";
				hit="hit_purple";
				break;
		}
		
		// Obtener el nodo Sprite2D 
		sprite = GetNode<AnimatedSprite2D>("Sprite2D");

		// Cambiar la textura
		//sprite.Texture = nuevaTextura;
		
		originalPosition=this.Position;
		GD.Print("Bullet creada en: " + Position);
		GD.Print("Velocidad: " + Velocity);
		BodyEntered += OnBodyEntered;
		
	}


	public override void _PhysicsProcess(double delta){
		Position += Velocity * Speed * (float)delta;
		sprite.Rotation = MathF.Atan2(-Velocity.Y, -Velocity.X);

		if (sprite.Animation != hit) // solo reproducir default si no está en hit
			sprite.Play(default_);

		if (!GetViewportRect().HasPoint(GlobalPosition) ||
			Position.DistanceTo(originalPosition) > maxDistance)
			QueueFree();
	}
	
	private void OnBodyEntered(Node body){
	
		if (body.HasMethod("OnHitByBullet")){
			sprite.Play(hit);
			body.Call("OnHitByBullet", this, this.player);
		}

		if (flag){
			sprite.AnimationFinished += () => QueueFree(); // Espera a que acabe
		}
		flag = true;
	}
	
}
