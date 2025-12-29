using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.modelos;
using Primerjuego2D.escenas.modelos.controles;
using Primerjuego2D.escenas.modelos.interfaces;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class MenuPrincipal : Control
{
    private bool _modoNavegacionTeclado = true;

    private ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private ContenedorMenuPrincipal _ContenedorBotonesPrincipal;
    public ContenedorMenuPrincipal ContenedorMenuPrincipal => _ContenedorBotonesPrincipal ??= GetNode<ContenedorMenuPrincipal>("ContenedorMenuPrincipal");

    private ContenedorMenuAjustes _ContenedorMenuAjustes;
    public ContenedorMenuAjustes ContenedorMenuAjustes => _ContenedorMenuAjustes ??= GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");

    private ContenedorMenuEstadisticas _ContenedorMenuEstadisticas;
    public ContenedorMenuEstadisticas ContenedorMenuEstadisticas => _ContenedorMenuEstadisticas ??= GetNode<ContenedorMenuEstadisticas>("ContenedorMenuEstadisticas");

    private IEnumerable<Control> Menus =>
    [
    ContenedorMenuPrincipal,
    ContenedorMenuAjustes,
    ContenedorMenuEstadisticas
    ];

    public ContenedorMenu UltimoContenedorMostrado;

    private Label _LabelVersion;
    private Label LabelVersion => _LabelVersion ??= GetNode<Label>("LabelVersion");

    public Control _UltimoElementoConFocus;
    public Control UltimoElementoConFoco
    {
        get => _UltimoElementoConFocus;
        set
        {
            _UltimoElementoConFocus = value;
            LoggerJuego.Trace("Último elemento con focus actualizado a '" + value.Name + "'.");
        }
    }

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        LabelVersion.Text = "v" + Ajustes.Version;

        this.ContenedorMenuPrincipal.FocoElemento += InformarUltimoElementoConFoco;
        this.ContenedorMenuAjustes.FocoElemento += InformarUltimoElementoConFoco;
        this.ContenedorMenuEstadisticas.FocoElemento += InformarUltimoElementoConFoco;

        GrabFocusPrimerElemento(this.ContenedorMenuPrincipal);
    }

    public override void _Input(InputEvent @event)
    {
        DetectarMetodoDeEntrada(@event);
    }

    private void DetectarMetodoDeEntrada(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (!_modoNavegacionTeclado &&
                UtilidadesControles.IsActionPressed(@event, ConstantesAcciones.UP, ConstantesAcciones.RIGHT, ConstantesAcciones.DOWN, ConstantesAcciones.LEFT, ConstantesAcciones.ESCAPE))
            {
                LoggerJuego.Trace("Activamos la navegación por teclado.");
                ActivarNavegacionTeclado();
            }
        }
        else if (@event is InputEventMouse)
        {
            if (_modoNavegacionTeclado)
            {
                LoggerJuego.Trace("Desactivamos la navegación por teclado.");
                DesactivarNavegacionTeclado();
            }
        }
    }

    public void ActivarNavegacionTeclado()
    {
        if (!_modoNavegacionTeclado)
        {
            _modoNavegacionTeclado = true;

            this.ContenedorMenuPrincipal.ActivarNavegacionTeclado();
            this.ContenedorMenuAjustes.ActivarNavegacionTeclado();

            GrabFocusUltimoBotonConFoco();
        }
    }

    public void DesactivarNavegacionTeclado()
    {
        if (_modoNavegacionTeclado)
        {
            _modoNavegacionTeclado = false;

            this.ContenedorMenuPrincipal.DesactivarNavegacionTeclado();
            this.ContenedorMenuAjustes.DesactivarNavegacionTeclado();
        }
    }

    public void InformarUltimoElementoConFoco(Control ultimoElementoConFoco)
    {
        this.UltimoElementoConFoco = ultimoElementoConFoco;
    }

    public void GrabFocusUltimoBotonConFoco()
    {
        if (UltimoElementoConFoco == null)
            return;

        if (this.UltimoElementoConFoco is IFocusSilencioso elementoConFocusSilencioso)
            elementoConFocusSilencioso.GrabFocusSilencioso();
        else
            this.UltimoElementoConFoco.GrabFocus();
    }

    public void MostrarMenuPrincipal()
    {
        this.ContenedorMenuPrincipal.ActivarFocusBotones();

        MostrarMenu(this.ContenedorMenuPrincipal);
    }

    private void MostrarMenuAjustes()
    {
        this.ContenedorMenuPrincipal.DesactivarFocusBotones();

        MostrarMenu(this.ContenedorMenuAjustes);
    }

    private void MostrarMenuEstadisticas()
    {
        this.ContenedorMenuPrincipal.DesactivarFocusBotones();

        MostrarMenu(this.ContenedorMenuEstadisticas);
    }

    private void MostrarMenu(ContenedorMenu contenedorMenu)
    {
        foreach (var menu in Menus)
            menu.Visible = false;

        contenedorMenu.Visible = true;
        this.UltimoContenedorMostrado = contenedorMenu;

        GrabFocusPrimerElemento(contenedorMenu);
    }

    private void GrabFocusPrimerElemento(ContenedorMenu contenedorMenu)
    {
        Control elementoASeleccionar = contenedorMenu.ObtenerPrimerElemento();
        if (elementoASeleccionar == null) return;

        if (_modoNavegacionTeclado)
            if (elementoASeleccionar is IFocusSilencioso elementoConFocusSilencioso)
                elementoConFocusSilencioso.GrabFocusSilencioso();
            else
                elementoASeleccionar.GrabFocus();
        else
            this.UltimoElementoConFoco = elementoASeleccionar;
    }
}
