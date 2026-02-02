using Godot;
using System;

public partial class Circulo : CharacterBody2D
{
	private bool isDying = false;
	private bool isAppearing = false;
	
	public PackedScene BulletScene = GD.Load<PackedScene>("res://escenas/disparos.tscn");
	
	[Export] public bool esJefe = false;

	public String colorEscrito=""; 
	
	private MundoBase padre;
	
	private const float RETROCESO_BALA=10;
	private const float RETROCESO_CHOQUE=100;
	public float VELOCIDAD=50;

	private Vector2 originalPos;
	[Export] public int color=4;
	[Export] private CollisionShape2D collisionShape2D;
	
	public Player player;

	AnimatedSprite2D sprite;
	
	private float shootCooldown = 5.0f; // tiempo mínimo entre disparos en segundos
	private Timer shootTimer;

	public override void _Ready()
	{

		// Obtener el nodo Sprite2D
		sprite = GetNode<AnimatedSprite2D>("Circulo");

		originalPos = GlobalPosition;
		
				
		padre = (MundoBase)GetParent().GetParent();
		
		var area = GetNode<Area2D>("DamageArea");
		area.BodyEntered += OnBodyEntered;
		
		if (esJefe){
			volverJefe();
		}
		
		shootTimer = new Timer();
		shootTimer.WaitTime = shootCooldown;
		shootTimer.OneShot = true;
		AddChild(shootTimer);
	}

	public void OnHitByBullet(Node bullet){
		if(isAppearing){
			return;
		}
		
		if (bullet is Bullet bullet1)
		{
			if(bullet1.esDeJefe){
				return;
			}
			bullet1.impactar();
			if (bullet1.color != this.color){
				Vector2 retroceso = bullet1.Velocity.Normalized() * RETROCESO_BALA;
				Tween tween = GetTree().CreateTween();
				tween.TweenProperty(this, "global_position", GlobalPosition + retroceso, 0.1f);
			}
			else{
				isDying = true; // Activamos estado de muerte
				sprite.Play("demonio_" + colorEscrito + "_death");
				//sprite.AnimationFinished += () => QueueFree();
				sprite.AnimationFinished += () => {
					
					 //Detener colisiones inmediatamente 
					collisionShape2D.SetDeferred("disabled", true);
					
					padre.EnemigoDerrotado();
					
					QueueFree(); // Eliminar el enemigo
				};
				
				if(!bullet1.esDeJefe){
					player.cambiarColor(false);
				}
			}
		}
	}
	
	private void OnBodyEntered(Node body){

			if (body is Player jugador){
				//GD.Print("Jugador tocado por enemigo "+colorEscrito);
				if(isDying==true || isAppearing ==true){
					return;
				}
				
				jugador.takeDamage();
				Vector2 retroceso = (GlobalPosition - jugador.GlobalPosition).Normalized() * RETROCESO_CHOQUE;
				var tween = CreateTween();
				tween.TweenProperty(this, "global_position", GlobalPosition + retroceso, 0.1f);
			}
		
	}
	
