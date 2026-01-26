using Godot;
using System;

public partial class Mundo5 : MundoBase
{
	[Export]
	public int nColores=6;
	
	protected override String siguienteEscena=>"res://escenas/visual_novel.tscn";
	
	// Called when the node enters the scene tree for the first time.
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		
		nEnemigos=13;
		
		hayJefe=true;
		
		player = GetNode<Player>("personaje");
		player.world="res://escenas/mundo_5.tscn";
		
		var personaje=GetNode<CharacterBody2D>("personaje");
		if(personaje is Player player1){
			player1.SetNColores(6);
		}
		Node Enemigos = GetNode("Enemigos");
		
		int ajuste=0;

		foreach (Node child in Enemigos.GetChildren()){
			if (child is CharacterBody2D enemigo){
				enemigos.Add(enemigo);
				

				if(enemigo is Circulo circulo){
					
					
					
					circulo.VELOCIDAD*=3.3f;
					if((nEnemigo-ajuste)%6==1){
						circulo.color = 2;
					}else if((nEnemigo-ajuste)%6==2){
						circulo.color=0;
					}else if((nEnemigo-ajuste)%6==3){
						circulo.color=3;
					}else if((nEnemigo-ajuste)%6==4){
						circulo.color=1;
					}else if((nEnemigo-ajuste)%6==5){
						circulo.color=5;
					}
					
					if(circulo.esJefe){
						ajuste++;
						circulo.color=6;
						circulo.VELOCIDAD/=3.3f;
					}
					spawnearEnemigo(circulo);
				}

				
				nEnemigo++;
			}
		}
		
		//nEnemigos-=ajuste;
		
	}
	
}
