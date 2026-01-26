using Godot;
using System;

public partial class Mundo3 : MundoBase
{	
	[Export]
	public int nColores=6;
	
	protected override String siguienteEscena=>"res://escenas/recoger_llave.tscn";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		
		nEnemigos=20;
		
		player = GetNode<Player>("personaje");
		player.world="res://escenas/mundo_3.tscn";
		
		var personaje=GetNode<CharacterBody2D>("personaje");
		if(personaje is Player player1){
			player1.SetNColores(4);
		}
		
		Node Enemigos = GetNode("Enemigos");

		foreach (Node child in Enemigos.GetChildren()){
			if (child is CharacterBody2D enemigo){
				enemigos.Add(enemigo);

				if(enemigo is Circulo circulo){
					circulo.VELOCIDAD*=(float)1.6;
					if(nEnemigo%4==1){
						circulo.color = 2;
					}else if(nEnemigo%4==2){
						circulo.color=0;
					}else if(nEnemigo%4==3){
						circulo.color=3;
					}
					spawnearEnemigo(circulo);
				}

				
				nEnemigo++;
			}
		}
		
	}
	
}
