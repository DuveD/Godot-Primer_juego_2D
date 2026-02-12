using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.menuPrincipal.perfil;
using Primerjuego2D.escenas.ui;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPerfiles : CenterContainer
{
    [Signal]
    public delegate void OnCrearPrimerPerfilEventHandler();

    public PanelContainerPerfiles PanelContainerPerfiles;
    public PanelContainerNuevoPerfil PanelNuevoPerfil;

    private SlotPerfil _slotSeleccionado;

    private bool _ocultarBotonAtras;

    public override void _Ready()
    {
        base._Ready();

        PanelContainerPerfiles = GetNode<PanelContainerPerfiles>("PanelContainerPerfiles");
        PanelNuevoPerfil = GetNode<PanelContainerNuevoPerfil>("PanelContainerNuevoPerfil");

        PanelContainerPerfiles.OnSlotSeleccionadoPressed += OnSlotPerfilPressed;
        PanelContainerPerfiles.OnSlotVacioPressed += OnSlotVacioPressed;
        PanelNuevoPerfil.OnButtonConfirmarPressed += OnCrearNuevoPerfil;
        PanelNuevoPerfil.OnButtonCancelarPressed += OnCancelarNuevoPerfil;

        LoggerJuego.Trace(this.Name + " Ready.");

        // Inicializa UI
        ActualizarSlots();
    }

    private void ActualizarSlots()
    {
        var perfiles = new List<Perfil>
        {
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot1),
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot2),
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot3)
        }.Where(p => p != null).ToList();

        PanelContainerPerfiles.ConfigurarSlots(perfiles);
    }

    private void OnSlotVacioPressed(SlotPerfil slotVacio)
    {
        _slotSeleccionado = slotVacio;
        PanelContainerPerfiles.Hide();
        PanelNuevoPerfil.Show(true);
        PanelNuevoPerfil.Limpiar();
    }

    private void OnSlotPerfilPressed(SlotPerfil slotPerfil)
    {
        if (!slotPerfil.Vacio && !slotPerfil.Activo)
        {
            string textoMensaje;
            if (PanelContainerPerfiles.HaySlotPerfilActivo)
                textoMensaje = "MenuPrincipal.perfil.confirmarCambiarPerfil";
            else
                textoMensaje = "MenuPrincipal.perfil.confirmarCargarPerfil";

            PanelContainerPerfiles.Hide();

            // Mostrar confirmación de cambio
            ContenedorConfirmacion.Instanciar(
                this,
                textoMensaje,
                "General.si",
                "General.no",
                () => CambiarPerfilActivo(slotPerfil),
                () => PanelContainerPerfiles.Show(false, false)
            );
        }
    }

    private void OnCrearNuevoPerfil(string nombrePerfil)
    {
        if (_slotSeleccionado == null) return;

        string idPerfil = GestorPerfiles.GenerarIdPerfil();
        Perfil nuevoPerfil = new(idPerfil, nombrePerfil, DateTime.Now, null);
        GestorPerfiles.GuardarPerfil(nuevoPerfil);

        _slotSeleccionado.Perfil = nuevoPerfil;
        CambiarPerfilActivo(_slotSeleccionado);

        _slotSeleccionado = null;

        PanelNuevoPerfil.Hide();
        PanelContainerPerfiles.Show(true);
        ActualizarSlots();

        if (_ocultarBotonAtras)
            EmitSignal(SignalName.OnCrearPrimerPerfil);
    }

    private void OnCancelarNuevoPerfil()
    {
        _slotSeleccionado = null;
        PanelNuevoPerfil.Hide();
        PanelContainerPerfiles.Show(true);
    }

    private void CambiarPerfilActivo(SlotPerfil slotPerfil)
    {
        var perfil = slotPerfil.Perfil;
        Global.CambiarPerfilActivo(perfil);

        PanelContainerPerfiles.Show(false, false);
        foreach (var s in new[] { PanelContainerPerfiles }) // aquí podrías iterar slots si fuera necesario
        {
            // UI: marcar activo
            foreach (var sp in s.ObtenerElementosConFoco().OfType<SlotPerfil>())
                sp.SetActivo(sp == slotPerfil);
        }
    }

    public void Show(bool seleccionarPrimerElemento, bool ocultarBotonAtras)
    {
        _ocultarBotonAtras = ocultarBotonAtras;
        PanelContainerPerfiles.Show(seleccionarPrimerElemento, ocultarBotonAtras);
        PanelNuevoPerfil.Hide();
        this.Show();
    }
}
