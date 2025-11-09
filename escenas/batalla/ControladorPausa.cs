using Godot;
using System;

public partial class ControladorPausa : CanvasLayer
{
    [Signal]
    public delegate void PauseButtonPressedEventHandler();

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(ConstantesAcciones.PAUSAR_JUEGO))
        {
            OnPauseButtonPressed();
        }
    }

    public override void _Ready()
    {
        ProcessMode = Node.ProcessModeEnum.Always;
        this.Hide();
    }

    private void OnPauseButtonPressed()
    {
        bool pausarJuego = !Ajustes.JuegoPausado;
        UtilidadesNodos.PausarNodo(this, pausarJuego);

        EmitSignal(SignalName.PauseButtonPressed);

        if (pausarJuego)
        {
            this.Show();
        }
        else
        {
            this.Hide();
        }
    }
}
