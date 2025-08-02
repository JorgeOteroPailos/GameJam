using Godot;
using System;


//EL MAPA MIDE 1140*648

public partial class Mundo : Node2D
{
	[Export]
	public PackedScene CirculoScene;
	
	[Export]
	public int nEnemigos=10;
	
	[Export]
	public int nColores=6;
	
	private float minDistance =500f;
	
	private Random random = new Random();
	
	private Player player;
	
	public override void _Ready(){
		
		player = GetNode<Player>("Node2D"); // Cambia esta ruta al nodo real del jugador
		if(player == null){
			GD.PrintErr("No se encontró el nodo Player!(Mundo, método _ready)");
		}
		
		for (int i = 0; i < nEnemigos; i++)
		{
			spawnearEnemigo(i);
		}
	}
	
	private void spawnearEnemigo(int color){
		// Instanciar una escena "Circulo.tscn"
		Circulo miCirculo = (Circulo)CirculoScene.Instantiate();
		
		miCirculo.inicializar(color % nColores);
		miCirculo.player=player;

		Vector2 playerPosition = player.Position;
		Vector2 newPosition;

		do
		{
			// Generamos una posición aleatoria dentro de un rango (ajusta según tu mundo/juego)
			float x = (float)(random.NextDouble() * 1920); // por ejemplo ancho pantalla 1920
			float y = (float)(random.NextDouble() * 1080); // alto pantalla 1080

			newPosition = new Vector2(x, y);

		// Repetir mientras la distancia sea menor a 500
		} while (
			playerPosition.DistanceTo(newPosition) < minDistance ||
			!IsInsideMap(newPosition)
		);


		miCirculo.Position = newPosition;


		AddChild(miCirculo);
		
		GD.Print($"Enemigo creado: ");

	}
	
	bool IsInsideMap(Vector2 position){
		return position.X >= 0 && position.X <= 1140 &&
			   position.Y >= 0 && position.Y <= 648;
	}
	
}
