using Godot;
using Primerjuego2D.escenas.objetos.moneda;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.estadisticas;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class BatallaControlador : Control
{
    [Signal]
    public delegate void IniciandoBatallaEventHandler();

    [Signal]
    public delegate void BatallaIniciadaEventHandler();

    [Signal]
    public delegate void BatallaFinalizadaEventHandler();

    [Signal]
    public delegate void PausarBatallaEventHandler();

    [Signal]
    public delegate void RenaudarBatallaEventHandler();

    [Signal]
    public delegate void PuntuacionActualizadaEventHandler(int Puntuacion);

    public int Puntuacion { get; private set; } = 0;

    public bool BatallaEnCurso { get; private set; } = false;

    public bool JuegoPausado { get; set; } = false;

    private BatallaHUD _BatallaHUD;
    private BatallaHUD BatallaHUD => _BatallaHUD ??= GetNode<BatallaHUD>("../BatallaHUD");

    private SpawnEnemigos _SpawnEnemigos;
    private SpawnEnemigos SpawnEnemigos => _SpawnEnemigos ??= GetNode<SpawnEnemigos>("../SpawnEnemigos");

    private SpawnMonedas _SpawnMonedas;
    private SpawnMonedas SpawnMonedas => _SpawnMonedas ??= GetNode<SpawnMonedas>("../SpawnMonedas");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        GestorEstadisticas.InicializarPartida();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            PausarJuego();
            AcceptEvent();
        }
    }

    public void PausarJuego()
    {
        if (!this.BatallaEnCurso)
            return;

        this.JuegoPausado = !JuegoPausado;

        UtilidadesNodos.PausarNodo(this, this.JuegoPausado);

        if (this.JuegoPausado)
        {
            LoggerJuego.Trace("Juego pausado.");
            EmitSignal(SignalName.PausarBatalla);
        }
        else
        {
            LoggerJuego.Trace("Juego renaudado.");
            EmitSignal(SignalName.RenaudarBatalla);
        }
    }

    public async void IniciarBatalla()
    {
        if (BatallaEnCurso)
            return;

        LoggerJuego.Info("Iniciamos la batalla.");
        EmitSignal(SignalName.IniciandoBatalla);

        this.Puntuacion = 0;
        this.BatallaEnCurso = true;

        EmitSignal(SignalName.PuntuacionActualizada, Puntuacion);

        SpawnMonedas.Spawn();

        await UtilidadesNodos.EsperarSegundos(this, 2.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        LoggerJuego.Info("Batalla iniciada.");
        EmitSignal(SignalName.BatallaIniciada);
    }

    public void FinalizarBatalla()
    {
        if (!this.BatallaEnCurso)
            return;

        BatallaEnCurso = false;

        GestorEstadisticas.PartidaActual.RegistrarPuntuacion(this.Puntuacion);
        GestorEstadisticas.FinalizarPartida();

        LoggerJuego.Info("Batalla finalizada.");
        EmitSignal(SignalName.BatallaFinalizada);

        GestorLogros.EmitirEvento(GestorLogros.EVENTO_LOGRO_PRIMERA_PARTIDA);
    }

    public void SumarPuntuacion(Moneda moneda)
    {
        if (!this.BatallaEnCurso)
            return;

        this.Puntuacion += moneda.Valor;
        GestorEstadisticas.PartidaActual.RegistrarMoneda(moneda is MonedaEspecial);

        EmitSignal(SignalName.PuntuacionActualizada, this.Puntuacion);
    }
}
