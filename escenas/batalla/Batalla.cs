using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class Batalla : Node
{
    [Signal]
    public delegate void GameOverFinalizadoEventHandler();

    private Jugador Jugador;

    private Marker2D StartPosition;

    private BatallaControlador BatallaControlador;

    public override void _Ready()
    {
        this.Jugador = GetNode<Jugador>("Jugador");
        this.StartPosition = GetNode<Marker2D>("StartPosition");
        this.BatallaControlador = GetNode<BatallaControlador>("BatallaControlador");

        this.NuevoJuego();

        LoggerJuego.Trace(this.Name + " Ready.");
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
