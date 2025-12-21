using Godot;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.modelos.interfaces;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class MenuPrincipal : Control
{
    private bool _opcionPulsada = false;

    private bool _navegacionPorTeclado = true;

    private ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private ContenedorMenuPrincipal _ContenedorBotonesPrincipal;
    public ContenedorMenuPrincipal ContenedorMenuPrincipal => _ContenedorBotonesPrincipal ??= GetNode<ContenedorMenuPrincipal>("ContenedorMenuPrincipal");

    private ContenedorMenuAjustes _ContenedorMenuAjustes;
    public ContenedorMenuAjustes ContenedorMenuAjustes => _ContenedorMenuAjustes ??= GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");

    private Label _LabelVersion;
    private Label LabelVersion => _LabelVersion ??= GetNode<Label>("LabelVersion");

    public Control _UltimoElementoConFocus;
    public Control UltimoElementoConFocus
    {
        get => _UltimoElementoConFocus;
        set
        {
            _UltimoElementoConFocus = value;
            LoggerJuego.Trace("Último elemento con focus actualizado a: " + value.Name);
        }
    }

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        LabelVersion.Text = "v" + Ajustes.Version;

        GrabFocusPrimerElemento();
    }

    private void GrabFocusPrimerElemento()
    {
        if (this.ContenedorMenuPrincipal.Visible)
        {
            this.ContenedorMenuPrincipal.ButtonEmpezarPartida.GrabFocusSilencioso();
        }
        else
        {
            this.ContenedorMenuAjustes.ControlVolumenGeneral.SliderVolumen.GrabFocusSilencioso();
        }
    }

    public override void _Input(InputEvent @event)
    {
        CambioMetodoImput(@event);
    }

    private void CambioMetodoImput(InputEvent @event)
    {
        if (_opcionPulsada)
        {
            LoggerJuego.Trace("EL menú está desactivado.");
            // Ignora cualquier input
            return;
        }

        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (!_navegacionPorTeclado &&
                UtilidadesControles.IsActionPressed(@event, ConstantesAcciones.UP, ConstantesAcciones.RIGHT, ConstantesAcciones.DOWN, ConstantesAcciones.LEFT))
            {
                LoggerJuego.Trace("Activamos la navegación por teclado.");
                ActivarNavegacionTeclado();
            }
        }
        else if (@event is InputEventMouse)
        {
            if (_navegacionPorTeclado)
            {
                LoggerJuego.Trace("Desactivamos la navegación por teclado.");
                DesactivarNavegacionTeclado();
            }
        }
    }

    public void ActivarNavegacionTeclado()
    {
        if (!_navegacionPorTeclado)
        {
            _navegacionPorTeclado = true;

            this.ContenedorMenuPrincipal.ActivarNavegacionTeclado();
            this.ContenedorMenuAjustes.ActivarNavegacionTeclado();

            GrabFocusUltimoBotonConFocus();
        }
    }

    public void DesactivarNavegacionTeclado()
    {
        if (_navegacionPorTeclado)
        {
            _navegacionPorTeclado = false;

            this.ContenedorMenuPrincipal.DesactivarNavegacionTeclado();
            this.ContenedorMenuAjustes.DesactivarNavegacionTeclado();
        }
    }

    public void GrabFocusUltimoBotonConFocus()
    {
        if (this.UltimoElementoConFocus is IFocusSilencioso elementoConFocusSilencioso)
        {
            elementoConFocusSilencioso.GrabFocusSilencioso();
        }
        else
        {
            this.UltimoElementoConFocus.GrabFocus();
        }
    }

    private void MostrarContenedorMenuAjustes()
    {
        LoggerJuego.Trace("Botón 'ButtonAjustes' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");

        this.ContenedorMenuPrincipal.Visible = false;
        this.ContenedorMenuAjustes.Visible = true;

        if (_navegacionPorTeclado)
        {
            GrabFocusPrimerElemento();
        }
        else
        {
            this.UltimoElementoConFocus = this.ContenedorMenuAjustes.ControlVolumenGeneral.SliderVolumen;
        }
    }

    public void MostrarContenedorMenuprincipal()
    {
        LoggerJuego.Trace("Botón Ajustes 'Atrás' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");

        this.ContenedorMenuAjustes.Visible = false;
        this.ContenedorMenuPrincipal.Visible = true;

        this.ContenedorMenuPrincipal.ActivarFocusBotones();

        if (_navegacionPorTeclado)
        {
            this.ContenedorMenuPrincipal.ButtonAjustes.GrabFocusSilencioso();
        }
        else
        {
            this.UltimoElementoConFocus = this.ContenedorMenuPrincipal.ButtonAjustes;
        }
    }
}
