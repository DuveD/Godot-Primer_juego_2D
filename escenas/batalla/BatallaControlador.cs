namespace Primerjuego2D.escenas.batalla;

using System;
using Godot;
using Primerjuego2D.nucleo.ajustes;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;


public partial class BatallaControlador : Node
{
    [Signal]
    public delegate void PauseBattleEventHandler();

    [Signal]
    public delegate void BatallaIniciadaEventHandler();

    [Signal]
    public delegate void BatallaFinalizadaEventHandler();

    public bool BatallaEnCurso { get; private set; } = false;

    public override void _Ready()
    {
        Logger.Trace("BatallaControlador Ready.");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(ConstantesAcciones.PAUSAR_JUEGO))
        {
            OnPauseButtonPressed();
        }
    }

    private void OnPauseButtonPressed()
    {

        if (!this.BatallaEnCurso)
            return;

        bool pausarJuego = !Ajustes.JuegoPausado;

        if (pausarJuego)
            Logger.Trace("Juego pausado.");
        else
            Logger.Trace("Juego renaudado.");

        UtilidadesNodos.PausarNodo(this, pausarJuego, pausarJuego);

        EmitSignal(SignalName.PauseBattle);
    }

    public void IniciarBatalla()
    {
        if (BatallaEnCurso)
            return;

        BatallaEnCurso = true;
        Logger.Info("Batalla iniciada.");
        EmitSignal(SignalName.BatallaIniciada);
    }

    public void FinalizarBatalla()
    {
        if (!BatallaEnCurso)
            return;

        BatallaEnCurso = false;
        Logger.Info("Batalla finalizada.");
        EmitSignal(SignalName.BatallaFinalizada);
    }
}
