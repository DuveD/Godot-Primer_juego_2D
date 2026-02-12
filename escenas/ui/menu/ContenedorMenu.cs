using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.modelos.interfaces;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.menu;

public abstract partial class ContenedorMenu : Container
{
    public bool EjecutarAccion => IsInstanceValid(this) && this.IsVisibleInTree();

    public Control _UltimoElementoConFoco;
    public Control UltimoElementoConFoco
    {
        get => _UltimoElementoConFoco;
        set
        {
            _UltimoElementoConFoco = value;
            if (value != null)
                LoggerJuego.Trace($"({this.Name}) Último elemento con focus actualizado a '{value.Name}'.");
            else
                LoggerJuego.Trace($"({this.Name}) Último elemento con focus limpiado.");
        }
    }

    public abstract Control ObtenerPrimerElementoConFoco();

    public abstract List<Control> ObtenerElementosConFoco();

    public override void _Ready()
    {
        CallDeferred(nameof(PostReady));
    }

    private void OnNavegacionTecladoCambiado(bool navegacionTeclado)
    {
        if (!EjecutarAccion)
            return;

        if (navegacionTeclado)
            OnActivarNavegacionTeclado();
        else
            OnDesactivarNavegacionTeclado();
    }

    // PostReady: se ejecuta tras layout inicial para evitar problemas de focus.
    protected virtual void PostReady()
    {
        ConfigurarElementosConFoco();

        this.VisibilityChanged += OnVisibilityChanged;

        if (this.IsVisibleInTree())
            CallDeferred(nameof(OnMenuVisible));
    }

    public void OnVisibilityChanged()
    {
        if (!IsInstanceValid(this))
            return;

        if (this.Visible)
        {
            OnMenuVisible();
        }
        else
        {
            OnMenuInvisible();
        }
    }

    public virtual void OnMenuVisible()
    {
        ActivarFocusElementosConFoco();
        CallDeferred(nameof(GrabFocusUltimoElementoConFoco));
        Global.Instancia.OnNavegacionTecladoCambiado += OnNavegacionTecladoCambiado;
    }

    public virtual void OnMenuInvisible()
    {
        this.DesactivarFocusElementosConFoco();
        Global.Instancia.OnNavegacionTecladoCambiado -= OnNavegacionTecladoCambiado;
    }

    public void GrabFocusUltimoElementoConFoco()
    {
        if (!EjecutarAccion || !Global.NavegacionTeclado)
            return;

        if (UltimoElementoConFoco == null || !IsInstanceValid(UltimoElementoConFoco))
        {
            LoggerJuego.Trace($"({this.Name}) Último elemento nulo. Seleccionamos el primer elemento.");
            GrabFocusPrimerElemento();
            return;
        }

        bool desactivado = UltimoElementoConFoco is BaseButton button && button.Disabled;
        if (desactivado)
        {
            LoggerJuego.Trace($"({this.Name}) Último elemento con foco está desactivado. Seleccionamos el primer elemento.");
            GrabFocusPrimerElemento();
            return;
        }

        if (UltimoElementoConFoco.FocusMode == FocusModeEnum.None)
            return;

        LoggerJuego.Trace($"({this.Name}) Cogemos el foco del último elemento con foco: '" + UltimoElementoConFoco.Name + "'.");

        if (this.UltimoElementoConFoco is IFocusSilencioso elementoConFocusSilencioso)
            elementoConFocusSilencioso.GrabFocusSilencioso();
        else
            this.UltimoElementoConFoco.GrabFocus();
    }

    private void GrabFocusPrimerElemento()
    {
        if (!EjecutarAccion || !Global.NavegacionTeclado)
            return;

        Control elementoASeleccionar = ObtenerPrimerElementoConFoco();
        if (elementoASeleccionar == null || !IsInstanceValid(elementoASeleccionar))
            return;

        LoggerJuego.Trace($"({this.Name}) Cogemos el foco del primer elemento con foco: '" + elementoASeleccionar.Name + "'.");

        if (elementoASeleccionar is IFocusSilencioso elementoConFocusSilencioso)
            elementoConFocusSilencioso.GrabFocusSilencioso();
        else
            elementoASeleccionar.GrabFocus();
    }

