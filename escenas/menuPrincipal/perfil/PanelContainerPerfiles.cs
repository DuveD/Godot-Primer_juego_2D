using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.ui;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal.perfil;

public partial class PanelContainerPerfiles : ContenedorMenu
{
    //[Signal]
    //public delegate void OnSlotPerfilNoActivoPressedEventHandler(SlotPerfil slotPerfilSeleccionado);

    [Signal]
    public delegate void OnSlotVacioPressedEventHandler(SlotPerfil slotPerfilVacioSeleccionado);

    [Signal]
    public delegate void OnCrearPrimerPerfilEventHandler();

    private SlotPerfil _slotPerfil1;
    private SlotPerfil _slotPerfil2;
    private SlotPerfil _slotPerfil3;

    private List<SlotPerfil> slotPerfiles;

    private ButtonPersonalizado _buttonAtras;

    public bool OcultarBotonAtras = false;

    public override void _Ready()
    {
        base._Ready();

        _slotPerfil1 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil1");
        _slotPerfil1.Pressed += OnSlotPerfil1Pressed;
        CargarSlotPerfil(_slotPerfil1, Ajustes.IdPerfilSlot1);

        _slotPerfil2 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil2");
        _slotPerfil2.Pressed += OnSlotPerfil2Pressed;
        CargarSlotPerfil(_slotPerfil2, Ajustes.IdPerfilSlot2);

        _slotPerfil3 = UtilidadesNodos.ObtenerNodoPorNombre<SlotPerfil>(this, "SlotPerfil3");
        _slotPerfil3.Pressed += OnSlotPerfil3Pressed;
        CargarSlotPerfil(_slotPerfil3, Ajustes.IdPerfilSlot3);

        slotPerfiles = [_slotPerfil1, _slotPerfil2, _slotPerfil3];

        _buttonAtras = UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

        LoggerJuego.Trace(this.Name + " Ready.");
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
            slotPerfil.SetActivo(true);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Solo respondemos si el menú es visible.
        if (!this.IsVisibleInTree())
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            if (Global.NavegacionTeclado)
            {
                if (!OcultarBotonAtras)
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

    public void OnSlotPerfilPressed(SlotPerfil slotPerfilSeleccionado)
    {
        if (!slotPerfilSeleccionado.Vacio && !slotPerfilSeleccionado.Activo)
        {
            //EmitSignal(SignalName.OnSlotPerfilNoActivoPressed, slotPerfilSeleccionado);
            OnSlotPerfilNoActivoPressed(slotPerfilSeleccionado);
        }
        else if (slotPerfilSeleccionado.Vacio)
        {
            EmitSignal(SignalName.OnSlotVacioPressed, slotPerfilSeleccionado);
        }
    }

    private void OnSlotPerfilNoActivoPressed(SlotPerfil slotPerfilSeleccionado)
    {
        string textoMensaje;
        if (slotPerfiles.Any(p => p.Activo))
            textoMensaje = "MenuPrincipal.perfil.confirmarCambiarPerfil";
        else
            textoMensaje = "MenuPrincipal.perfil.confirmarCargarPerfil";

        this.Hide();

        ContenedorConfirmacion.Instanciar(this, textoMensaje, "General.si", "General.no",
            () => OnSeleccionarSlotPerfil(slotPerfilSeleccionado), OnSeleccionarPerfilCancelar);
    }

    public void OnSeleccionarSlotPerfil(SlotPerfil slotPerfilSeleccionado)
    {
        CambiarPerfilActivo(slotPerfilSeleccionado);

        CallDeferred(nameof(PulsarBotonAtras));
    }

    private void PulsarBotonAtras()
    {
        UtilidadesNodos.PulsarBoton(this._buttonAtras);
    }


    private void CambiarPerfilActivo(SlotPerfil slotPerfilSeleccionado)
    {
        Perfil perfilSeleccionado = slotPerfilSeleccionado.Perfil;
        Global.CambiarPerfilActivo(perfilSeleccionado);

        foreach (var slotPerfil in slotPerfiles.Where(sp => !sp.Vacio && sp.Activo))
            slotPerfil.SetActivo(false);

        slotPerfilSeleccionado.SetActivo(true);
    }

    public void OnSeleccionarPerfilCancelar()
    {
        this.Show();
        this.GrabFocusUltimoElementoConFoco();
    }

    public void Show(bool seleccionarPrimerElemento, bool ocultarBotonAtras)
    {
        this.Show(seleccionarPrimerElemento);

        OcultarBotonAtras = ocultarBotonAtras;
        if (ocultarBotonAtras)
            this._buttonAtras.Hide();
    }

    public void CrearPerfilEnSlot(SlotPerfil slotPerfilVacioSeleccionado, string nombrePerfil)
    {
        string idPerfil = GestorPerfiles.GenerarIdPerfil();
        Perfil nuevoPerfil = new(idPerfil, nombrePerfil, DateTime.Now, null);

        GestorPerfiles.GuardarPerfil(nuevoPerfil);
        slotPerfilVacioSeleccionado.Perfil = nuevoPerfil;

        int slotIndex = slotPerfiles.IndexOf(slotPerfilVacioSeleccionado);
        Ajustes.SetIdPerfil(slotIndex + 1, idPerfil);

        CambiarPerfilActivo(slotPerfilVacioSeleccionado);

        if (OcultarBotonAtras)
            EmitSignal(SignalName.OnCrearPrimerPerfil);
    }
}
