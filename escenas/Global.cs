using System;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.escenas.sistema.audio;
using Primerjuego2D.escenas.ui.overlays;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas;

public partial class Global : Node
{
    [Signal]
    public delegate void OnCambioPerfilActivoEventHandler();

    [Signal]
    public delegate void OnNavegacionTecladoCambiadoEventHandler(bool nuevoValor);

    public static bool NavegacionTeclado
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                Instancia.EmitSignal(SignalName.OnNavegacionTecladoCambiado, value);
            }
        }
    }

    public static Global Instancia;

    private GestorColor _GestorColor;
    public static GestorColor GestorColor => Global.Instancia._GestorColor;

    private GestorAudio _GestorAudio;
    public static GestorAudio GestorAudio => Global.Instancia._GestorAudio;

    private GestorEfectosAudio _GestorEfectosAudio;
    public static GestorEfectosAudio GestorEfectosAudio => Global.Instancia._GestorEfectosAudio;

    private Perfil _perfilActivo;
    public static Perfil PerfilActivo => Global.Instancia._perfilActivo;

    private IndicadorCarga _indicadorCarga;
    public static IndicadorCarga IndicadorCarga => Global.Instancia._indicadorCarga;

    private IndicadorGuardado _indicadorGuardado;
    public static IndicadorGuardado IndicadorGuardado => Global.Instancia._indicadorGuardado;

    public Global()
    {
        Global.Instancia = this;
        Ajustes.CargarAjustes();
    }

    public override void _Ready()
    {
        // Informar idioma.
        Idioma idioma = Ajustes.Idioma;
        GestorIdioma.CambiarIdioma(idioma);

        _GestorColor = GetNode<GestorColor>("GestorColor");
        _GestorAudio = GetNode<GestorAudio>("GestorAudio");
        _GestorEfectosAudio = GetNode<GestorEfectosAudio>("GestorEfectosAudio");
        _indicadorCarga = GetNode<IndicadorCarga>("IndicadorCarga");
        _indicadorGuardado = GetNode<IndicadorGuardado>("IndicadorGuardado");

        // Mostramos colisiones.
        bool verColisiones = Ajustes.VerColisiones;
        GetTree().DebugCollisionsHint = verColisiones;

        CargarPerfilActivo();

        NavegacionTeclado = false;

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private async void CargarPerfilActivo()
    {
        if (String.IsNullOrWhiteSpace(Ajustes.IdPerfilActivo))
        {
            LoggerJuego.Info("No hay perfil activo configurado en ajustes.");
            return;
        }

        IndicadorCarga.Mostrar();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        Perfil perfilActivo = await Task.Run(() => GestorPerfiles.CargarPerfil(Ajustes.IdPerfilActivo));

        if (perfilActivo != null)
        {
            CambiarPerfilActivo(perfilActivo);
        }
        else
        {
            LoggerJuego.Warn($"No se pudo cargar el perfil ({Ajustes.IdPerfilActivo}) activo configurado en ajustes.");
        }

        IndicadorCarga.Esconder();
    }

    public static void CambiarPerfilActivo(Perfil perfil)
    {
        Global.Instancia._perfilActivo = perfil;
        Ajustes.IdPerfilActivo = perfil?.Id;

        Global.Instancia.EmitSignal(SignalName.OnCambioPerfilActivo);
    }

    public static async void GuardarPerfilActivo()
    {
        if (Global.PerfilActivo == null)
            return;

        IndicadorGuardado.Mostrar();

        await Global.Instancia.ToSignal(Global.Instancia.GetTree(), SceneTree.SignalName.ProcessFrame);

        await Task.Run(() => GestorPerfiles.GuardarPerfil(Global.PerfilActivo));

        IndicadorGuardado.Esconder();
    }
}