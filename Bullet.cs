using Godot;
using System;

public partial class Bullet : Area2D
{
	public Vector2 Velocity = Vector2.Zero;
	public float Speed = 600f;
	public float maxDistance=400;
	bool flag=false;
	private Vector2 originalPosition;

	public override void _Ready()
	{
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
		if(flag){
			QueueFree(); // Elimina la bala		
		}
		flag=true;
	}
	
}
