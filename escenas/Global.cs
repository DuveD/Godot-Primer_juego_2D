using Godot;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.escenas.sistema.audio;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.estadisticas;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas;

public partial class Global : Node
{
    private static Global _instancia;

    private GestorColor _GestorColor;
    public static GestorColor GestorColor => Global._instancia._GestorColor;

    private GestorAudio _GestorAudio;
    public static GestorAudio GestorAudio => Global._instancia._GestorAudio;

    private GestorEfectosAudio _GestorEfectosAudio;
    public static GestorEfectosAudio GestorEfectosAudio => Global._instancia._GestorEfectosAudio;

    private Perfil _perfilActivoActiva;
    public static Perfil perfilActivo => Global._instancia._perfilActivoActiva;

    private string _nombreUltimoPerfil;
    public static string NombreUltimoPerfil => Global._instancia._nombreUltimoPerfil;

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

        Global._instancia = this;

        _GestorColor = GetNode<GestorColor>("GestorColor");
        _GestorAudio = GetNode<GestorAudio>("GestorAudio");
        _GestorEfectosAudio = GetNode<GestorEfectosAudio>("GestorEfectosAudio");

        // Mostramos colisiones.
        bool verColisiones = Ajustes.VerColisiones;
        GetTree().DebugCollisionsHint = verColisiones;
    }
}