    private void ConfigurarElementosConFoco()
    {
        if (!IsInstanceValid(this))
            return;

        var elementosConFoco = ObtenerElementosConFoco();
        if (elementosConFoco == null)
            return;

        LoggerJuego.Trace($"({this.Name}) Configuramos el focus de los elementos con foco del contenedor.");

        foreach (var elementoConFoco in elementosConFoco)
        {
            if (elementoConFoco == null || !IsInstanceValid(elementoConFoco))
                continue;

            elementoConFoco.FocusEntered += OnElementoConFocoFocusEntered;

            if (elementoConFoco is BaseButton buttonConFoco)
            {
                if (buttonConFoco.Disabled)
                {
                    elementoConFoco.MouseFilter = MouseFilterEnum.Ignore; // Ignora clicks
                    elementoConFoco.FocusMode = FocusModeEnum.None; // Ignora teclado
                }
                else
                {
                    elementoConFoco.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
                    elementoConFoco.FocusMode = FocusModeEnum.All; // Aceptamos teclado
                }
            }
            else
            {
                elementoConFoco.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
                elementoConFoco.FocusMode = FocusModeEnum.All; // Aceptamos teclado
            }
        }
    }

    public void OnElementoConFocoFocusEntered()
    {
        this.UltimoElementoConFoco = GetViewport().GuiGetFocusOwner();
    }

    public void ActivarFocusElementosConFoco()
    {
        if (!EjecutarAccion)
            return;

        var elementosConFoco = ObtenerElementosConFoco();
        if (elementosConFoco == null)
            return;

        LoggerJuego.Trace($"({this.Name}) Activamos el focus de los elementos con foco del contenedor.");

        foreach (var elementoConFoco in elementosConFoco)
        {
            if (elementoConFoco is BaseButton buttonConFoco)
            {
                if (!buttonConFoco.Disabled)
                {
                    elementoConFoco.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
                    elementoConFoco.FocusMode = FocusModeEnum.All; // Aceptamos teclado
                }
            }
            else
            {
                elementoConFoco.MouseFilter = MouseFilterEnum.Pass; // Aceptamos clicks
                elementoConFoco.FocusMode = FocusModeEnum.All; // Aceptamos teclado
            }
        }
    }

    public void DesactivarFocusElementosConFoco()
    {
        if (!EjecutarAccion)
            return;

        var elementosConFoco = ObtenerElementosConFoco();
        if (elementosConFoco == null)
            return;

        LoggerJuego.Trace($"({this.Name}) Desactivamos el focus de los elementos con foco del contenedor.");

        foreach (var elementoConFoco in elementosConFoco)
        {
            if (elementoConFoco is BaseButton buttonConFoco)
            {
                if (buttonConFoco.Disabled)
                {
                    elementoConFoco.MouseFilter = MouseFilterEnum.Ignore; // Ignora clicks
                    elementoConFoco.FocusMode = FocusModeEnum.None; // Ignora teclado
                }
            }
            else
            {
                elementoConFoco.MouseFilter = MouseFilterEnum.Ignore; // Ignora clicks
                elementoConFoco.FocusMode = FocusModeEnum.None; // Ignora teclado
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (this.IsVisibleInTree())
            DetectarMetodoDeEntrada(@event);
    }

    private void DetectarMetodoDeEntrada(InputEvent @event)
    {
        // if (@event is InputEventKey keyEvent && UtilidadesControles.IsActionPressed(keyEvent, ConstantesAcciones.ACCIONES_NAVEGACION_MENU))
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
        {
            if (!Global.NavegacionTeclado)
            {
                Global.NavegacionTeclado = true;
                AcceptEvent();
            }
        }
        else if (@event is InputEventMouse)
        {
            if (Global.NavegacionTeclado)
            {
                Global.NavegacionTeclado = false;
            }
        }
    }

    private void OnActivarNavegacionTeclado()
    {
        var elementosConFoco = ObtenerElementosConFoco();
        if (elementosConFoco == null)
            return;

        LoggerJuego.Trace($"({this.Name}) Activamos la navegación por teclado.");

        foreach (var elementoConFoco in elementosConFoco)
        {
            if (elementoConFoco is BaseButton button && button.Disabled)
                continue;

            elementoConFoco.FocusMode = FocusModeEnum.All;
        }

        CallDeferred(nameof(GrabFocusUltimoElementoConFoco));
    }

    private void OnDesactivarNavegacionTeclado()
    {
        var elementosConFoco = ObtenerElementosConFoco();
        if (elementosConFoco == null)
            return;

        LoggerJuego.Trace($"({this.Name}) Desactivamos la navegación por teclado.");

        foreach (var elementoConFoco in elementosConFoco)
        {
            elementoConFoco.FocusMode = FocusModeEnum.None;
        }
    }

    public void Show(bool seleccionarPrimerElemento)
    {
        if (seleccionarPrimerElemento)
            _UltimoElementoConFoco = ObtenerPrimerElementoConFoco();

        this.Visible = true;
    }
}