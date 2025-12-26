using Godot;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.sistema;

public partial class GestorColor : Node
{
    [Export]
    public Color ColorFondo { get; set; } = new Color(ConstantesColores.NEGRO_ROTO);

    [Export]
    public Color ColorParticulas { get; set; } = new Color(ConstantesColores.AMARILLO_PASTEL);

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");
    }
}
