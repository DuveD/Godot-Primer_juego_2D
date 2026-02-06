using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.modelos.interfaces;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.menu;

public abstract partial class ContenedorMenu : Container
{
    [Signal]
    public delegate void ModoNavegacionTecladoChangedEventHandler(bool modoNavegacionTeclado);

    private bool _ModoNavegacionTeclado;
    public bool ModoNavegacionTeclado
    {
        get => _ModoNavegacionTeclado;
        set
        {
            if (_ModoNavegacionTeclado == value)
                return;

            _ModoNavegacionTeclado = value;

            if (value)
                OnActivarNavegacionTeclado();
            else
                OnDesactivarNavegacionTeclado();

            EmitSignal(SignalName.ModoNavegacionTecladoChanged, value);
        }
    }

    protected List<Control> ElementosConFoco;

    public Control _UltimoElementoConFoco;
    public Control UltimoElementoConFoco
    {
        get => _UltimoElementoConFoco;
        set
        {
            _UltimoElementoConFoco = value;
            if (value != null)
                LoggerJuego.Trace($"Último elemento con focus actualizado a '{value.Name}'.");
            else
                LoggerJuego.Trace("Último elemento con focus limpiado.");
        }
    }

    public abstract Control ObtenerPrimerElementoConFoco();

    public abstract List<Control> ObtenerElementosConFoco();

    public override void _Ready()
    {
        CallDeferred(nameof(PostReady));
    }

    protected virtual void PostReady()
    {
        ConfigurarElementosConFoco();

        this.VisibilityChanged += OnVisibilityChanged;

        if (this.Visible & this.ModoNavegacionTeclado)
            GrabFocusPrimerElemento();
        else
            this._UltimoElementoConFoco = ObtenerPrimerElementoConFoco();
    }

    public void ConfigurarElementosConFoco()
    {
        this.ElementosConFoco = ObtenerElementosConFoco();

        foreach (var elementoConFoco in this.ElementosConFoco)
        {
            var elementoConFocoLocal = elementoConFoco;
            elementoConFocoLocal.FocusEntered += () =>
                InformarUltimoElementoConFoco(elementoConFocoLocal);
        }
    }

    public void InformarUltimoElementoConFoco(Control ultimoElementoConFoco)
    {
        this.UltimoElementoConFoco = ultimoElementoConFoco;
    }

    private void OnVisibilityChanged()
    {
        if (this.Visible)
        {
            ActivarFocusBotones();
            CallDeferred(nameof(GrabFocusUltimoElementoConFoco));
        }
        else
        {
            this.DesactivarFocusBotones();
        }
    }

    public void GrabFocusUltimoElementoConFoco()
    {
        if (!this.Visible || !this.ModoNavegacionTeclado)
            return;

        if (UltimoElementoConFoco == null)
            return;

        LoggerJuego.Trace("Cogemos el foco del último elemento con foco: '" + UltimoElementoConFoco.Name + "'.");

        if (this.UltimoElementoConFoco is IFocusSilencioso elementoConFocusSilencioso)
            elementoConFocusSilencioso.GrabFocusSilencioso();
        else
            this.UltimoElementoConFoco.GrabFocus();
    }

    private void GrabFocusPrimerElemento()
    {
        if (!this.Visible || !this.ModoNavegacionTeclado)
            return;

        Control elementoASeleccionar = ObtenerPrimerElementoConFoco();
        if (elementoASeleccionar == null) return;

        LoggerJuego.Trace("Cogemos el foco del primer elemento con foco: '" + elementoASeleccionar.Name + "'.");

        if (ModoNavegacionTeclado)
            if (elementoASeleccionar is IFocusSilencioso elementoConFocusSilencioso)
                elementoConFocusSilencioso.GrabFocusSilencioso();
            else
                elementoASeleccionar.GrabFocus();
        else
            this.UltimoElementoConFoco = elementoASeleccionar;
    }

    public void ActivarFocusBotones()
    {
        if (ElementosConFoco == null)
            return;

        LoggerJuego.Trace("Activamos el focus de los botones del contenedor.");

        foreach (var elementoConFoco in this.ElementosConFoco)
        {
            elementoConFoco.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
            elementoConFoco.FocusMode = FocusModeEnum.All;      // Aceptamos teclado
        }
    }

    public void DesactivarFocusBotones()
    {
        if (ElementosConFoco == null)
            return;

        LoggerJuego.Trace("Desactivamos el focus de los botones del contenedor.");

        foreach (var elementoConFoco in ElementosConFoco)
        {
            elementoConFoco.MouseFilter = MouseFilterEnum.Ignore; // Ignora clicks
            elementoConFoco.FocusMode = FocusModeEnum.None;       // Ignora teclado
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (this.Visible)
            DetectarMetodoDeEntrada(@event);
    }

    private void DetectarMetodoDeEntrada(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (!ModoNavegacionTeclado)
            {
                this.ModoNavegacionTeclado = true;
                AcceptEvent();
            }
        }
        else if (@event is InputEventMouse)
        {
            if (ModoNavegacionTeclado)
            {
                this.ModoNavegacionTeclado = false;
            }
        }
    }

    private void OnActivarNavegacionTeclado()
    {
        if (ElementosConFoco == null)
            return;

        LoggerJuego.Trace("Activamos la navegación por teclado.");

        foreach (var elementoConFoco in this.ElementosConFoco)
        {
            elementoConFoco.FocusMode = FocusModeEnum.All;      // Aceptamos teclado
        }

        CallDeferred(nameof(GrabFocusUltimoElementoConFoco));
    }

    private void OnDesactivarNavegacionTeclado()
    {
        if (ElementosConFoco == null)
            return;

        LoggerJuego.Trace("Desactivamos la navegación por teclado.");

        foreach (var elementoConFoco in this.ElementosConFoco)
        {
            elementoConFoco.FocusMode = FocusModeEnum.None;       // Ignora teclado
        }
    }

    public void Show(bool modoNavegacionTeclado, bool seleccionarPrimerElemento)
    {
        this._ModoNavegacionTeclado = modoNavegacionTeclado;

        if (seleccionarPrimerElemento)
            _UltimoElementoConFoco = ObtenerPrimerElementoConFoco();

        this.Visible = true;
    }
}