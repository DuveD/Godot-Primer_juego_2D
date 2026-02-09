using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.menuPrincipal.perfil;
using Primerjuego2D.escenas.ui;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPerfiles : ContenedorMenu
{
    private SlotPerfil _slotPerfil1;
    private SlotPerfil _slotPerfil2;
    private SlotPerfil _slotPerfil3;

    private List<SlotPerfil> slotPerfiles;

    private Control _controlPanelPerfiles;

    private ContenedorConfirmacion _contenedorConfirmacionSeleccionarPerfil;
    private SlotPerfil _slotPerfilSeleccionado;

    private ButtonPersonalizado _buttonAtras;

    private bool _ocultarBotonAtras = false;

    public override void _Ready()
    {
        base._Ready();

        _slotPerfil1 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil1");
        _slotPerfil1.Pressed += OnSlotPerfil1Pressed;
        _slotPerfil2 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil2");
        _slotPerfil2.Pressed += OnSlotPerfil2Pressed;
        _slotPerfil3 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil3");
        _slotPerfil3.Pressed += OnSlotPerfil3Pressed;

        slotPerfiles = [_slotPerfil1, _slotPerfil2, _slotPerfil3];

        _controlPanelPerfiles = GetNode<Control>("ControlPanelPerfiles");
        _contenedorConfirmacionSeleccionarPerfil = GetNode<ContenedorConfirmacion>("ContenedorConfirmacionSeleccionarPerfil");
        _buttonAtras = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

        CargarSlotsPerfiles();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void CargarSlotsPerfiles()
    {
        if (!String.IsNullOrWhiteSpace(Ajustes.IdSlotPerfil1))
        {
            CargarSlotPerfil(_slotPerfil1, Ajustes.IdSlotPerfil1);
        }

        if (!String.IsNullOrWhiteSpace(Ajustes.IdSlotPerfil2))
        {
            CargarSlotPerfil(_slotPerfil2, Ajustes.IdSlotPerfil2);
        }

        if (!String.IsNullOrWhiteSpace(Ajustes.IdSlotPerfil3))
        {
            CargarSlotPerfil(_slotPerfil3, Ajustes.IdSlotPerfil3);
        }
    }

    private void CargarSlotPerfil(SlotPerfil slotPerfil, string idSlotPerfil)
    {
        if (slotPerfil == null || String.IsNullOrWhiteSpace(idSlotPerfil))
            return;

        Perfil perfil = GestorPerfiles.CargarPerfil(idSlotPerfil);
        if (perfil == null)
            return;

        slotPerfil.Perfil = perfil;

        if (Global.PerfilActivo != null && Global.PerfilActivo.Id == perfil.Id)
            slotPerfil.SetSeleccionado(true);
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
                if (!_ocultarBotonAtras)
                {
                    UtilidadesNodos.PulsarBoton(this._buttonAtras);
                    AcceptEvent();
                }
            }
        }
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [_slotPerfil1, _slotPerfil2, _slotPerfil3, _buttonAtras];
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return _slotPerfil1;
    }

    private void OnSlotPerfil1Pressed() => OnSlotPerfilPressed(_slotPerfil1);
    private void OnSlotPerfil2Pressed() => OnSlotPerfilPressed(_slotPerfil2);
    private void OnSlotPerfil3Pressed() => OnSlotPerfilPressed(_slotPerfil3);


    public void OnSlotPerfilPressed(SlotPerfil slotPerfil)
    {
        if (!slotPerfil.Vacio && !slotPerfil.Seleccionado)
        {
            _slotPerfilSeleccionado = slotPerfil;
            _controlPanelPerfiles.Hide();
            _contenedorConfirmacionSeleccionarPerfil.Show(this.ModoNavegacionTeclado, true);
        }
    }

    public void OnSeleccionarPerfilConfirmar()
    {
        Perfil perfilSeleccionado = _slotPerfilSeleccionado.Perfil;
        CambiarPerfilActivo(perfilSeleccionado);

        _slotPerfilSeleccionado = null;
        _contenedorConfirmacionSeleccionarPerfil.Hide();
        _controlPanelPerfiles.Show();
        this.GrabFocusUltimoElementoConFoco();

        UtilidadesNodos.PulsarBoton(this._buttonAtras);
    }

    private void CambiarPerfilActivo(Perfil perfilSeleccionado)
    {
        Global.CambiarPerfilActivo(perfilSeleccionado);

        foreach (var slotPerfil in slotPerfiles.Where(sp => !sp.Vacio && sp.Seleccionado))
            slotPerfil.SetSeleccionado(false);

        _slotPerfilSeleccionado.SetSeleccionado(true);
    }


    public void OnSeleccionarPerfilCancelar()
    {
        _slotPerfilSeleccionado = null;
        _contenedorConfirmacionSeleccionarPerfil.Hide();
        _controlPanelPerfiles.Show();
        this.GrabFocusUltimoElementoConFoco();
    }

    public void Show(bool modoNavegacionTeclado, bool seleccionarPrimerElemento, bool ocultarBotonAtras = false)
    {
        base.Show(modoNavegacionTeclado, seleccionarPrimerElemento);

        _ocultarBotonAtras = ocultarBotonAtras;
        if (ocultarBotonAtras)
            this._buttonAtras.Hide();
    }
}
