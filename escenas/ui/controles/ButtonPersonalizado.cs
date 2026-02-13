using System;
using Godot;
using Primerjuego2D.escenas.modelos.interfaces;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.ui.controles;

public partial class ButtonPersonalizado : Button, IFocusSilencioso
{
    [Export]
    public string NombreSonidoOnFocusEntered { get; set; }
    [Export]
    public string NombreSonidoOnMouseEntered { get; set; }
    [Export]
    public string NombreSonidoOnPressed { get; set; }

    private bool _reproducirSonido = true;

    public new bool Disabled
    {
        get => base.Disabled;
        set
        {
            base.Disabled = value;
            if (value)
            {
                this.FocusMode = FocusModeEnum.None;
                this.MouseFilter = MouseFilterEnum.Ignore;
            }
            else
            {
                this.FocusMode = FocusModeEnum.All;
                this.MouseFilter = MouseFilterEnum.Pass;
            }
        }
    }

    public override void _Ready()
    {
        this.FocusEntered += OnFocusedEntered;
        this.MouseEntered += OnMouseEntered;
        this.Pressed += OnPressed;
    }

    public void OnFocusedEntered()
    {
        if (this._reproducirSonido && !string.IsNullOrEmpty(NombreSonidoOnFocusEntered))
            Global.GestorAudio.ReproducirSonido(NombreSonidoOnFocusEntered);
    }

    public void OnMouseEntered()
    {
        if (!string.IsNullOrEmpty(NombreSonidoOnMouseEntered))
        {
            if (!this.Disabled)
                Global.GestorAudio.ReproducirSonido(NombreSonidoOnMouseEntered);
        }
    }

    private void OnPressed()
    {
        LoggerJuego.Trace("Botón '" + this.Name + "' pulsado.");

        if (this._reproducirSonido && !string.IsNullOrEmpty(NombreSonidoOnPressed))
            Global.GestorAudio.ReproducirSonido(NombreSonidoOnPressed);
    }

    public void GrabFocusSilencioso()
    {
        this._reproducirSonido = false;
        this.GrabFocus();
        this._reproducirSonido = true;
    }

    public void Desactivar(bool desactivar)
    {
        Disabled = desactivar;
        FocusMode = desactivar ? FocusModeEnum.None : FocusModeEnum.All;
        MouseFilter = desactivar ? MouseFilterEnum.Ignore : MouseFilterEnum.Pass;
    }
}