using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.menuPrincipal.botones;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPrincipal : ContenedorMenu
{
    private bool _menuDesactivado = false;

    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    private ButtonEmpezarPartida _ButtonEmpezarPartida;
    public ButtonEmpezarPartida ButtonEmpezarPartida => _ButtonEmpezarPartida ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonEmpezarPartida>(this, "ButtonEmpezarPartida");

    private ButtonAjustes _ButtonAjustes;
    public ButtonAjustes ButtonAjustes => _ButtonAjustes ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonAjustes>(this, "ButtonAjustes");

    private ButtonEstadisticas _ButtonEstadisticas;
    public ButtonEstadisticas ButtonEstadisticas => _ButtonEstadisticas ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonEstadisticas>(this, "ButtonEstadisticas");

    private ButtonSalir _ButtonSalir;
    public ButtonSalir ButtonSalir => _ButtonSalir ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonSalir>(this, "ButtonSalir");

    private CanvasLayer _CrtLayer;
    private CanvasLayer CrtLayer => _CrtLayer ??= GetNode<CanvasLayer>("../CRTShutdown");

    private AnimationPlayer _AnimPlayer;
    private AnimationPlayer AnimPlayer => _AnimPlayer ??= CrtLayer.GetNode<AnimationPlayer>("AnimationPlayer");

    public ButtonPersonalizado UltimoBotonPulsado;

    public override void _Ready()
    {
        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");

        ConfigurarBotonesMenu();
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [.. UtilidadesNodos.ObtenerNodosDeTipo<ButtonPersonalizado>(this).Cast<Control>()];
    }

    private void ConfigurarBotonesMenu()
    {
        LoggerJuego.Trace("Configuramos el focus de los botones del menú.");

        foreach (var boton in this.ElementosConFoco.OfType<ButtonPersonalizado>().ToList())
            boton.Pressed += () => this.UltimoBotonPulsado = boton;
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return this.UltimoBotonPulsado ?? ButtonEmpezarPartida;
    }

    public void ActivarFocusBotones()
    {
        LoggerJuego.Trace("Activamos el focus de los botones del menú.");

        _menuDesactivado = false;
        foreach (var elementoConFoco in this.ElementosConFoco)
        {
            elementoConFoco.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
            elementoConFoco.FocusMode = FocusModeEnum.All;      // Aceptamos teclado
        }
    }

    public void DesactivarFocusBotones()
    {
        LoggerJuego.Trace("Desactivamos el focus de los botones del menú.");

        _menuDesactivado = true;
        foreach (var elementoConFoco in ElementosConFoco)
        {
            elementoConFoco.MouseFilter = MouseFilterEnum.Ignore; // Ignora clicks
            elementoConFoco.FocusMode = FocusModeEnum.None;       // Ignora teclado
        }
    }

    private void OnButtonEmpezarPartidaPressedAnimationEnd()
    {
        LoggerJuego.Trace("Botón 'ButtonEmpezarPartida' pulsado.");

        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
    }

    private async void OnButtonSalirPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonSalir' pulsado.");

        Global.GestorAudio.PausarMusica(0.5f);

        CrtLayer.Visible = true;
        AnimPlayer.Play("ApagarTV");

        await ToSignal(AnimPlayer, "animation_finished");
        await Task.Delay(300);

        this.GetTree().Quit();
    }
}
