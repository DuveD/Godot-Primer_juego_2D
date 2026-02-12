using Godot;
using Primerjuego2D.escenas.menuPrincipal.perfil;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPerfiles : CenterContainer
{
    public PanelContainerPerfiles PanelContainerPerfiles;

    public PanelContainerNuevoPerfil _panelContainerNuevoPerfil;

    private SlotPerfil _slotSeleccionado;

    public override void _Ready()
    {
        base._Ready();

        PanelContainerPerfiles = GetNode<PanelContainerPerfiles>("PanelContainerPerfiles");
        _panelContainerNuevoPerfil = GetNode<PanelContainerNuevoPerfil>("PanelContainerNuevoPerfil");

        PanelContainerPerfiles.OnSlotVacioPressed += ContainerPerfiles_OnSlotVacioPressed;
        _panelContainerNuevoPerfil.OnButtonConfirmarPressed += ContainerNuevoPerfil_OnButtonConfirmarPressed;
        _panelContainerNuevoPerfil.OnButtonCancelarPressed += ContainerNuevoPerfil_OnButtonCancelarPressed;

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void ContainerPerfiles_OnSlotVacioPressed(SlotPerfil slotPerfilVacioSeleccionado)
    {
        _slotSeleccionado = slotPerfilVacioSeleccionado;
        PanelContainerPerfiles.Hide();
        _panelContainerNuevoPerfil.Show(true);
    }

    private void ContainerNuevoPerfil_OnButtonConfirmarPressed(string nombrePerfil)
    {
        _panelContainerNuevoPerfil.Hide();
        PanelContainerPerfiles.Show(false);
        PanelContainerPerfiles.CrearPerfilEnSlot(_slotSeleccionado, nombrePerfil);
        _slotSeleccionado = null;
    }

    private void ContainerNuevoPerfil_OnButtonCancelarPressed()
    {
        _slotSeleccionado = null;
        _panelContainerNuevoPerfil.Hide();
        PanelContainerPerfiles.Show(PanelContainerPerfiles.OcultarBotonAtras);
    }

    public void Show(bool seleccionarPrimerElemento, bool ocultarBotonAtras)
    {
        PanelContainerPerfiles.Show(seleccionarPrimerElemento, ocultarBotonAtras);
        this.Show();
    }
}
