using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class Batalla : Node
{
    [Signal]
    public delegate void GameOverFinalizadoEventHandler();

    ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private Jugador _Jugador;
    private Jugador Jugador => _Jugador ??= GetNode<Jugador>("Jugador");

    private Marker2D _StartPosition;
    private Marker2D StartPosition => _StartPosition ??= GetNode<Marker2D>("StartPosition");

    private BatallaControlador _BatallaControlador;
    private BatallaControlador BatallaControlador => _BatallaControlador ??= GetNode<BatallaControlador>("BatallaControlador");

    private CanvasLayer _CanvasLayer;
    private CanvasLayer CanvasLayer => _CanvasLayer ??= GetNode<CanvasLayer>("CanvasLayer");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        ProcessMode = ProcessModeEnum.Pausable;

        this.NuevoJuego();
    }

    public void NuevoJuego()
    {
        LoggerJuego.Info("Nuevo juego.");

        this.Jugador.Start(this.StartPosition.Position);

        this.BatallaControlador.IniciarBatalla();
    }

    public void TerminarJuego()
    {
        if (this.BatallaControlador.JuegoPausado)
            this.BatallaControlador.RenaudarJuego();

        this.Jugador.Morir();
    }

    public void InicioGameOver()
    {
        if (this.BatallaControlador.JuegoPausado)
            this.BatallaControlador.RenaudarJuego();

        Global.GestorAudio.PausarMusica(2f);

        this.BatallaControlador.FinalizarBatalla();
    }

    public void FinGameOver()
    {
        if (this.BatallaControlador.JuegoPausado)
            this.BatallaControlador.RenaudarJuego();

        EmitSignal(SignalName.GameOverFinalizado);
    }
}
