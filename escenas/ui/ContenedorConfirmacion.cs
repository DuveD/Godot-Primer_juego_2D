using System;
using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui;

public partial class ContenedorConfirmacion : ContenedorMenu
{
    public ButtonPersonalizado ButtonConfirmar;
    public ButtonPersonalizado ButtonCancelar;

    public override void _Ready()
    {
        base._Ready();

        ButtonConfirmar = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonConfirmar");
        ButtonCancelar = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonCancelar");

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            if (this.ModoNavegacionTeclado)
            {
                UtilidadesNodos.PulsarBoton(ButtonCancelar);
                AcceptEvent();
            }
        }
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [ButtonConfirmar, ButtonCancelar];
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return ButtonCancelar;
    }

}
