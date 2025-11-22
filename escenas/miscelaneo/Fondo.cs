namespace Primerjuego2D.escenas.miscelaneo;

using System;
using Godot;
using Primerjuego2D.escenas.sistema;


public partial class Fondo : Control
{
    [Export]
    public ColorRect ColorFondo { get; set; }

    [Export]
    public GestorColor _GestorColor { get; set; }
    public GestorColor GestorColor => _GestorColor ??= Juego.GestorColor;

    public override void _Ready()
    {
        this.ColorFondo.Color = this.GestorColor.ColorFondo;
    }
}
