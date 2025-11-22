using Godot;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas;

public partial class Global : Node
{
    [Export]
    public GestorColor _GestorColor { get; set; } // Nodo de la escena
    public static GestorColor GestorColor { get; private set; }

    public override void _Ready()
    {
        Logger.Trace(this.Name + " Ready.");

        InicializarValoresEstaticos();
    }

    private void InicializarValoresEstaticos()
    {
        GestorColor ??= _GestorColor;
    }
}