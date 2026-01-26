using Godot;
using System;

public partial class Mundo1 : MundoBase
{
	[Export]
	public int nColores=6;
	
	protected override String siguienteEscena=>"res://escenas/recoger_llave.tscn";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		
		nEnemigos=12;
		
		player = GetNode<Player>("personaje");
		player.world="res://escenas/mundo_1.tscn";
		
		Node Enemigos = GetNode("Enemigos");

		foreach (Node child in Enemigos.GetChildren()){
			if (child is CharacterBody2D enemigo){
				enemigos.Add(enemigo);
				

				if (nEnemigo % 2 == 0 && enemigo is Circulo circulo){
					circulo.color = 2;
					
				}

				spawnearEnemigo((Circulo)enemigo);
				nEnemigo++;
			}
		}
		
		//AnimatedSprite2D llamita = GetNode<AnimatedSprite2D>("llamita");
		//llamita.Play("default");	
	}
}
