//HOLA

using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	
	[Export] public int nColores=2;
	
	public PackedScene BulletScene = GD.Load<PackedScene>("res://escenas/disparos.tscn");

	// 0: Rojo-Red
	// 1: Naranja-Orange
	// 2: Amarillo-Yellow
	// 3: Verde-Green
	// 4: Azul-Blue
	// 5: Morado-Purple
	
	public int color=4;
	public string world;
	private Texture2D texturaRueda;
	private AnimatedSprite2D personajeAnimado;
	 
	private bool flagInvulnerabilidad=false;
	private Timer _timer;
	
	private Timer _blinkTimer;
	
	private List<AnimatedSprite2D> heartList = new List<AnimatedSprite2D>();
	private int health = 3;
	
	public const float speed = 270f;

	// Límites del mapa
	private const float minX = 10f;
	private const float maxX = 1130f;
	private const float minY = 10f;
	private const float maxY = 638f;
	
	private float shootCooldown = 0.5f; // tiempo mínimo entre disparos en segundos
	private Timer shootTimer;
	
	//joystick para pantalla táctil
	private Vector2 _joystickInput = Vector2.Zero;
	private bool _isMobile;
	private Func<Vector2> _getInput; // estrategia de entrada
	
	private const int tamanoCursor = 96;


	public override void _Ready()
	{
		_getInput = GetKeyboardInput;
		_isMobile = OS.HasFeature("mobile");
		//_isMobile=true;
		
		if(_isMobile)
		{
			_getInput = GetJoystickInput;
			// Cargar la escena del HUD
			PackedScene hudScene = GD.Load<PackedScene>("res://escenas/HUD_movil.tscn");

			// Instanciar
			CanvasLayer hud = (CanvasLayer)hudScene.Instantiate();
			
			hud.CallDeferred("SetPlayer", this);

			// Añadir al árbol, encima de todo
			GetTree().CurrentScene.CallDeferred("add_child", hud);

			// Conectar la señal del HUD al método SetJoystickInput
			var error = hud.Connect("JoystickMoved", new Callable(this, nameof(SetJoystickInput)));
			GD.Print($"Señal conectada JoystickMoved → SetJoystickInput: {error}");

		}

		
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

		//GD.Print($"Total corazones: {heartList.Count}");
		
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
		
		shootTimer = new Timer();
		shootTimer.WaitTime = shootCooldown;
		shootTimer.OneShot = true;
		AddChild(shootTimer);
		
		asertarColor();
	}
	
	public void SetJoystickInput(Vector2 input)
	{
		_joystickInput = input;
		//GD.Print($"Joystick input recibido: {input}");
	}

	private Vector2 GetJoystickInput() => _joystickInput;

	private Vector2 GetKeyboardInput()
	{
		
		Vector2 velocity = Vector2.Zero;
		
		if (Input.IsActionPressed("moverArriba") && GlobalPosition.Y > minY)
			velocity.Y -= 1;
		if (Input.IsActionPressed("moverAbajo") && GlobalPosition.Y < maxY)
			velocity.Y += 1;
		if (Input.IsActionPressed("moverIzquierda") && GlobalPosition.X > minX)
			velocity.X -= 1;
		if (Input.IsActionPressed("moverDerecha") && GlobalPosition.X < maxX)
			velocity.X += 1;
			
		return velocity.Normalized();
	}

	public override void _PhysicsProcess(double delta){
		// ---- MOVIMIENTO ----
		Vector2 inputVector = _getInput();

		// zona muerta
		if (inputVector.Length() < 0.1f)
			inputVector = Vector2.Zero;

		// mover
		Velocity = inputVector * speed;
		MoveAndSlide();

		// limitar dentro de los bordes
		GlobalPosition = new Vector2(
			Mathf.Clamp(GlobalPosition.X, minX, maxX),
			Mathf.Clamp(GlobalPosition.Y, minY, maxY)
		);

		
		// ---- ANIMACIONES ----
		if (Velocity != Vector2.Zero)
		{

			float angle = Mathf.RadToDeg(Mathf.Atan2(Velocity.Y, Velocity.X));
			string anim = "idle";
			bool flipH = personajeAnimado.FlipH;

			if (angle > -22.5f && angle <= 22.5f)
			{
				anim = "run_lado";   // derecha
				flipH = true;
			}
			else if (angle > 22.5f && angle <= 67.5f)
			{
				anim = "run_abajo_diagonal";
				flipH = true;
			}
			else if (angle > 67.5f && angle <= 112.5f)
			{
				anim = "run_abajo";
			}
			else if (angle > 112.5f && angle <= 157.5f)
			{
				anim = "run_abajo_diagonal";
				flipH = false;
			}
			else if (angle > 157.5f || angle <= -157.5f)
			{
				anim = "run_lado";   // izquierda
				flipH = false;
			}
			else if (angle > -157.5f && angle <= -112.5f)
			{
				anim = "run_arriba_diagonal";
				flipH = false;
			}
			else if (angle > -112.5f && angle <= -67.5f)
			{
				anim = "run_arriba";
			}
			else if (angle > -67.5f && angle <= -22.5f)
			{
				anim = "run_arriba_diagonal";
				flipH = true;
			}

			personajeAnimado.FlipH = flipH;
			if (personajeAnimado.Animation != anim)
				personajeAnimado.Play(anim);
		}else{
			Velocity = Vector2.Zero;
			if (personajeAnimado.Animation != "idle")
				personajeAnimado.Play("idle");
		}

		if(!Input.IsAnythingPressed()){
			personajeAnimado.Play("idle");
		}

		// disparar al hacer clic
		if (Input.IsActionJustPressed("mouse_left"))
		{
			//hay una descompensación chunga entre a donde debe ir la bala y a donde va, y ns de donde sale pero aqui se arregla
			Shoot(GlobalPosition,GetGlobalMousePosition() + new Vector2(0, tamanoCursor) - new Vector2(11 * tamanoCursor / 50f, 34 * tamanoCursor / 50f), this.color);
		}
	}
	
	public void cambiarColor(bool tocaJefe){
		
		if(tocaJefe){
			color=6;
			asertarColor();
			GD.Print("Cambiando color a blanco");
			return;
		}
		
		switch(nColores){
				case 2:
					if(color==2) color=4;
					else if(color==4) color=2;
					break;
				case 3:
					if(color==0) color=2;
					else if(color==2) color=4;
					else if(color==4) color=0;
					break;
				case 4:
					if(color==0) color=2;
					else if(color>1&&color<4) color++;
					else if(color==4) color=0; 
					break;
				case 6:
					if(color<5) color++;
					else color=0;
					break;
				default:
					break;
			}
			
			asertarColor();
	}
	
	private void asertarColor(){
		string[] colores = { "red", "orange", "yellow", "green", "blue", "purple", "white" };
		string[] coloresCastellano = {"rojo", "naranja", "amarillo", "verde", "azul", "lila", "blanco"};
		texturaRueda = GD.Load<Texture2D>("res://assets/rueda_"+nColores+"_"+colores[color]+".png");
		
		// Obtener el nodo Sprite2D 
		Sprite2D sprite = GetNode<Sprite2D>("rueda/Rueda");

		// Cambiar la textura
		sprite.Texture = texturaRueda;
		
		//CAMBIAR EL CURSOR
		if(!_isMobile){
			// Cargar la textura como imagen
			var img = GD.Load<Texture2D>("res://assets/pincel/pincel_"+coloresCastellano[color]+".png").GetImage();
			
			// Tamaño base relativo al viewport (por ejemplo, 4% de la altura)
			float factor = (float)tamanoCursor / 50f; // 96/50 ≈ 1.92

			// Redimensionar la imagen al tamaño deseado (manteniendo aspecto cuadrado)
			img.Resize(tamanoCursor, tamanoCursor, Image.Interpolation.Nearest);

			// Crear nueva textura a partir de la imagen escalada
			var tex = ImageTexture.CreateFromImage(img);

			// Si no se especifica hotspot, usar el centro
			Vector2 centro = new Vector2(11 * factor, 34 * factor);

			// Asignar cursor personalizado
			Input.SetCustomMouseCursor(tex, Input.CursorShape.Arrow, centro);
		}
	}

	public void Shoot(Vector2 origen, Vector2 objetivo, int colorDisparo){
		
		if(!shootTimer.IsStopped()){ return; }
		
		Vector2 direction = (objetivo - origen).Normalized();

		// Instanciar bala
		Bullet bullet = (Bullet)BulletScene.Instantiate();
		bullet.Position = origen;
		bullet.Velocity = direction;
		bullet.color=colorDisparo;
		bullet.player=this;

		// Añadir al árbol
		GetTree().CurrentScene.AddChild(bullet);
		
		shootTimer.Start();
	}
	
	public void takeDamage() {
		if (!flagInvulnerabilidad) {
			if (health > 0)
				health--;

			updateLife();

			if (health <= 0) {
				GetTree().CallDeferred("change_scene_to_file", world);
			}

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
	
	public void SetNColores(int nuevoValor){
		nColores = nuevoValor;

		// Normalizar color para que sea válido
		switch (nColores)
		{
			case 2:
				if (color != 2 && color != 4) color = 2;
				break;
			case 3:
				if (color != 0 && color != 2 && color != 4) color = 0;
				break;
			case 4:
				if (color != 0 && color != 2 && color != 3 && color != 4) color = 0;
				break;
			case 6:
				if (color < 0 || color > 5) color = 0;
				break;
		}
		
		asertarColor();
	}

	/*
	public void OnHitByBullet(Node bullet, Player player){
		//notese que player aquí es null, pq la bala la disparó el jefe
		if (bullet is Bullet bullet1){
			//this.takeDamage();
		}
	}
	*/

}
