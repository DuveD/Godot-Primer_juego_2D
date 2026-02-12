using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal.perfil;

public partial class PanelContainerNuevoPerfil : ContenedorMenu
{
    [Signal]
    public delegate void OnButtonConfirmarPressedEventHandler(string nombrePerfil);

    [Signal]
    public delegate void OnButtonCancelarPressedEventHandler();

    private LineEdit _lineEdit;
    private ButtonPersonalizado _buttonConfirmar;
    private ButtonPersonalizado _buttonCancelar;

    public override void _Ready()
    {
        base._Ready();

        _lineEdit = GetNode<LineEdit>("VBoxContainer/LineEdit");
        _lineEdit.TextChanged += OnLineEditTextChanged;

        _buttonConfirmar = GetNode<ButtonPersonalizado>("VBoxContainer/HBoxContainer/ButtonCrearPerfilConfirmar");
        _buttonConfirmar.Pressed += () => EmitSignal(SignalName.OnButtonConfirmarPressed, _lineEdit.Text);

        _buttonCancelar = GetNode<ButtonPersonalizado>("VBoxContainer/HBoxContainer/ButtonCrearPerfilCancelar");
        _buttonCancelar.Pressed += () => EmitSignal(SignalName.OnButtonCancelarPressed);

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void OnLineEditTextChanged(string newText)
    {
        _buttonConfirmar.Disabled = string.IsNullOrWhiteSpace(newText);
    }

    public override List<Control> ObtenerElementosConFoco() => new List<Control> { _lineEdit, _buttonConfirmar, _buttonCancelar };

    public override Control ObtenerPrimerElementoConFoco() => _lineEdit;

    public void Limpiar() => _lineEdit.Text = "";
}
