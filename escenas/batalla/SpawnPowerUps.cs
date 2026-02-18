
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class SpawnPowerUps : Control
{
  [Export]
  long _TiempoSpawn = 6;

  [Export]
  public int ProbabilidadSpawnPowerUps = 50;

  [Export]
  public PackedScene PowerUpImanMonedaPackedScene;

  [Export]
  public PackedScene PowerUpInvulnerabilidadPackedScene;

  [Export]
  public PackedScene BotiquinPackedScene;

  [Export]
  public int DistanciaMinima = 200;

  private Jugador Jugador { get; set; }

  private Timer _TimerSpawn;

  public override void _Ready()
  {
    LoggerJuego.Trace(this.Name + " Ready.");
  }

  public void IniciarSpawnLoop(Jugador jugador)
  {
    this.Jugador = jugador;
    InicializarTimer();
  }

  public void InicializarTimer()
  {
    this._TimerSpawn = new Timer();
    this._TimerSpawn.WaitTime = _TiempoSpawn;

    this._TimerSpawn.Timeout += SpawnPowerUp;

    this.AddChild(this._TimerSpawn);
    this._TimerSpawn.Start();
  }

  public void ReiniciarTimer()
  {
    if (_TimerSpawn == null)
      return;

    this._TimerSpawn.Stop();
    this._TimerSpawn.WaitTime = _TiempoSpawn;
    this._TimerSpawn.Start();
  }

  private void SpawnPowerUp()
  {
    List<PackedScene> posibles = new();

    // Siempre permitido
    posibles.Add(PowerUpImanMonedaPackedScene);
    posibles.Add(PowerUpInvulnerabilidadPackedScene);

    // Solo si NO tiene la vida al máximo
    if (this.Jugador.Vida < this.Jugador.VidaMaxima)
      posibles.Add(BotiquinPackedScene);

    // Seguridad: si por algún motivo no hay ninguno
    if (posibles.Count == 0)
      return;

    int index = Randomizador.GetRandomInt(0, posibles.Count - 1);
    SpawnPowerUp(posibles[index]);

    ReiniciarTimer();
  }

  private void SpawnPowerUp(PackedScene powerUpPackedScene)
  {
    LoggerJuego.Trace("Spawneamos un nuevo PowerUp.");

    // Lo hacemos Consumible para los powerup cómo la vida, que no tienen efecto continuo, sólo al recogerse.
    Consumible powerUp = powerUpPackedScene.Instantiate<Consumible>();
    powerUp.Position = ObtenerPosicionAleatoriaSegura();

    this.GetParent().AddChild(powerUp);
  }

  private Vector2 ObtenerPosicionAleatoriaSegura()
  {
    Vector2 nuevaPos;
    Vector2 centroJugador = Jugador.GlobalPosition;
    bool cercaJugador;

    List<Consumible> consumibles = UtilidadesNodos.ObtenerNodosDeTipo<Consumible>(GetTree().CurrentScene);
    bool cercaOtraMoneda;

    do
    {
      float x = (float)Randomizador.GetRandomDouble(GlobalPosition.X, GlobalPosition.X + Size.X);
      float y = (float)Randomizador.GetRandomDouble(GlobalPosition.Y, GlobalPosition.Y + Size.Y);
      nuevaPos = new Vector2(x, y);

      cercaJugador = UtilidadesMatematicas.PuntosCerca(centroJugador, nuevaPos, this.DistanciaMinima);
      cercaOtraMoneda = consumibles.Any(consumible => UtilidadesMatematicas.PuntosCerca(consumible.Position, nuevaPos, consumible.ObtenerRadioCollisionShape2D() * 2));
    }
    while (cercaJugador || cercaOtraMoneda);

    return nuevaPos;
  }
}
