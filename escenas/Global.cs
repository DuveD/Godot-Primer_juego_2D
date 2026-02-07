using System;
using Godot;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.escenas.sistema.audio;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
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

    private Perfil _perfilActivo;
    public static Perfil PerfilActivo => Global._instancia._perfilActivo;

    public Global()
    {
        Global._instancia = this;
    }

    public override void _Ready()
    {
        Ajustes.CargarAjustes();
        CargarPerfilActivo();

        // Informar idioma.
        Idioma idioma = Ajustes.Idioma;
        GestorIdioma.CambiarIdioma(idioma);

        _GestorColor = GetNode<GestorColor>("GestorColor");
        _GestorAudio = GetNode<GestorAudio>("GestorAudio");
        _GestorEfectosAudio = GetNode<GestorEfectosAudio>("GestorEfectosAudio");

        // Mostramos colisiones.
        bool verColisiones = Ajustes.VerColisiones;
        GetTree().DebugCollisionsHint = verColisiones;

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void CargarPerfilActivo()
    {
        if (!String.IsNullOrWhiteSpace(Ajustes.IdPerfilActivo))
        {
            Perfil perfilActivo = GestorPerfiles.CargarPerfil(Ajustes.IdPerfilActivo);
            if (perfilActivo != null)
            {
                _perfilActivo = perfilActivo;
            }
        }
    }

    public static void CambiarPerfilActivo(Perfil perfil)
    {
        if (perfil == null)
            throw new ArgumentNullException(nameof(perfil));

        Global._instancia._perfilActivo = perfil;
        Ajustes.IdPerfilActivo = perfil.Id;
    }

    public static void GuardarPerfilActivo()
    {
        if (Global.PerfilActivo != null)
            GestorPerfiles.GuardarPerfil(Global.PerfilActivo);
    }
}