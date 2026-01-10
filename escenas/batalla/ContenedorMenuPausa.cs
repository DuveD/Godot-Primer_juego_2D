using System;
using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;

namespace Primerjuego2D.escenas.batalla;

public partial class ContenedorMenuPausa : ContenedorMenu
{
    private ButtonPersonalizado _ButtonRenaudar;
    public ButtonPersonalizado ButtonRenaudar => _ButtonRenaudar ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonRenaudar");

    private ButtonPersonalizado _ButtonAjustes;
    public ButtonPersonalizado ButtonAjustes => _ButtonAjustes ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAjustes");

    private ButtonPersonalizado _ButtonSalir;
    public ButtonPersonalizado ButtonSalir => _ButtonSalir ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonSalir");

    public override void _UnhandledInput(InputEvent @event)
    {
        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            if (this.ModoNavegacionTeclado)
            {
                UtilidadesNodos.PulsarBoton(ButtonRenaudar);
                AcceptEvent();
            }
        }
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [ButtonRenaudar, ButtonAjustes, ButtonSalir];
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return ButtonRenaudar;
    }
}