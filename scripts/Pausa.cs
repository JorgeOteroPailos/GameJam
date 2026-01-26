using Godot;
using System;

public partial class Pausa : CanvasLayer
{
	private TextureButton continuarButton;
	private TextureButton opcionesButton;
	private TextureButton salirButton;
	private TextureButton volverButton;
	
	private VBoxContainer opcionesContainer;
	private VBoxContainer basicoContainer;
	private HBoxContainer idiomasContainer;
	

	public override void _Ready()
{
	ProcessMode = Node.ProcessModeEnum.Always;
	Visible = false;
	
	continuarButton = GetNode<TextureButton>("PanelContainer/VBoxContainer/boton_continuar");
	opcionesButton = GetNode<TextureButton>("PanelContainer/VBoxContainer/boton_opciones");
	salirButton = GetNode<TextureButton>("PanelContainer/VBoxContainer/boton_salir");
	volverButton = GetNode<TextureButton>("PanelContainer/VBoxContainerOpciones/boton_volver");
	
	opcionesContainer=GetNode<VBoxContainer>("PanelContainer/VBoxContainerOpciones");
	opcionesContainer.Visible=false;
	basicoContainer=GetNode<VBoxContainer>("PanelContainer/VBoxContainer");
	basicoContainer.Visible=true;
	idiomasContainer=GetNode<HBoxContainer>("PanelContainer/VBoxContainerOpciones/HBoxContainerIdiomas");

	CargarTexturasSegunIdioma();
	
	ConstruirBotonesIdioma();

	continuarButton.Pressed += OnContinuarPressed;
	opcionesButton.Pressed += OnOpcionesPressed;
	salirButton.Pressed += OnSalirPressed;
	volverButton.Pressed+= OnVolverPressed;
}


	private void CargarTexturasSegunIdioma()
	{
		string idioma = Estado.idioma;
		continuarButton.TextureNormal = GD.Load<Texture2D>($"res://assets/menus/{idioma}/boton_continuar.png");
		opcionesButton.TextureNormal = GD.Load<Texture2D>($"res://assets/menus/{idioma}/boton_opciones.png");
		salirButton.TextureNormal = GD.Load<Texture2D>($"res://assets/menus/{idioma}/boton_salir.png");
		volverButton.TextureNormal =GD.Load<Texture2D>($"res://assets/menus/{idioma}/boton_volver.png");
		
		continuarButton.TextureHover = GD.Load<Texture2D>($"res://assets/menus/{idioma}/boton_continuar_hover.png");
		opcionesButton.TextureHover = GD.Load<Texture2D>($"res://assets/menus/{idioma}/boton_opciones_hover.png");
		salirButton.TextureHover = GD.Load<Texture2D>($"res://assets/menus/{idioma}/boton_salir_hover.png");
		volverButton.TextureHover = GD.Load<Texture2D>($"res://assets/menus/{idioma}/boton_volver_hover.png");
	}

	public void TogglePause()
	{
		bool paused = !GetTree().Paused;
		GetTree().Paused = paused;
		Visible = paused;
		opcionesContainer.Visible=false;
		basicoContainer.Visible=true;
	}

	private void OnContinuarPressed()
	{
		TogglePause();
	}
	
	private void OnVolverPressed(){
		opcionesContainer.Visible=false;
		basicoContainer.Visible=true;
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("Escape"))
		{
			TogglePause();
		}
	}

	private void OnOpcionesPressed()
	{
		if(opcionesContainer.Visible==false){
			opcionesContainer.Visible=true;
			basicoContainer.Visible=false;
			CargarTexturasSegunIdioma();
		}else{
			opcionesContainer.Visible=false;
			basicoContainer.Visible=true;
		}
		
	}

	private void OnSalirPressed()
	{
		GetTree().Quit();
	}
	
	private void ConstruirBotonesIdioma()
	{
		// Vaciar por si ya tenía hijos
		foreach (Node n in idiomasContainer.GetChildren())
			n.QueueFree();

		foreach (var code in Estado.idiomasDisponibles) // p.ej. ["gl","es","en"]
		{
			string lang = code; // capturar variable

			var btn = new TextureButton
			{
				Name = lang,
				TextureNormal = GD.Load<Texture2D>($"res://assets/menus/botones_idioma/{lang}.png"),
				CustomMinimumSize = new Vector2(72, 72),
				StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered,
				FocusMode = Control.FocusModeEnum.None
			};

			btn.Pressed += () =>
			{
				if (Estado.idioma == lang) return;
				Estado.idioma = lang;
				CargarTexturasSegunIdioma(); // recarga las texturas del menú

				// (Opcional) resaltar activo
				foreach (Node m in idiomasContainer.GetChildren())
					if (m is TextureButton tb)
						tb.Modulate = (tb.Name == Estado.idioma) ? Colors.White : new Color(1, 1, 1, 0.6f);
			};

			idiomasContainer.AddChild(btn);
		}

		// (Opcional) resaltar activo al construir
		foreach (Node m in idiomasContainer.GetChildren())
			if (m is TextureButton tb)
				tb.Modulate = (tb.Name == Estado.idioma) ? Colors.White : new Color(1, 1, 1, 0.6f);
	}
}