	public override void _PhysicsProcess(double delta){
		// Si está muriendo, no hacer nada más
		if (isDying || isAppearing)
			return;

		// Vector dirección hacia el jugador
		Vector2 direction = (player.Position - Position).Normalized();
		
		Vector2 separation = Vector2.Zero;
		float minDistance = 40f; // Distancia mínima entre enemigos

		foreach (Node other in GetParent().GetChildren()){
			if (other is Circulo otro && otro != this && !otro.isDying){
				float dist = GlobalPosition.DistanceTo(otro.GlobalPosition);
				if (dist < minDistance && dist > 0){
					// Empujar en dirección contraria
					separation += (GlobalPosition - otro.GlobalPosition).Normalized() * (minDistance - dist);
				}
			}
		}

		Vector2 finalDir = (direction + separation.Normalized() * 0.5f).Normalized();

		Position += finalDir * VELOCIDAD * (float)delta;

		// Determinar animación en base a la dirección
		string anim = "";
		bool flipH = false;

		if (direction.X < 0 && direction.Y < 0){
			anim = "demonio_" + colorEscrito + "_arriba_diagonal";
			flipH = false;
		}else if (direction.X > 0 && direction.Y < 0){
			anim = "demonio_" + colorEscrito + "_arriba_diagonal";
			flipH = true;
		}else if (direction.X < 0 && direction.Y > 0){
			anim = "demonio_" + colorEscrito + "_abajo_diagonal";
			flipH = false;
		}else if (direction.X > 0 && direction.Y > 0){
			anim = "demonio_" + colorEscrito + "_abajo_diagonal";
			flipH = true;
		}else if (direction.Y < 0){
			anim = "demonio_" + colorEscrito + "_arriba";
		}else if (direction.Y > 0){
			anim = "demonio_" + colorEscrito + "_abajo";
		}else if (direction.X < 0){
			anim = "demonio_" + colorEscrito + "_lado";
			flipH = false;
		}else if (direction.X > 0){
			anim = "demonio_" + colorEscrito + "_lado";
			flipH = true;
		}

		if (!string.IsNullOrEmpty(anim)){
			sprite.FlipH = flipH;
			if (sprite.Animation != anim)
				sprite.Play(anim);
		}
		
		if(esJefe && shootTimer.IsStopped()){
			Shoot();
		}
		
	}
	
	private void Shoot(){
		
		
		Vector2 origen=GlobalPosition;
		Vector2 objetivo=player.Position;
		
		Vector2 direction = (objetivo - origen).Normalized();

		// Instanciar bala
		Bullet bullet = (Bullet)BulletScene.Instantiate();
		bullet.Position = origen;
		bullet.Velocity = direction;
		bullet.color=7; //negro
		bullet.esDeJefe=true;

		// Añadir al árbol
		GetTree().CurrentScene.AddChild(bullet);
		
		shootTimer.Start();
	}
	
	public void volverJefe(){
		this.VELOCIDAD/=1.5f;

		
		GD.Print("el jefe ahora es negro y color"+this.color);
		
		// Triplicar el tamaño del sprite
		this.Scale *= new Vector2(3, 3);
		
		// Ajustar el tamaño de la hitbox también
		if (collisionShape2D.Shape is CircleShape2D circle){
			//circle.Radius *= 3;
		}else if (collisionShape2D.Shape is RectangleShape2D rect){
			//rect.Size *= new Vector2(1, 1);
		}
	}
	
	/*
	private void spawnearEnemigos(){
		// Colores que coincidan con tus animaciones
		string[] coloresNombres = { "rojo", "naranja", "amarillo", "verde", "azul", "morado" };

		float radio = 200f; // distancia desde el jugador
		int total = 6;

		for (int i = 0; i < total; i++){
			// Instanciar la escena
			Circulo enemigo = padre.CirculoScene.Instantiate<Circulo>();

			// Asignar propiedades
			enemigo.player = player;
			enemigo.color = i;                     // 0..5
			enemigo.colorEscrito = coloresNombres[i];

			// Posición en círculo alrededor del jugador
			float angulo = Mathf.Tau * i / total;  // divide 360° en 6
			Vector2 offset = new Vector2(Mathf.Cos(angulo), Mathf.Sin(angulo)) * radio;
			enemigo.Position = player.Position + offset;

			// Añadir a la escena
			AddChild(enemigo);

			GD.Print($"Spawned enemigo {i} color={enemigo.colorEscrito} en {enemigo.Position}");
		}

		// Actualizar contador
		padre.nEnemigos += total;
		
	}
	*/
	
	public void Aparecer()
{
	isAppearing = true;
	sprite.Play("demonio_" + colorEscrito + "_aparecer");

	// Cuando termine la animación, empezar a comportarse normal
	sprite.AnimationFinished += OnAparecerFinished;
}

private void OnAparecerFinished()
{
	// Si acaba justo la animación de aparecer
	if (sprite.Animation == "demonio_" + colorEscrito + "_aparecer")
	{
		isAppearing = false;
		sprite.AnimationFinished -= OnAparecerFinished; // quitar handler para que no se dispare en otras animaciones
	}
}

}
