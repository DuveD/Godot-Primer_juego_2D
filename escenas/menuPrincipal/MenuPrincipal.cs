namespace Primerjuego2D.escenas.menuPrincipal;

using System;
using Godot;
using Primerjuego2D.escenas.batalla;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.nucleo.utilidades.log;


public partial class MenuPrincipal : Control
{
    [Export]
    public GestorColor GestorColor { get; set; }

    ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    public override void _Ready()
    {
        Logger.Trace("MenuPrincipal Ready.");

        this.Fondo.Color = this.GestorColor.ColorFondo;
    }

    private void OnButtonEmpezarPartidaPressed()
    {
        Logger.Trace("Botón 'ButtonEmpezarPartida' pulsado.");
        GetTree().ChangeSceneToFile("res://escenas/batalla/Batalla.tscn");
    }

    private void OnButtonCargarPartidaPressed()
    {
        Logger.Trace("Botón 'ButtonCargarPartida'' pulsado.");
    }

    private void OnButtonSalirPressed()
    {
        Logger.Trace("Botón 'ButtonSalir' pulsado.");
        this.GetTree().Quit();
    }
}
