
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
  public PackedScene PowerUpInvulnerabilidadpacPackedScene;

  [Export]
  public int DistanciaMinima = 200;

  private Jugador Jugador { get; set; }

  public override void _Ready()
  {
    LoggerJuego.Trace(this.Name + " Ready.");
  }

  public void IniciarSpawnLoop(Jugador jugador)
  {
    this.Jugador = jugador;
    SpawnLoop();
  }

  private async void SpawnLoop()
  {
    while (true)
    {
      await ToSignal(GetTree().CreateTimer(_TiempoSpawn), Timer.SignalName.Timeout);

      SpawnPowerUp();
    }
  }
  private void SpawnPowerUp()
  {
    if (Randomizador.GetRandomFloat() >= 0.5)
    {
      SpawnPowerUp(PowerUpImanMonedaPackedScene);
    }
    else
    {
      SpawnPowerUp(PowerUpInvulnerabilidadpacPackedScene);
    }
  }

  private void SpawnPowerUp(PackedScene powerUpPackedScene)
  {
    LoggerJuego.Trace("Spawneamos un nuevo PowerUp.");

    PowerUp powerUp = powerUpPackedScene.Instantiate<PowerUp>();
    powerUp.TiempoDestruccion = 3;
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
