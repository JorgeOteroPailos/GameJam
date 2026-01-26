using Godot;
using System;

public partial class HUD_movil : CanvasLayer
{
	[Signal]
	public delegate void JoystickMovedEventHandler(Vector2 input);

	private bool joystickActive = false;
	private int activeTouchId = -1; // para trackear un toque específico

	private Control joystick;
	private TextureRect borde;
	private TextureRect centro;
	
	private TextureButton botonPausa;

	
	private Player player;

	private float deadzone = 0.15f;
	private float maxRadius; // en píxels, para el centro del knob
	private Vector2 centroDefaultPos; // posición local (top-left) por defecto del knob
	
public override void _Ready()
{
	joystick = GetNode<Control>("Joystick");
	borde = joystick.GetNode<TextureRect>("Borde");
	centro = joystick.GetNode<TextureRect>("Centro");

	// --- Escalar mediante Size en lugar de Scale (más fiable para Controls)
	float screenWidth = GetViewport().GetVisibleRect().Size.X;
	float desiredSize = screenWidth * 0.10f; // 10% del ancho
	borde.Size = new Vector2(desiredSize, desiredSize);

	// knob más pequeño, por ejemplo la mitad del borde (ajusta knobFactor si quieres otro tamaño)
	float knobFactor = 0.5f;
	float knobSize = desiredSize * knobFactor;
	centro.Size = new Vector2(knobSize, knobSize);
	
	botonPausa = GetNode<TextureButton>("BotonPausa");
	
	botonPausa.Pressed += OnBotonPausaPressed;

	// Centrar el Centro dentro del Borde (posición local = top-left corregida por la posición del borde)
	centroDefaultPos = borde.Position + (borde.Size - centro.Size) * 0.5f;
	centro.Position = centroDefaultPos;


	// El radio máximo (en px) será la mitad del borde menos la mitad del knob,
	// porque trabajamos con el centro del knob.
	maxRadius = (borde.Size.X * 0.5f) - (centro.Size.X * 0.5f);

	// Posicionar el joystick en la esquina inferior izquierda con margen 5% del viewport
	Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
	float margin = viewportSize.X * 0.05f; // 5% del ancho
	joystick.Position = new Vector2(margin, viewportSize.Y - borde.Size.Y - margin);
}




	public override void _Input(InputEvent @event)
	{
		// Tocar y soltar
		if (@event is InputEventScreenTouch touch)
		{
			if (touch.Pressed)
			{
				// Si tocas dentro del borde → joystick
				if (borde.GetGlobalRect().HasPoint(touch.Position))
				{
					joystickActive = true;
					activeTouchId = touch.Index;
					HandleDrag(touch.Position);
				}
				else
				{
					// toque fuera del joystick → disparo
					player.Shoot(player.GlobalPosition, touch.Position, player.color);
				}
			}
			else if (joystickActive && touch.Index == activeTouchId)
			{
				joystickActive = false;
				activeTouchId = -1;
				ResetCentro();
			}
		}
		// Arrastrar con touch
		else if (@event is InputEventScreenDrag drag)
		{
			if (joystickActive && drag.Index == activeTouchId)
				HandleDrag(drag.Position);
		}
		// Clic del ratón
		else if (@event is InputEventMouseButton mb)
		{
			if (mb.Pressed)
				HandleDrag(mb.Position);
			else
				ResetCentro(); // clic levantado → volver al centro
		}
		// Movimiento del ratón mientras clic está presionado
		else if (@event is InputEventMouseMotion mm)
		{
			if (Input.IsMouseButtonPressed(MouseButton.Left))
				HandleDrag(mm.Position);
			else
				ResetCentro(); // por si se soltó mientras movías
		}
	}


	private void HandleDrag(Vector2 globalPos)
{
	// Centro global del borde (rect global es fiable aunque cambies Size)
	Rect2 borderGlobalRect = borde.GetGlobalRect();
	Vector2 globalCenter = borderGlobalRect.Position + borderGlobalRect.Size * 0.5f;

	// Vector desde el centro global hasta el punto tocado (global)
	Vector2 dirGlobal = globalPos - globalCenter;

	// Limitar por el radio máximo (en píxels, calculado en _Ready)
	if (dirGlobal.Length() > maxRadius)
		dirGlobal = dirGlobal.Normalized() * maxRadius;

	// Nuevo centro global del knob (centro del sprite)
	Vector2 newGlobalKnobCenter = globalCenter + dirGlobal;

	// Convertir ese centro global a posición local top-left dentro de 'joystick'
	// top-left del knob local = newGlobalKnobCenter - globalPosJoystick - medio knob
	Vector2 joystickGlobalPos = joystick.GetGlobalRect().Position;
	Vector2 newLocalTopLeft = newGlobalKnobCenter - joystickGlobalPos - (centro.Size * 0.5f);

	centro.Position = newLocalTopLeft;

	// Normalizado en -1..1 (en base a maxRadius)
	Vector2 normalized = maxRadius > 0 ? dirGlobal / maxRadius : Vector2.Zero;
	if (normalized.Length() < deadzone)
		normalized = Vector2.Zero;

	EmitSignal(nameof(JoystickMoved), normalized);
}


	private void ResetCentro()
	{
		centro.Position = centroDefaultPos;
		EmitSignal(nameof(JoystickMoved), Vector2.Zero);
		//GD.Print("joystick reseteado");
	}
	
	public void SetPlayer(Player p)
	{
		player = p;
	}
	
	private void OnBotonPausaPressed()
	{
		var pausa = (Pausa)GetNode("/root/Pausa");
		pausa.TogglePause();
	}
}
