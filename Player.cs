using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	public PackedScene BulletScene = GD.Load<PackedScene>("res://disparos.tscn");

	// 0: Rojo-Red
	// 1: Naranja-Orange
	// 2: Amarillo-Yellow
	// 3: Verde-Green
	// 4: Azul-Blue
	// 5: Morado-Purple
	
	public int color=0; 
	private Texture2D texturaRueda;
	private AnimatedSprite2D personajeAnimado;
	 
	private bool flagInvulnerabilidad=false;
	private Timer _timer;
	
	private Timer _blinkTimer;
	
	private List<AnimatedSprite2D> heartList = new List<AnimatedSprite2D>();
	private int health = 3;
	public float Speed = 200f;

	public override void _Ready()
	{
		personajeAnimado=GetNode<AnimatedSprite2D>("Sprite2D");
		
		Node heartsParent = GetNode("barra_vida/HBoxContainer");

		foreach (Node child in heartsParent.GetChildren())
		{
			if (child is AnimatedSprite2D heart)
			{
				heartList.Add(heart);
				heart.Play("mover");
			}
		}

		GD.Print($"Total corazones: {heartList.Count}");
		
		// Obtener referencia al Sprite2D o Control que quieres posicionar
		var rueda = GetNode<Node2D>("rueda/Rueda");

		// Obtener el tamaño del viewport
		Vector2 viewportSize = GetViewportRect().Size;

		// Colocar en la esquina inferior derecha, con un margen de 20px
		rueda.GlobalPosition = new Vector2(viewportSize.X - 100, viewportSize.Y - 100);
		
		_timer = new Timer();
		_timer.WaitTime = 1.0f;
		_timer.OneShot = true;
		_timer.Timeout += invulnerabilidad;
		AddChild(_timer);
	
		_blinkTimer = new Timer();
		_blinkTimer.WaitTime = 0.1f; // velocidad del parpadeo (cada 0.1 segundos)
		_blinkTimer.OneShot = false;
		_blinkTimer.Timeout += OnBlinkTimeout;
		AddChild(_blinkTimer);
	
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		//Arriba
		string anim = "";
		bool flipH = false;

		if (Input.IsKeyPressed(Key.W) && Input.IsKeyPressed(Key.A))
		{
			anim = "run_arriba_diagonal";
			flipH = false;
			velocity.Y-=1;
			velocity.X-=1;
		}
		else if (Input.IsKeyPressed(Key.W) && Input.IsKeyPressed(Key.D))
		{
			anim = "run_arriba_diagonal";
			flipH = true;
			velocity.Y-=1;
			velocity.X+=1;
		}else if (Input.IsKeyPressed(Key.S) && Input.IsKeyPressed(Key.A))
		{
			anim = "run_abajo_diagonal";
			flipH = false;
			velocity.Y+=1;
			velocity.X-=1;
		}else if (Input.IsKeyPressed(Key.S) && Input.IsKeyPressed(Key.D))
		{
			anim = "run_abajo_diagonal";
			flipH = true;
			velocity.Y+=1;
			velocity.X+=1;
		}
		else if (Input.IsKeyPressed(Key.W))
		{
			anim = "run_arriba";
			velocity.Y -= 1;
		}
		else if (Input.IsKeyPressed(Key.S))
		{
			anim = "run_abajo";
			velocity.Y += 1;
		}
		else if (Input.IsKeyPressed(Key.A))
		{
			anim = "run_lado";
			flipH = false;
			velocity.X -= 1;
		}
		else if (Input.IsKeyPressed(Key.D))
		{
			anim = "run_lado";
			flipH = true;
			velocity.X += 1;

		}
		else
		{
			anim = "idle"; // animación de estar quieto
		}

		if (!string.IsNullOrEmpty(anim))
		{
			personajeAnimado.FlipH = flipH;
			if (personajeAnimado.Animation != anim) // solo cambiar si es distinta
				personajeAnimado.Play(anim);
		}
		
		
		if(!Input.IsAnythingPressed()){
			personajeAnimado.Play("idle");
		}
		//Para debugg:
		if(Input.IsActionJustPressed("espacio")) {
			if(this.color<5){
				this.color++;
			}
			else this.color=0;
		}
		
		switch(color){
			case 0:
				texturaRueda = GD.Load<Texture2D>("res://assets/rueda_red.png");
				break;
			case 1:
				texturaRueda = GD.Load<Texture2D>("res://assets/rueda_orange.png");
				break;
			case 2:
				texturaRueda = GD.Load<Texture2D>("res://assets/rueda_yellow.png");
				break;
			case 3:
				texturaRueda = GD.Load<Texture2D>("res://assets/rueda_green.png");
				break;					
			case 4:
				texturaRueda = GD.Load<Texture2D>("res://assets/rueda_blue.png");
				break;
			default:
				texturaRueda = GD.Load<Texture2D>("res://assets/rueda_purple.png");
				break;
		}
		
		// Obtener el nodo Sprite2D 
		Sprite2D sprite = GetNode<Sprite2D>("rueda/Rueda");

		// Cambiar la textura
		sprite.Texture = texturaRueda;

		Velocity = velocity.Normalized() * Speed;
		MoveAndSlide();

		// disparar al hacer clic
		if (Input.IsActionJustPressed("mouse_left"))
			Shoot();
	}

	private void Shoot(){
		
		GD.Print("hola");
		// Obtener posición del mouse en el mundo
		Vector2 mousePosition = GetGlobalMousePosition();
		Vector2 direction = (mousePosition - GlobalPosition).Normalized();

		// Instanciar bala
		Bullet bullet = (Bullet)BulletScene.Instantiate();
		bullet.Position = GlobalPosition;
		bullet.Velocity = direction;
		bullet.color=this.color;
		bullet.player=this;
		
		GD.Print(bullet.Position);
		

		// Añadir al árbol
		GetTree().CurrentScene.AddChild(bullet);

	}
	
	public void takeDamage(){
		if (!flagInvulnerabilidad){
			if (health > 0)
				health--;

			updateLife();

			if (health <= 0)
				GetTree().Quit();

			flagInvulnerabilidad = true;

			// Reutilizamos el mismo timer
			_timer.Start();
			_blinkTimer.Start();
		}
	}
	
	private void updateLife(){
		for (int i = 0; i < heartList.Count; i++){
			bool herir = i >= health;

			if (herir && heartList[i].Animation != "herir"){
				heartList[i].Play("herir");
			}
   		}
	}
	
	private void invulnerabilidad(){
		flagInvulnerabilidad=false;
		_blinkTimer.Stop();
		personajeAnimado.Visible = true;
	}
	
	private void OnBlinkTimeout(){
		if (personajeAnimado != null)
			personajeAnimado.Visible = !personajeAnimado.Visible;
	}

}
