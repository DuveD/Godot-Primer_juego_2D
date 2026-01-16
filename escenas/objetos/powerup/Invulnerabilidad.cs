using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.nucleo.constantes;

namespace Primerjuego2D.escenas.objetos.powerup;

public partial class Invulnerabilidad : PowerUp
{
    [Export]
    private float AlphaMin = 0.1f;

    [Export]
    private float AlphaMax = 0.8f;

    [Export]
    private float VelocidadParpadeoFinal = 15f; // Oscilación muy rápida al final

    [Export]
    private float DuracionParpadeo = 1f;       // Último tramo donde parpadea

    public override void AplicarEfectoPowerUp(Jugador jugador)
    {
        jugador.Invulnerable = true;
        jugador.Velocidad += 100;

        // Al principio solo azul fijo
        jugador.CallDeferred(nameof(Jugador.SetSpriteColor), new Color(ConstantesColores.AZUL_CLARO));
        jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
    }

    public override void ProcessPowerUp(double delta, Jugador jugador)
    {
        // Si es permanente o no hay timer, efecto fijo
        if (this.TimerDuracionPowerUp == null || TiempoDuracion <= 0)
        {
            jugador.CallDeferred(nameof(Jugador.SetSpriteColor),
                new Color(ConstantesColores.AZUL_CLARO));
            jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
        }
        else
        {
            float timeLeft = (float)this.TimerDuracionPowerUp.TimeLeft;

            // Fase final
            if (timeLeft <= DuracionParpadeo)
            {
                float tiempoFinal = DuracionParpadeo - timeLeft;

                // Onda rápida
                float onda =
                    (Mathf.Sin(tiempoFinal * VelocidadParpadeoFinal * Mathf.Pi * 2f) + 1f) * 0.5f;

                Color color = new Color(ConstantesColores.AZUL_CLARO)
                    .Lerp(new Color(1, 1, 1), onda);

                jugador.CallDeferred(nameof(Jugador.SetSpriteColor), color);

                float alpha = Mathf.Lerp(AlphaMin, AlphaMax, onda);
                jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), alpha);
            }
            else
            {
                // Fase estable
                jugador.CallDeferred(nameof(Jugador.SetSpriteColor),
                    new Color(ConstantesColores.AZUL_CLARO));
                jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
            }
        }
    }

    public override void EfectoPowerUpExpirado(Jugador jugador)
    {
        jugador.Invulnerable = false;
        jugador.Velocidad -= 100;

        // Restauramos visibilidad total y color normal
        jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
        jugador.CallDeferred(nameof(Jugador.SetSpriteColor), new Color(1, 1, 1));
    }
}