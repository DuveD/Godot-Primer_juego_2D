using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;

namespace Primerjuego2D.escenas.menuPrincipal.perfil;

public partial class PanelContainerNuevoPerfil : ContenedorMenu
{
    [Signal]
    public delegate void OnButtonConfirmarPressedEventHandler(string nombrePerfil);

    [Signal]
    public delegate void OnButtonCancelarPressedEventHandler();

    private LineEdit _lineEdit;
    private ButtonPersonalizado _buttonCrearPerfilConfirmar;
    private ButtonPersonalizado _buttonCrearPerfilCancelar;

    public override void _Ready()
    {
        _lineEdit = GetNode<LineEdit>("VBoxContainer/LineEdit");
        _lineEdit.TextChanged += OnLineEditTextChanged;

        _buttonCrearPerfilConfirmar = GetNode<ButtonPersonalizado>("VBoxContainer/HBoxContainer/ButtonCrearPerfilConfirmar");
        _buttonCrearPerfilConfirmar.Pressed += () => EmitSignal(SignalName.OnButtonConfirmarPressed, _lineEdit.Text);

        _buttonCrearPerfilCancelar = GetNode<ButtonPersonalizado>("VBoxContainer/HBoxContainer/ButtonCrearPerfilCancelar");
        _buttonCrearPerfilCancelar.Pressed += () => EmitSignal(SignalName.OnButtonCancelarPressed);
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [_lineEdit, _buttonCrearPerfilConfirmar, _buttonCrearPerfilCancelar];
    }

    private void OnLineEditTextChanged(string newText)
    {
        _buttonCrearPerfilConfirmar.Disabled = string.IsNullOrWhiteSpace(newText);
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return _lineEdit;
    }
}