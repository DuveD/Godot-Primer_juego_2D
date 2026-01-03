using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.nucleo.constantes;

public partial class Invulnerabilidad : PowerUp
{
    [Export] private float AlphaMin = 0.1f;
    [Export] private float AlphaMax = 0.8f;
    [Export] private float VelocidadParpadeoFinal = 15f; // Oscilación muy rápida al final
    [Export] private float DuracionParpadeo = 1f;       // Último tramo donde parpadea

    private float _tiempo;

    public override void AplicarEfectoPowerUp(Jugador jugador)
    {
        jugador.Invulnerable = true;
        _tiempo = 0f;

        // Al principio solo azul fijo
        jugador.CallDeferred(nameof(Jugador.SetSpriteColor), new Color(ConstantesColores.AZUL_CLARO));
        jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
    }

    public override void ProcessPowerUp(double delta, Jugador jugador)
    {
        _tiempo += (float)delta;

        // Si el powerup tiene duración finita, detectamos fase final
        if (TiempoDuracion > 0 && _tiempo >= TiempoDuracion - DuracionParpadeo)
        {
            float tiempoFinal = _tiempo - (TiempoDuracion - DuracionParpadeo);

            // Oscilación muy rápida entre azul y normal
            float onda = (Mathf.Sin(tiempoFinal * VelocidadParpadeoFinal * Mathf.Pi * 2) + 1f) * 0.5f;

            // Interpolamos entre azul y blanco
            Color color = new Color(ConstantesColores.AZUL_CLARO).Lerp(new Color(1, 1, 1), onda);
            jugador.CallDeferred(nameof(Jugador.SetSpriteColor), color);

            // También podemos oscilar la transparencia si quieres
            float alpha = Mathf.Lerp(AlphaMin, AlphaMax, onda);
            jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), alpha);
        }
        else
        {
            // Fase inicial: azul fijo
            jugador.CallDeferred(nameof(Jugador.SetSpriteColor), new Color(ConstantesColores.AZUL_CLARO));
            jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
        }
    }

    public override void EfectoPowerUpExpirado(Jugador jugador)
    {
        jugador.Invulnerable = false;

        // Restauramos visibilidad total y color normal
        jugador.CallDeferred(nameof(Jugador.SetSpriteAlpha), 1f);
        jugador.CallDeferred(nameof(Jugador.SetSpriteColor), new Color(1, 1, 1));
    }
}
