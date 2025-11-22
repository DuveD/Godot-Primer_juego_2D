using Godot;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.miscelaneo;

public partial class Fondo : Control
{
    [Export]
    public ColorRect ColorFondo { get; set; }

    [Export]
    public GestorColor _GestorColor { get; set; }

    public GestorColor GestorColor => _GestorColor ??= Global.GestorColor;

    public override void _Ready()
    {
        Logger.Trace(this.Name + " Ready.");

        this.ColorFondo.Color = this.GestorColor.ColorFondo;
    }
}
