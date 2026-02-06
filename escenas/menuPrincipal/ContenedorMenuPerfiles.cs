using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.menuPrincipal.perfil;
using Primerjuego2D.escenas.ui;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPerfiles : ContenedorMenu
{
    private SlotPerfil _slotPerfil1;
    private SlotPerfil _slotPerfil2;
    private SlotPerfil _slotPerfil3;

    private Control _controlPanelPerfiles;

    private ContenedorConfirmacion _contenedorConfirmacionSeleccionarPerfil;
    private SlotPerfil _slotPerfilSeleccionado;

    private ButtonPersonalizado _buttonAtras;

    public override void _Ready()
    {
        base._Ready();

        _slotPerfil1 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil1");
        _slotPerfil1.Pressed += () => OnSlotPerfilPressed(_slotPerfil1);
        _slotPerfil2 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil2");
        _slotPerfil2.Pressed += () => OnSlotPerfilPressed(_slotPerfil2);
        _slotPerfil3 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil3");
        _slotPerfil3.Pressed += () => OnSlotPerfilPressed(_slotPerfil3);
        _controlPanelPerfiles = GetNode<Control>("ControlPanelPerfiles");
        _contenedorConfirmacionSeleccionarPerfil = GetNode<ContenedorConfirmacion>("ContenedorConfirmacionSeleccionarPerfil");
        _buttonAtras = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

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
                UtilidadesNodos.PulsarBoton(this._buttonAtras);
                AcceptEvent();
            }
        }
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [_slotPerfil1, _slotPerfil2, _slotPerfil3, _buttonAtras];
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return _buttonAtras;
    }

    public void OnSlotPerfilPressed(SlotPerfil slotPerfil)
    {
        _slotPerfilSeleccionado = slotPerfil;
        _controlPanelPerfiles.Hide();
        _contenedorConfirmacionSeleccionarPerfil.Show(this.ModoNavegacionTeclado, true);
    }

    public void OnSeleccionarPerfilConfirmar()
    {
        _slotPerfilSeleccionado = null;
        _contenedorConfirmacionSeleccionarPerfil.Hide();
        _controlPanelPerfiles.Show();
        this.GrabFocusUltimoElementoConFoco();
    }

    public void OnSeleccionarPerfilCancelar()
    {
        _slotPerfilSeleccionado = null;
        _contenedorConfirmacionSeleccionarPerfil.Hide();
        _controlPanelPerfiles.Show();
        this.GrabFocusUltimoElementoConFoco();
    }
}
