namespace Primerjuego2D.escenas.sistema;

using System;
using Godot;
using Primerjuego2D.nucleo.utilidades.log;


public partial class GestorColor : ColorRect
{
    [Export]
    public Color ColorFondo { get; set; } = new Color(ConstantesColores.NEGRO_ROTO);

    [Export]
    public Color ColorParticulas { get; set; } = new Color(ConstantesColores.AMARILLO_PASTEL);

    public override void _Ready()
    {
        Logger.Trace("GestorColor Ready.");
    }
}
