using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	private Button continuarButton;
	private Button salirButton;

	public override void _Ready()
	{
		continuarButton = GetNode<Button>("VBoxContainer/Continuar");
		salirButton = GetNode<Button>("VBoxContainer/Salir");

		continuarButton.Pressed += OnContinuarPressed;
		salirButton.Pressed += OnSalirPressed;

		Visible = false; // Empieza oculto
		ProcessMode = ProcessModeEnum.Always; // Para que reciba input en pausa
	}

	public void ShowMenu()
	{
		Visible = true;
		GetTree().Paused = true;
	}

	public void HideMenu()
	{
		Visible = false;
		GetTree().Paused = false;
	}

	private void OnContinuarPressed()
	{
		HideMenu();
	}

	private void OnSalirPressed()
	{
		GetTree().Quit();
	}
}
