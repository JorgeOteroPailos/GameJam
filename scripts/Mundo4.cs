using Godot;
using System;

public partial class Mundo4 : MundoBase
{
	[Export]
	public int nColores=6;
	
	protected override String siguienteEscena=>"res://escenas/recoger_llave.tscn";
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		
		nEnemigos=24;
		
		player = GetNode<Player>("personaje");
		player.world="res://escenas/mundo_4.tscn";
		
		var personaje=GetNode<CharacterBody2D>("personaje");
		if(personaje is Player player1){
			player1.SetNColores(6);
		}
		
		Node Enemigos = GetNode("Enemigos");

		foreach (Node child in Enemigos.GetChildren()){
			if (child is CharacterBody2D enemigo){
				enemigos.Add(enemigo);

				if(enemigo is Circulo circulo){
					circulo.VELOCIDAD*=2;
					if(nEnemigo%6==1){
						circulo.color = 2;
					}else if(nEnemigo%6==2){
						circulo.color=0;
					}else if(nEnemigo%6==3){
						circulo.color=3;
					}else if(nEnemigo%6==4){
						circulo.color=1;
					}else if(nEnemigo%6==5){
						circulo.color=5;
					}
					spawnearEnemigo(circulo);
				}

				nEnemigo++;
			}
		}
		
	}
	
}
