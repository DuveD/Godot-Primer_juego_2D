using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.modelos.objetos;

namespace Primerjuego2D.escenas.objetos.powerup;

public partial class ImanMonedas : PowerUp
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void AplicarEfectoPowerUp(Jugador jugador)
    {
    }

    public override void EfectoPowerUpExpirado(Jugador jugador)
    {
    }

    public override void ProcessPowerUp(Jugador jugador, double delta)
    {
    }
}
