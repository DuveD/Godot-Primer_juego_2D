using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.entidades.enemigo;
using Primerjuego2D.escenas.modelos.objetos;
using Primerjuego2D.escenas.objetos.powerup;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.entidades.jugador;

public partial class Jugador : CharacterBody2D
{
    public const string ANIMATION_UP = "up";

    public const string ANIMATION_WALK = "walk";

    [Export]
    public int Velocidad { get; set; } = 400; // Velocidad de movimiento del jugador (pixels/sec).

    public bool Muerto { get; private set; } = false;

    // Señal "MuerteJugador" para indicar colisión con el jugador.
    [Signal]
    public delegate void MuerteJugadorEventHandler();

    [Signal]
    public delegate void AnimacionMuerteJugadorTerminadaEventHandler();

    private CollisionShape2D _CollisionShape2D;
    private CollisionShape2D CollisionShape2D => _CollisionShape2D ??= GetNode<CollisionShape2D>("HitBox/HitBoxCollisionShape2D");

    private AnimatedSprite2D _AnimatedSprite2D;
    private AnimatedSprite2D AnimatedSprite2D => _AnimatedSprite2D ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    private CpuParticles2D _ExplosionMuerte;
    private CpuParticles2D ExplosionMuerte => _ExplosionMuerte ??= GetNode<CpuParticles2D>("ExplosionMuerte");

    private Vector2 TamanoPantalla => UtilidadesPantalla.ObtenerTamanoPantalla(this);

    private readonly List<PowerUp> _PowerUpsActivos = [];
    public IReadOnlyList<PowerUp> PowerUpsActivos => _PowerUpsActivos;

    public bool Invulnerable = false;

