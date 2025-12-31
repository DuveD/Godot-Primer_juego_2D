using System;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.modelos.objetos;

public abstract partial class PowerUp : Consumible
{
    public enum TipoAcumulacionPowerUp
    {
        NoAcumulable,
        RenuevaDuracion,
        AcumulaEfecto
    }

    public string Id { get; } = System.Guid.NewGuid().ToString();

    // Si es -1 es permanente.
    [Export]
    public float TiempoDuracion { get; set; } = -1f;

    [Export]
    public TipoAcumulacionPowerUp TipoAcumulacion;

    [Export]
    public bool PermiteDuplicados = false;

    private Timer TimerDuracionPowerUp;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        base._Ready();
    }

    public override void OnRecogida(Jugador jugador)
    {
        LoggerJuego.Info("PowerUp " + this.Name + " recogido.");

        if (jugador.TienePowerUp(this))
        {
            switch (TipoAcumulacion)
            {
                default:
                case TipoAcumulacionPowerUp.NoAcumulable:
                    OnRecogidaNoAcumulable(jugador);
                    break;
                case TipoAcumulacionPowerUp.RenuevaDuracion:
                    OnRecogidaRenuevaDuracion(jugador);
                    break;
                case TipoAcumulacionPowerUp.AcumulaEfecto:
                    OnRecogidaAcumulaEfecto(jugador);
                    break;
            }
        }
        else
        {
            AnadirPowerUpAJugador(jugador);
        }

        // Evitamos que se vuelva a disparar la señal.
        CollisionShape2D?.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
        this.SetDeferred(Area2D.PropertyName.Monitoring, true);

        // Ocultamos el Sprite del consumible.
        Sprite2D.SetDeferred(CanvasItem.PropertyName.Visible, false);
    }

    private void AnadirPowerUpAJugador(Jugador jugador)
    {
        InicializarTimerDuracion(jugador);
        jugador.CallDeferred(nameof(Jugador.AnadirPowerUp), this);
        AplicarEfectoPowerUp(jugador);
    }

    public virtual void OnRecogidaNoAcumulable(Jugador jugador)
    {
    }

    public virtual void OnRecogidaRenuevaDuracion(Jugador jugador)
    {
        PowerUp powerUp = jugador.ObtenerPowerUp(this.GetType());
        powerUp?.ReiniciarTimer();
    }

    public virtual void OnRecogidaAcumulaEfecto(Jugador jugador)
    {
        if (this.PermiteDuplicados)
        {
            AnadirPowerUpAJugador(jugador);
        }
        else
        {
            SumarTiempoTimer(this.TiempoDuracion);
        }
    }

    private void InicializarTimerDuracion(Jugador jugador)
    {
        if (this.TiempoDuracion > 0)
        {
            this.TimerDuracionPowerUp = new Timer
            {
                WaitTime = this.TiempoDuracion,
                OneShot = true,
                Autostart = true,
            };
            this.TimerDuracionPowerUp.Timeout += () => OnTimerDuracionTimeout(jugador);
            AddChild(this.TimerDuracionPowerUp);
        }
    }

    public void ReiniciarTimer()
    {
        if (TimerDuracionPowerUp == null)
            return;

        this.TimerDuracionPowerUp.Stop();
        this.TimerDuracionPowerUp.WaitTime = TiempoDuracion;
        this.TimerDuracionPowerUp.Start();
    }

    public void SumarTiempoTimer(float tiempo)
    {
        if (TimerDuracionPowerUp == null)
            return;

        this.TimerDuracionPowerUp.Stop();
        this.TimerDuracionPowerUp.WaitTime += tiempo;
        this.TimerDuracionPowerUp.Start();
    }

    public abstract void AplicarEfectoPowerUp(Jugador jugador);

    public abstract void ProcessPowerUp(double delta, Jugador jugador);

    public void OnTimerDuracionTimeout(Jugador jugador)
    {
        LoggerJuego.Info("PowerUp " + this.Name + " expirado.");
        jugador.QuitarPowerUp(this);
        EfectoPowerUpExpirado(jugador);
    }

    public abstract void EfectoPowerUpExpirado(Jugador jugador);
}