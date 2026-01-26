using Godot;
using System;

public partial class Mundo2 : MundoBase
{
	
	[Export]
	public int nColores=6;
	
	protected override String siguienteEscena=>"res://escenas/recoger_llave.tscn";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		
		nEnemigos=16;
		
		player = GetNode<Player>("personaje");
		player.world="res://escenas/mundo_2.tscn";
		
		var personaje=GetNode<CharacterBody2D>("personaje");
		if(personaje is Player player1){
			player1.SetNColores(3);
		}
		
		Node Enemigos = GetNode("Enemigos");

		foreach (Node child in Enemigos.GetChildren()){
			if (child is CharacterBody2D enemigo){
				enemigos.Add(enemigo);

				if(enemigo is Circulo circulo){
					circulo.VELOCIDAD*=(float)1.25;
					if(nEnemigo%3==1){
						circulo.color = 2;
					}else if(nEnemigo%3==2){
						circulo.color=0;
					}
					spawnearEnemigo(circulo);
				}

				
				nEnemigo++;
			}
		}
		
	}
	
}