    // Se llama cuando el nodo entra por primera vez en el árbol de escenas.
    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.JUGADOR);

        // Oculatamos el sprite al inicio de la partida.
        this.Hide();

        this.CollisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }

    // Se llama en cada fotograma. 'delta' es el tiempo transcurrido desde el fotograma anterior.
    public override void _Process(double delta)
    {
        if (this.Muerto)
            return;

        ProcessMovimiento(delta);
        ProcessPowerUps(delta);
    }

    private void ProcessMovimiento(double delta)
    {
        var velocidad = CalcularVectorVelocidadMedianteBotonesPulsados();

        // Indicamos la animación inicial para que mire hacia arriba.
        //this.AnimatedSprite2D.Animation = ANIMATION_UP;

        // Idicamos la animación según si está en movimiento o parado.
        if (velocidad.Length() > 0)
        {
            velocidad = velocidad.Normalized();
            velocidad *= this.Velocidad;

            this.AnimatedSprite2D.Play();
        }
        else
        {
            this.AnimatedSprite2D.Stop();
        }

        // Utilizar el valor delta asegura que el movimiento se mantenga consistente incluso si la velocidad de cuadros cambia.
        this.Position += velocidad * (float)delta;

        // Evitamos que el jugador se salga de la pantalla.
        this.Position = new Vector2(x: Mathf.Clamp(Position.X, 0, TamanoPantalla.X), y: Mathf.Clamp(Position.Y, 0, TamanoPantalla.Y));

        // Rotamos el sprite a la dirección acorde a la velocidad de cada eje.
        RotarSpriteADireccion(velocidad, this.AnimatedSprite2D);
    }

    private static Vector2 CalcularVectorVelocidadMedianteBotonesPulsados()
    {
        var velocity = Vector2.Zero; // El vector de movimiento del jugador.

        if (Input.IsActionPressed(ConstantesAcciones.RIGHT))
            velocity.X += 1;

        if (Input.IsActionPressed(ConstantesAcciones.LEFT))
            velocity.X -= 1;

        if (Input.IsActionPressed(ConstantesAcciones.DOWN))
            velocity.Y += 1;

        if (Input.IsActionPressed(ConstantesAcciones.UP))
            velocity.Y -= 1;

        return velocity;
    }

    private static void RotarSpriteADireccion(Vector2 velocity, AnimatedSprite2D animatedSprite2D)
    {
        // 8 direcciones

        //     -
        // -   X   +
        //     +

        // NW  N  NE
        // W   X   E
        // SW  S  SE

        // E
        if (velocity.X > 0 && velocity.Y == 0)
        {
            animatedSprite2D.RotationDegrees = 90;
        }
        // NE
        else if (velocity.X > 0 && velocity.Y < 0)
        {
            animatedSprite2D.RotationDegrees = 45;
        }
        // N
        else if (velocity.X == 0 && velocity.Y < 0)
        {
            animatedSprite2D.RotationDegrees = 0;
        }
        // NW
        else if (velocity.X < 0 && velocity.Y < 0)
        {
            animatedSprite2D.RotationDegrees = -45;
        }
        // W
        else if (velocity.X < 0 && velocity.Y == 0)
        {
            animatedSprite2D.RotationDegrees = -90;
        }
        // SW
        else if (velocity.X < 0 && velocity.Y > 0)
        {
            animatedSprite2D.RotationDegrees = -135;
        }
        // S
        else if (velocity.X == 0 && velocity.Y > 0)
        {
            animatedSprite2D.RotationDegrees = 180;
        }
        // SE
        else if (velocity.X > 0 && velocity.Y > 0)
        {
            animatedSprite2D.RotationDegrees = 135;
        }
    }

    private void ProcessPowerUps(double delta)
    {
        foreach (var powerUp in PowerUpsActivos.ToList())
        {
            powerUp.ProcessPowerUp(delta, this);
        }
    }

    public void Start(Vector2 position)
    {
        LimpiarPowerUps();

        // Ponemos al jugador en la posición inicial indicada.
        this.Position = position;

        // Mostramos y activamos las colisiones del jugador.
        Show();

        this.Muerto = false;

        this.CollisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Enemigo)
        {
            OnBodyEnteredEnemigo();
        }
    }

    private async void OnBodyEnteredEnemigo()
    {
        if (this.Invulnerable)
        {
            LoggerJuego.Info("Jugador golpeado por enemigo pero es invulnerable.");
            return;
        }

        LoggerJuego.Info("Jugador golpeado por enemigo.");

        LimpiarPowerUps();

        // Desactivamos la colisión para que la señal no se siga emitiendo.
        // Debe ser diferido ya que no podemos cambiar las propiedades físicas en un callback de física.
        this.CollisionShape2D.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);

        // Marcamos al jugador como muerto.
        this.Muerto = true;

        // Emitimos la señal de que hemos sido golpeados y esperamos dos segundos.
        EmitSignal(SignalName.MuerteJugador);

        Global.GestorAudio.ReproducirSonido("game_over_arcade.mp3");

        // Iniciamos la animación de muerte del jugador.
        await AnimacionMuerte();
    }

    private async Task AnimacionMuerte()
    {
        // Provocamos un shacek de la cámara.
        Juego.Camara?.AddTrauma(0.3f);

        // Paramamos la animación del sprite y cambiamos el color a rojo.
        this.AnimatedSprite2D.Stop();
        this.AnimatedSprite2D.Modulate = new Color(ConstantesColores.ROJO_PASTEL);

        await UtilidadesNodos.EsperarSegundos(this, 0.5);

        // Escondemos el sprite del jugador.
        AnimatedSprite2D.Hide();
        // Restauramos el color original del sprite.
        this.AnimatedSprite2D.Modulate = new Color(1, 1, 1);

        ExplosionMuerte.Show();
        ExplosionMuerte.Emitting = true;

        await UtilidadesNodos.EsperarSegundos(this, 2.0);

        EmitSignal(SignalName.AnimacionMuerteJugadorTerminada);
    }

    public void SetSpriteAlpha(float alpha)
    {
        var color = AnimatedSprite2D.Modulate;
        color.A = alpha;
        AnimatedSprite2D.Modulate = color;
    }
    public void SetSpriteColor(Color color)
    {
        AnimatedSprite2D.Modulate = color;
    }

    #region PowerUps

    // Añade un PowerUp como hijo del jugador
    internal void AnadirPowerUp(PowerUp powerUp)
    {
        if (powerUp == null) return;
        if (_PowerUpsActivos.Contains(powerUp)) return;

        powerUp.Reparent(this);
        _PowerUpsActivos.Add(powerUp);
    }

    // Comprueba si hay un PowerUp del tipo exacto
    internal bool TienePowerUpDeTipo(Type type)
    {
        if (type == null) return false;
        return PowerUpsActivos.Any(p => p.GetType() == type);
    }

    // Comprueba si hay un PowerUp del tipo T
    public bool TienePowerUpDeTipo<T>() where T : PowerUp
    {
        return PowerUpsActivos.OfType<T>().Any();
    }

    // Comprueba si existe esta instancia concreta de PowerUp
    internal bool TienePowerUp(PowerUp powerUp)
    {
        return powerUp?.GetParent() == this;
    }

    // Obtiene todos los PowerUps del tipo T
    public List<T> ObtenerPowerUps<T>() where T : PowerUp
    {
        return PowerUpsActivos.OfType<T>().ToList();
    }

    // Obtiene el primer PowerUp del tipo T
    public T ObtenerPowerUp<T>() where T : PowerUp
    {
        return PowerUpsActivos.OfType<T>().FirstOrDefault();
    }

    // Obtiene el primer PowerUp del tipo indicado
    public PowerUp ObtenerPowerUp(Type type)
    {
        if (type == null) return null;
        return PowerUpsActivos.FirstOrDefault(p => p.GetType() == type);
    }

    // Quita todos los PowerUps de un tipo específico
    internal void QuitarPowerUps(Type type)
    {
        if (type == null) return;
        foreach (var powerUp in _PowerUpsActivos.ToArray())
        {
            if (powerUp.GetType() == type)
                QuitarPowerUp(powerUp);
        }
    }

    // Quita un PowerUp específico (por instancia)
    internal void QuitarPowerUp(PowerUp powerUp)
    {
        if (powerUp == null) return;
        if (!_PowerUpsActivos.Remove(powerUp)) return;
        if (powerUp.GetParent() != this) return;

        RemoveChild(powerUp);
        powerUp.CallDeferred(Node.MethodName.QueueFree);
    }

    public void QuitarPowerUps<T>() where T : PowerUp
    {
        foreach (var powerUp in ObtenerPowerUps<T>())
            QuitarPowerUp(powerUp);
    }

    private void LimpiarPowerUps()
    {
        foreach (var powerUp in _PowerUpsActivos.ToList())
        {
            QuitarPowerUp(powerUp);
        }

        List<PowerUp> powerUps = UtilidadesNodos.ObtenerNodosDeTipo<PowerUp>(this);
        if (powerUps.Any())
        {
            LoggerJuego.Warn("Existen PowerUps en Jugador no registrados en la lista de PowerUpsActivos.");
            foreach (var powerUp in powerUps.ToList())
            {
                RemoveChild(powerUp);
                powerUp.CallDeferred(Node.MethodName.QueueFree);
            }
        }
    }
    #endregion
}
