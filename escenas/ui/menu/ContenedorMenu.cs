using System.Collections.Generic;
using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.menu;

public abstract partial class ContenedorMenu : CenterContainer
{
    [Signal]
    public delegate void FocoElementoEventHandler(Control control);

    protected List<Control> ElementosConFoco;

    public abstract Control ObtenerPrimerElementoConFoco();

    public abstract List<Control> ObtenerElementosConFoco();

    public override void _Ready()
    {
        ConfigurarElementosConFoco();
    }

    public void ConfigurarElementosConFoco()
    {
        LoggerJuego.Trace("Configuramos el focus de los elementos del menú ajustes.");

        this.ElementosConFoco = ObtenerElementosConFoco();

        foreach (var elementoConFoco in this.ElementosConFoco)
        {
            var elementoConFocoLocal = elementoConFoco;
            elementoConFocoLocal.FocusEntered += () =>
                EmitSignal(SignalName.FocoElemento, elementoConFocoLocal);
        }
    }

    public void ActivarNavegacionTeclado()
    {
        LoggerJuego.Trace("Activamos la navegación por teclado.");

        foreach (var elementoConFoco in this.ElementosConFoco)
            elementoConFoco.FocusMode = FocusModeEnum.All;
    }

    public void DesactivarNavegacionTeclado()
    {
        LoggerJuego.Trace("Desactivamos la navegación por teclado.");

        foreach (var elementoConFoco in this.ElementosConFoco)
            elementoConFoco.FocusMode = FocusModeEnum.None;
    }
}