using Godot;
using System;
using System.Collections.Generic;

public abstract partial class MundoBase : Node2D
{
	protected Player player;
	
	[Export]
	public PackedScene CirculoScene;
	
	[Export] protected bool hayJefe=false;
	
	protected int nEnemigo=0;
	
	protected Random random = new Random();
	
	protected List<CharacterBody2D> enemigos = new List<CharacterBody2D>();
	
	public virtual int nEnemigos { get; set; }
	
	protected virtual String siguienteEscena { get; }
	
	public void EnemigoDerrotado() {
		//GD.Print("Enemigo derrotado.");
		nEnemigos--;
		//GD.Print($"Quedan {nEnemigos} enemigos.");
		if (nEnemigos <= 0)
		{
			GD.Print("Todos los enemigos derrotados. Cambio de escena a "+siguienteEscena);
			GetTree().ChangeSceneToFile(siguienteEscena);
		}else if (hayJefe){
			if(nEnemigos==1){
				player.cambiarColor(true);
				GD.Print("Cambiando a color multicolor desde MundoBase");
			}
		}
		
	}
	
	protected void spawnearEnemigo(Circulo miCirculo){
		miCirculo.player=player;
		
		string[] colores = { "red", "orange", "yellow", "green", "blue", "purple", "negro" };
		
		miCirculo.colorEscrito=colores[miCirculo.color];
		miCirculo.Aparecer();

	}
	
	
}
