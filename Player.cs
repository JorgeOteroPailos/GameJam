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
	
	private List<TextureRect> heartList = new List<TextureRect>();
	private int health = 3;
	public float Speed = 200f;

	public override void _Ready()
	{
		Node heartsParent = GetNode("barra_vida/HBoxContainer");

		foreach (Node child in heartsParent.GetChildren())
		{
			if (child is TextureRect heart)
			{
				heartList.Add(heart);
			}
		}

		GD.Print($"Total corazones: {heartList.Count}");
		
		// Obtener referencia al Sprite2D o Control que quieres posicionar
		var rueda = GetNode<Node2D>("rueda/Rueda");

		// Obtener el tamaño del viewport
		Vector2 viewportSize = GetViewportRect().Size;

		// Colocar en la esquina inferior derecha, con un margen de 20px
		rueda.GlobalPosition = new Vector2(viewportSize.X - 100, viewportSize.Y - 100);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsKeyPressed(Key.W)&&this.GlobalPosition.Y>0) velocity.Y -= 1;
		if (Input.IsKeyPressed(Key.S)&&this.GlobalPosition.Y<640) velocity.Y += 1;
		if (Input.IsKeyPressed(Key.A)&&this.GlobalPosition.X>0) velocity.X -= 1;
		if (Input.IsKeyPressed(Key.D)&&this.GlobalPosition.X<1140) velocity.X += 1;
		
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
		if(health>0){
			health--;
		}
		updateLife();
	}
	
	private void updateLife(){
		for(int i=0;i<heartList.Count;i++){
			heartList[i].Visible=i<health;
		}
	}
}
