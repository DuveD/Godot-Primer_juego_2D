using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.modelos;
using Primerjuego2D.escenas.modelos.controles;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPrincipal : ContenedorMenu
{
    private bool _menuDesactivado = false;

    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    private List<ButtonPersonalizado> _BotonesMenu;
    private List<ButtonPersonalizado> BotonesMenu => _BotonesMenu ??= UtilidadesNodos.ObtenerNodosDeTipo<ButtonPersonalizado>(this);

    private ButtonEmpezarPartida _ButtonEmpezarPartida;
    public ButtonEmpezarPartida ButtonEmpezarPartida => _ButtonEmpezarPartida ??= BotonesMenu.OfType<ButtonEmpezarPartida>().FirstOrDefault();

    private ButtonAjustes _ButtonAjustes;
    public ButtonAjustes ButtonAjustes => _ButtonAjustes ??= BotonesMenu.OfType<ButtonAjustes>().FirstOrDefault();

    private ButtonEstadisticas _ButtonEstadisticas;
    public ButtonEstadisticas ButtonEstadisticas => _ButtonEstadisticas ??= BotonesMenu.OfType<ButtonEstadisticas>().FirstOrDefault();

    private ButtonSalir _ButtonSalir;
    public ButtonSalir ButtonSalir => _ButtonSalir ??= BotonesMenu.OfType<ButtonSalir>().FirstOrDefault();

    private CanvasLayer _CrtLayer;
    private CanvasLayer CrtLayer => _CrtLayer ??= GetNode<CanvasLayer>("../CRTShutdown");

    private AnimationPlayer _AnimPlayer;
    private AnimationPlayer AnimPlayer => _AnimPlayer ??= CrtLayer.GetNode<AnimationPlayer>("AnimationPlayer");

    public ButtonPersonalizado UltimoBotonPulsado;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        ConfigurarFocusBotones();
    }

    private void ConfigurarFocusBotones()
    {
        LoggerJuego.Trace("Configuramos el focus de los botones del menú.");

        foreach (var boton in BotonesMenu)
        {
            boton.FocusEntered += () => EmitSignal(SignalName.FocoElemento, boton);
            boton.Pressed += () => this.UltimoBotonPulsado = boton;
        }
    }

    public override Control ObtenerPrimerElemento()
    {
        return this.UltimoBotonPulsado ?? ButtonEmpezarPartida;
    }

    public void ActivarFocusBotones()
    {
        LoggerJuego.Trace("Activamos el focus de los botones del menú.");

        _menuDesactivado = false;
        foreach (var boton in BotonesMenu)
        {
            boton.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
            boton.FocusMode = FocusModeEnum.All;      // Aceptamos teclado
        }
    }

    public void DesactivarFocusBotones()
    {
        LoggerJuego.Trace("Desactivamos el focus de los botones del menú.");

        _menuDesactivado = true;
        foreach (var boton in BotonesMenu)
        {
            boton.MouseFilter = MouseFilterEnum.Ignore; // Ignora clicks
            boton.FocusMode = FocusModeEnum.None;       // Ignora teclado
        }
    }

    public void ActivarNavegacionTeclado()
    {
        LoggerJuego.Trace("Activamos la navegación por teclado.");

        foreach (var boton in BotonesMenu)
            boton.FocusMode = FocusModeEnum.All;
    }

    public void DesactivarNavegacionTeclado()
    {
        LoggerJuego.Trace("Desactivamos la navegación por teclado.");

        foreach (var boton in BotonesMenu)
            boton.FocusMode = FocusModeEnum.None;
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
