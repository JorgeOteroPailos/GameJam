using Godot;

public partial class Credits : Control
{
	[Export] public string NextScenePath = "res://escenas/credits_2.tscn"; // Cambia a la escena que desees

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			GetTree().ChangeSceneToFile("res://escenas/credits_2.tscn");
		}
	}
}
