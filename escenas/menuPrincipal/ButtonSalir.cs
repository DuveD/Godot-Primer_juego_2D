using Godot;
using Primerjuego2D.escenas;

public partial class ButtonSalir : Button
{
	public override void _Ready()
	{
		this.FocusEntered += OnFocusedEntered;
	}

	public void OnFocusedEntered()
	{
		Global.GestorAudio.ReproducirSonido("kick.mp3");
	}
}
