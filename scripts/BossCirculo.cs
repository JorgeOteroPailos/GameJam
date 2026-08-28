using Godot;
using System;

public partial class BossCirculo : Circulo
{
	public override void _Ready()
	{
		base._Ready();
		VolverJefe();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		if (shootTimer.IsStopped())
		{
			Shoot();
		}
	}

	private async void Shoot()
	{
		shootTimer.Start();

		Vector2 origen = GlobalPosition;
		Vector2 playerPos = player.GlobalPosition;

		// Predictive aiming: slower bullets targeting player's future position
		float bulletSpeed = 220f; 
		float distance = origen.DistanceTo(playerPos);
		float timeToTarget = bulletSpeed > 0 ? distance / bulletSpeed : 0f;

		Vector2 futurePlayerPos = playerPos + (player.Velocity * timeToTarget);
		Vector2 direccionBase = (futurePlayerPos - origen).Normalized();

		float[] angulosOffset = new float[] { -0.26f, 0.0f, 0.26f };

		foreach (float offsetAngle in angulosOffset)
		{
			if (isDying || !IsInstanceValid(this)) 
				return;

			Vector2 direccionBala = direccionBase.Rotated(offsetAngle);

			Bullet bullet = (Bullet)BulletScene.Instantiate();
			bullet.Position = origen;
			bullet.Velocity = direccionBala;
			bullet.color = 7; // negro
			bullet.esDeJefe = true;
			bullet.Speed = bulletSpeed;

			GetTree().CurrentScene.AddChild(bullet);

			await ToSignal(GetTree().CreateTimer(0.15f), SceneTreeTimer.SignalName.Timeout);
		}
	}

	public void VolverJefe()
	{
		VELOCIDAD /= 1.5f;
		GD.Print("El jefe ahora es negro y color " + color);

		Scale *= new Vector2(3, 3);
	}
}
