using Godot;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.estadisticas;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas;

public partial class Global : Node
{
    public static Global Instancia { get; private set; }

    public GestorColor _GestorColor { get; private set; }
    public static GestorColor GestorColor => Global.Instancia._GestorColor;

    public GestorAudio _GestorAudio { get; private set; }
    public static GestorAudio GestorAudio => Global.Instancia._GestorAudio;

    public Global()
    {
        Ajustes.CargarAjustes();
        GestorEstadisticas.CargarEstadisticas();
        GestorLogros.CargarLogros();

        // Informar idioma.
        Idioma idioma = Ajustes.Idioma;
        GestorIdioma.CambiarIdioma(idioma);
    }

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        InicializarValoresEstaticos();

        // Mostramos colisiones.
        bool verColisiones = Ajustes.VerColisiones;
        GetTree().DebugCollisionsHint = verColisiones;
    }

    private void InicializarValoresEstaticos()
    {
        Global.Instancia = this;

        _GestorColor = GetNode<GestorColor>("GestorColor");
        _GestorAudio = GetNode<GestorAudio>("GestorAudio");
    }
}