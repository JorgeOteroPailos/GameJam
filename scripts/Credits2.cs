using Godot;

public partial class Credits2 : Control
{
	[Export] public string NextScenePath = "res://escenas/inicio.tscn"; // Cambia a la escena que desees

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			GetTree().ChangeSceneToFile("res://escenas/inicio.tscn");
		}
	}
}
