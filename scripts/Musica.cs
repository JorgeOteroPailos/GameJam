using Godot;
using System;

public partial class Musica : AudioStreamPlayer2D
{
	public override void _Ready()
{
	// Cargar el recurso de audio
	AudioStream stream = GD.Load<AudioStream>("res://music/Unholy_Knight.mp3");

	if (stream == null)
	{
		GD.PrintErr("Error: No se pudo cargar el archivo de audio");
		return;
	}

	// Configurar loop según el tipo de archivo
	if (stream is AudioStreamOggVorbis ogg)
	{
		ogg.Loop = true;
	}
	else if (stream is AudioStreamMP3 mp3)
	{
		mp3.Loop = true;
	}
	else if (stream is AudioStreamWav wav)
	{
		// Configuración correcta para WAV en Godot 4
		wav.LoopMode = AudioStreamWav.LoopModeEnum.Forward;
	}
	
	

}
}
