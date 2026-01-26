using Godot;
using System;

public partial class Bullet : Area2D
{
	private Texture2D nuevaTextura;
	public int color=0;
	public String default_="";
	public String hit="";
	private AnimatedSprite2D sprite;
	private CollisionShape2D collisionShape;
	
	private Node enemyHit = null;
	
	public Player player;
	public Vector2 Velocity = Vector2.Zero;
	public float Speed = 800f;
	public float maxDistance=400;
	private bool flagImpacte=false;
	private Vector2 originalPosition;

	public override void _Ready(){
		// 0: Rojo-Red
		// 1: Naranja-Orange
		// 2: Amarillo-Yellow
		// 3: Verde-Green
		// 4: Azul-Blue
		// 5: Morado-Purple
		
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		
		string[] colores={"red", "orange", "yellow", "green", "blue", "purple", "white"};
	
		default_="default_"+colores[color];
		hit="hit_"+colores[color];
		
		// Obtener el nodo Sprite2D 
		sprite = GetNode<AnimatedSprite2D>("Sprite2D");

		// Cambiar la textura
		//sprite.Texture = nuevaTextura;
		
		sprite.AnimationFinished += _OnAnimationFinished;
		
		originalPosition=this.Position;
		//GD.Print("Bullet creada en: " + Position);
		//GD.Print("Velocidad: " + Velocity);
		BodyEntered += OnBodyEntered;
		
	}


	public override void _PhysicsProcess(double delta){
		
		if (flagImpacte==true){
			return;
		}
		
		Position += Velocity * Speed * (float)delta;
		sprite.Rotation = MathF.Atan2(-Velocity.Y, -Velocity.X);

		if (sprite.Animation != hit) // solo reproducir default si no está en hit
			sprite.Play(default_);

		if (Position.DistanceTo(originalPosition) > maxDistance)
			QueueFree();
	}
	
private void OnBodyEntered(Node body)
{
	if (flagImpacte)
		return; // Ya impactó

	if (body.HasMethod("OnHitByBullet"))
	{
		flagImpacte = true;

		// Detener colisiones inmediatamente
		collisionShape.SetDeferred("disabled", true);

		// Guardar enemigo para llamar su método
		enemyHit = body;

		// Reproducir animación de impacto
		sprite.Play(hit);
		
		if (enemyHit != null && enemyHit.HasMethod("OnHitByBullet"))
		{
			enemyHit.Call("OnHitByBullet", this, player);
		}
		
		
	}
}

	
private void _OnAnimationFinished()
	{
	QueueFree(); // Destruye la bala al terminar la animación
}


	
}
