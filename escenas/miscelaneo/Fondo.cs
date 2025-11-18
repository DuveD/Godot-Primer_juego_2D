namespace Primerjuego2D.escenas.miscelaneo;

using System;
using Godot;
using Primerjuego2D.escenas.sistema;


public partial class Fondo : Control
{
    [Export]
    public ColorRect ColorFondo { get; set; }

    [Export]
    public GestorColor GestorColor { get; set; }

    public override void _Ready()
    {
        if (this.GestorColor != null)
            this.ColorFondo.Color = this.GestorColor.ColorFondo;
    }
}
