using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.modelos.interfaces;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.menu;

public abstract partial class ContenedorMenu : CenterContainer
{
    private bool _modoNavegacionTeclado;
    protected bool ModoNavegacionTeclado
    {
        get => _modoNavegacionTeclado;
        set
        {
            if (_modoNavegacionTeclado == value)
                return;

            _modoNavegacionTeclado = value;

            if (value)
                OnActivarNavegacionTeclado();
            else
                OnDesactivarNavegacionTeclado();
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
        LoggerJuego.Trace(this.Name + " Ready.");

        ConfigurarElementosConFoco();

        ModoNavegacionTeclado = true;
        this.VisibilityChanged += OnVisibilityChanged;

        if (this.Visible)
            CallDeferred(nameof(GrabFocusPrimerElemento));
    }

    public void ConfigurarElementosConFoco()
    {
        LoggerJuego.Trace("Configuramos el focus de los elementos del menú ajustes.");

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
            this.ActivarFocusBotones();
            CallDeferred(nameof(GrabFocusPrimerElemento));
        }
        else
        {
            this.DesactivarFocusBotones();
        }
    }

    private void GrabFocusPrimerElemento()
    {
        Control elementoASeleccionar = ObtenerPrimerElementoConFoco();
        if (elementoASeleccionar == null) return;

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
        DetectarMetodoDeEntrada(@event);
    }

    private void DetectarMetodoDeEntrada(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (!ModoNavegacionTeclado &&
                UtilidadesControles.IsActionPressed(@event, ConstantesAcciones.UP, ConstantesAcciones.RIGHT, ConstantesAcciones.DOWN, ConstantesAcciones.LEFT, ConstantesAcciones.ESCAPE))
            {
                this.ModoNavegacionTeclado = true;
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

        GrabFocusUltimoElementoConFoco();
    }

    private void GrabFocusUltimoElementoConFoco()
    {
        if (UltimoElementoConFoco == null)
            return;

        if (!this.Visible)
            return;

        if (this.UltimoElementoConFoco is IFocusSilencioso elementoConFocusSilencioso)
            elementoConFocusSilencioso.GrabFocusSilencioso();
        else
            this.UltimoElementoConFoco.GrabFocus();
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
}