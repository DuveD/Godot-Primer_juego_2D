namespace Primerjuego2D.escenas.entidades.jugador;

using Godot;
using Primerjuego2D.escenas.entidades.enemigo;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;


public partial class Jugador : Area2D
{
    [Export]
    public int Speed { get; set; } = 400; // Velocidad de movimiento del jugador (pixels/sec).

    public bool Muerto { get; private set; } = false;


    // Señal "MuerteJugador" para indicar colisión con el jugador.
    [Signal]
    public delegate void MuerteJugadorEventHandler();

    public const string ANIMATION_UP = "up";

    public const string ANIMATION_WALK = "walk";

    private CollisionShape2D _CollisionShape2D;
    private CollisionShape2D CollisionShape2D => _CollisionShape2D ??= GetNode<CollisionShape2D>("CollisionShape2D");

    private AnimatedSprite2D _AnimatedSprite2D;
    private AnimatedSprite2D AnimatedSprite2D => _AnimatedSprite2D ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");


    private Vector2 TamanoPantalla => UtilidadesPantalla.ObtenerTamanoPantalla(this);

    // Se llama cuando el nodo entra por primera vez en el árbol de escenas.
    public override void _Ready()
    {
        Logger.Trace("Jugador Ready.");

        // Oculatamos el sprite al inicio de la partida.
        this.Hide();

        this.CollisionShape2D.SetDeferred(nameof(CollisionShape2D.Disabled), true);
    }

    // Se llama en cada fotograma. 'delta' es el tiempo transcurrido desde el fotograma anterior.
    public override void _Process(double delta)
    {
        if (this.Muerto)
            return;

        var velocity = CalcularVectorVelocidadMedianteBotonesPulsados();

        // Indicamos la animación inicial para que mire hacia arriba.
        //this.AnimatedSprite2D.Animation = ANIMATION_UP;

        // Idicamos la animación según si está en movimiento o parado.
        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            this.AnimatedSprite2D.Play();
        }
        else
        {
            this.AnimatedSprite2D.Stop();
        }

        // Utilizar el valor delta asegura que el movimiento se mantenga consistente incluso si la velocidad de cuadros cambia.
        this.Position += velocity * (float)delta;

        // Evitamos que el jugador se salga de la pantalla.
        this.Position = new Vector2(x: Mathf.Clamp(Position.X, 0, TamanoPantalla.X), y: Mathf.Clamp(Position.Y, 0, TamanoPantalla.Y));

        // Rotamos el sprite a la dirección acorde a la velocidad de cada eje.
        RotarSpriteADireccion(velocity, this.AnimatedSprite2D);
    }

    private static Vector2 CalcularVectorVelocidadMedianteBotonesPulsados()
    {
        var velocity = Vector2.Zero; // El vector de movimiento del jugador.

        if (Input.IsActionPressed(ConstantesAcciones.MOVE_RIGHT))
            velocity.X += 1;

        if (Input.IsActionPressed(ConstantesAcciones.MOVE_LEFT))
            velocity.X -= 1;

        if (Input.IsActionPressed(ConstantesAcciones.MOVE_DOWN))
            velocity.Y += 1;

        if (Input.IsActionPressed(ConstantesAcciones.MOVE_UP))
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

    public void Start(Vector2 position)
    {
        // Ponemos al jugador en la posición inicial indicada.
        this.Position = position;

        // Mostramos y activamos las colisiones del jugador.
        Show();

        this.Muerto = false;

        this.CollisionShape2D.SetDeferred(nameof(CollisionShape2D.Disabled), false);
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
        // Desactivamos la colisión para que la señal no se siga emitiendo.
        // Debe ser diferido ya que no podemos cambiar las propiedades físicas en un callback de física.
        this.CollisionShape2D.SetDeferred(nameof(CollisionShape2D.Disabled), true);

        // Marcamos al jugador como muerto.
        this.Muerto = true;

        // Paramamos la animación del sprite y cambiamos el color a rojo.
        this.AnimatedSprite2D.Stop();
        this.AnimatedSprite2D.Modulate = new Color(ConstantesColores.ROJO_PASTEL);

        // Emitimos la señal de que hemos sido golpeados y esperamos dos segundos.
        EmitSignal(SignalName.MuerteJugador);
        await UtilidadesNodos.EsperarSegundos(this, 2.0);

        // Escondemos el sprite del jugador.
        Hide();

        // Restauramos el color original del sprite.
        this.AnimatedSprite2D.Modulate = new Color(1, 1, 1);
    }
}
