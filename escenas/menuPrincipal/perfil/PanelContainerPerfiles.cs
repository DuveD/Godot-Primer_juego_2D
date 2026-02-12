using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.sistema.perfil;

namespace Primerjuego2D.escenas.menuPrincipal.perfil;

public partial class PanelContainerPerfiles : ContenedorMenu
{
    [Signal]
    public delegate void OnSlotVacioPressedEventHandler(SlotPerfil slotPerfilVacioSeleccionado);

    [Signal]
    public delegate void OnSlotSeleccionadoPressedEventHandler(SlotPerfil slotPerfilSeleccionado);

    private SlotPerfil _slotPerfil1;
    private SlotPerfil _slotPerfil2;
    private SlotPerfil _slotPerfil3;

    private List<SlotPerfil> _slotPerfiles => new List<SlotPerfil> { _slotPerfil1, _slotPerfil2, _slotPerfil3 };

    private ButtonPersonalizado _buttonAtras;

    public ButtonPersonalizado ButtonAtras => _buttonAtras;

    public override void _Ready()
    {
        base._Ready();

        _slotPerfil1 = GetNode<SlotPerfil>("VBoxContainer/VBoxContainer/SlotPerfil1");
        _slotPerfil2 = GetNode<SlotPerfil>("VBoxContainer/VBoxContainer/SlotPerfil2");
        _slotPerfil3 = GetNode<SlotPerfil>("VBoxContainer/VBoxContainer/SlotPerfil3");

        _slotPerfil1.Pressed += () => EmitSignal(SignalName.OnSlotSeleccionadoPressed, _slotPerfil1);
        _slotPerfil2.Pressed += () => EmitSignal(SignalName.OnSlotSeleccionadoPressed, _slotPerfil2);
        _slotPerfil3.Pressed += () => EmitSignal(SignalName.OnSlotSeleccionadoPressed, _slotPerfil3);

        _buttonAtras = GetNode<ButtonPersonalizado>("VBoxContainer/HBoxContainer/ButtonAtras");
    }

    public void Show(bool seleccionarPrimerElemento, bool ocultarBotonAtras)
    {
        this.Show(seleccionarPrimerElemento);
        if (ocultarBotonAtras)
            this._buttonAtras.Hide();
    }

    public void ConfigurarSlots(List<Perfil> perfiles)
    {
        for (int i = 0; i < _slotPerfiles.Count; i++)
        {
            Perfil perfil = i < perfiles.Count ? perfiles[i] : null;
            _slotPerfiles[i].Perfil = perfil;
            bool activo = Ajustes.IdPerfilActivo == perfil?.Id;
            _slotPerfiles[i].SetActivo(activo);
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

    public bool HaySlotPerfilActivo => _slotPerfiles.Any(p => p.Activo);
}
