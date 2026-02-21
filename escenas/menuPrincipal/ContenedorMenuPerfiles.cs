using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.menuPrincipal.perfil;
using Primerjuego2D.escenas.ui;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPerfiles : CenterContainer
{
    [Signal]
    public delegate void OnCrearPrimerPerfilEventHandler();

    public PanelContainerPerfiles PanelContainerPerfiles;
    public PanelContainerNuevoPerfil PanelNuevoPerfil;

    private SlotPerfil _slotSeleccionado;

    private bool _ocultarBotones;

    private bool _modoBorrarActivo = false;

    private List<Perfil> _perfiles;

    public override void _Ready()
    {
        base._Ready();

        PanelContainerPerfiles = GetNode<PanelContainerPerfiles>("PanelContainerPerfiles");
        PanelNuevoPerfil = GetNode<PanelContainerNuevoPerfil>("PanelContainerNuevoPerfil");

        PanelContainerPerfiles.OnSlotSeleccionadoPressed += OnSlotPerfilPressed;
        PanelContainerPerfiles.OnButtonBorrarToggled += OnButtonBorrarToggled;
        PanelNuevoPerfil.OnButtonConfirmarPressed += OnCrearNuevoPerfil;
        PanelNuevoPerfil.OnButtonCancelarPressed += OnCancelarNuevoPerfil;

        LoggerJuego.Trace(this.Name + " Ready.");

        // Inicializa UI
        InicializarSlots();
    }

    private void InicializarSlots()
    {
        _perfiles = new List<Perfil>
        {
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot1),
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot2),
            GestorPerfiles.CargarPerfil(Ajustes.IdPerfilSlot3)
        };

        ActualizarSlots();
    }

    private void ActualizarSlots()
    {
        if (_perfiles == null || _perfiles.Count != 3)
        {
            LoggerJuego.Error("La lista de perfiles debe contener exactamente 3 elementos.");
            return;
        }

        for (int i = 0; i < _perfiles.Count; i++)
        {
            Perfil perfil = i < _perfiles.Count ? _perfiles[i] : null;
            if (perfil == null)
                Ajustes.SetIdPerfil(i + 1, ""); // Limpia el ID del perfil en ajustes si el perfil es null.
        }

        PanelContainerPerfiles.ConfigurarSlots(_perfiles);
    }

    private void OnButtonBorrarToggled(bool toggled)
    {
        _modoBorrarActivo = toggled;
    }

    private void OnSlotPerfilPressed(SlotPerfil slotPerfil)
    {
        // Borrar perfil
        if (_modoBorrarActivo)
        {
            OnSlotPerfilBorrarPressed(slotPerfil);
        }
        // Cambiar perfil activo
        else if (!slotPerfil.Vacio)
        {
            if (!slotPerfil.Activo)
            {
                OnSlotPerfilCambiarPressed(slotPerfil);
            }
            else
            {
                // El perfil seleccionado ya está activo.
            }
        }
        // Crear nuevo perfil
        else if (slotPerfil.Vacio)
        {
            OnSlotVacioPressed(slotPerfil);
        }
    }

    private void OnSlotPerfilBorrarPressed(SlotPerfil slotPerfil)
    {
        PanelContainerPerfiles.Hide();

        // Mostrar confirmación de cambio
        ContenedorConfirmacion.Instanciar(
            this,
            "MenuPrincipal.perfil.confirmarEliminarPerfil",
            "General.si",
            "General.no",
            () => EliminarPerfil(slotPerfil),
            () => PanelContainerPerfiles.Show(false, false)
        );
    }

    private void OnSlotPerfilCambiarPressed(SlotPerfil slotPerfil)
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

    private void OnSlotVacioPressed(SlotPerfil slotVacio)
    {
        _slotSeleccionado = slotVacio;
        PanelContainerPerfiles.Hide();
        PanelNuevoPerfil.Show(true);
    }

    private async void OnCrearNuevoPerfil(string nombrePerfil)
    {
        if (_slotSeleccionado == null) return;

        Global.IndicadorGuardado.Mostrar();

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        Perfil nuevoPerfil = await Task.Run(() =>
        {
            string idPerfil = GestorPerfiles.GenerarIdPerfil();
            Perfil nuevoPerfil = new(idPerfil, nombrePerfil, DateTime.Now, null);
            GestorPerfiles.InicializarPerfil(nuevoPerfil);
            GestorPerfiles.GuardarPerfil(nuevoPerfil);

            return nuevoPerfil;
        });

        _slotSeleccionado.Perfil = nuevoPerfil;
        CambiarPerfilActivo(_slotSeleccionado);

        _perfiles[_slotSeleccionado.NumeroSlot - 1] = nuevoPerfil;
        Ajustes.SetIdPerfil(_slotSeleccionado.NumeroSlot, nuevoPerfil.Id);

        ActualizarSlots();

        _slotSeleccionado = null;

        Global.IndicadorGuardado.Esconder();

        PanelNuevoPerfil.Hide();
        PanelContainerPerfiles.Show(false);

        if (_ocultarBotones)
            EmitSignal(SignalName.OnCrearPrimerPerfil);
    }
    private void EliminarPerfil(SlotPerfil slotPerfil)
    {
        var perfil = slotPerfil.Perfil;
        if (perfil == null) return;

        slotPerfil.Perfil = null;

        if (Ajustes.IdPerfilActivo == perfil.Id)
            Global.CambiarPerfilActivo(null);

        PanelContainerPerfiles.Show(true, _ocultarBotones);

        _perfiles[slotPerfil.NumeroSlot - 1] = null;
        Ajustes.SetIdPerfil(slotPerfil.NumeroSlot, ""); // Limpia el ID del perfil en ajustes.
        ActualizarSlots();

        GestorPerfiles.EliminarPerfil(perfil);
    }

    private void CambiarPerfilActivo(SlotPerfil slotPerfil)
    {
        var perfil = slotPerfil.Perfil;
        Global.CambiarPerfilActivo(perfil);

        PanelContainerPerfiles.Show(false, false);

        ActualizarSlots();
    }

    private void OnCancelarNuevoPerfil()
    {
        _slotSeleccionado = null;
        PanelNuevoPerfil.Hide();
        PanelContainerPerfiles.Show(false, _ocultarBotones);
    }

    public void Show(bool seleccionarPrimerElemento, bool ocultarBotones)
    {
        _ocultarBotones = ocultarBotones;
        PanelContainerPerfiles.Show(seleccionarPrimerElemento, ocultarBotones);
        PanelNuevoPerfil.Hide();
        this.Show();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            if (PanelContainerPerfiles.Visible)
            {
                UtilidadesNodos.PulsarBoton(PanelContainerPerfiles.ButtonAtras);
            }
            else if (PanelNuevoPerfil.Visible)
            {
                UtilidadesNodos.PulsarBoton(PanelNuevoPerfil.ButtonCancelar);
            }

            AcceptEvent();
        }
    }
}
