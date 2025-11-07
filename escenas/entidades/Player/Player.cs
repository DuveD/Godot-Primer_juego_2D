using Godot;
public partial class Player : Area2D
{
    // Señal "HitByEnemy" para indicar colisión con el jugador.
    [Signal]
    public delegate void HitByEnemyEventHandler();

    public const string ANIMATION_UP = "up";

    public const string ANIMATION_WALK = "walk";

    private CollisionShape2D _CollisionShape2D;
    private CollisionShape2D CollisionShape2D => _CollisionShape2D ??= GetNode<CollisionShape2D>("CollisionShape2D");

    private AnimatedSprite2D _AnimatedSprite2D;
    private AnimatedSprite2D AnimatedSprite2D => _AnimatedSprite2D ??= GetNode<AnimatedSprite2D>("AnimatedSprite2D");


    [Export]
    public int Speed { get; set; } = 400; // Velocidad de movimiento dle jugador (pixels/sec).

    private Vector2 ScreenSize => GetViewportRect().Size;

    // Se llama cuando el nodo entra por primera vez en el árbol de escenas.
    public override void _Ready()
    {
        // Oculatamos el sprite al inicio de la partida.
        this.Hide();

        this.CollisionShape2D.SetDeferred(nameof(CollisionShape2D.Disabled), true);
    }

    // Se llama en cada fotograma. 'delta' es el tiempo transcurrido desde el fotograma anterior.
    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero; // El vector de movimiento del jugador.

        if (Input.IsActionPressed(ConstantesAcciones.MOVE_RIGHT))
        {
            velocity.X += 1;
        }
        if (Input.IsActionPressed(ConstantesAcciones.MOVE_LEFT))
        {
            velocity.X -= 1;
        }
        if (Input.IsActionPressed(ConstantesAcciones.MOVE_DOWN))
        {
            velocity.Y += 1;
        }
        if (Input.IsActionPressed(ConstantesAcciones.MOVE_UP))
        {
            velocity.Y -= 1;
        }

        this.AnimatedSprite2D.Animation = ANIMATION_UP;

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            this.AnimatedSprite2D.Play();
        }
        else
        {
            this.AnimatedSprite2D.Stop();
        }

        RotateSpriteToDirection(velocity, this.AnimatedSprite2D);

        // Utilizar el valor delta asegura que el movimiento se mantenga consistente incluso si la velocidad de cuadros cambia.
        this.Position += velocity * (float)delta;

        // Evitamos que el jugador se salga de la pantalla.
        this.Position = new Vector2(x: Mathf.Clamp(Position.X, 0, ScreenSize.X), y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y));
    }

    private static void RotateSpriteToDirection(Vector2 velocity, AnimatedSprite2D animatedSprite2D)
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

        this.CollisionShape2D.SetDeferred(nameof(CollisionShape2D.Disabled), false);
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Enemy)
        {
            OnEnemyBodyEntered();
        }
    }

    private void OnEnemyBodyEntered()
    {
        // El jugador desaparece después de ser golpeado.
        Hide();

        // Emitimos la señal de que hemos sido golpeados.
        EmitSignal(SignalName.HitByEnemy);

        // Desactivamos la colisión para que la señal no se siga emitiendo.
        // Debe ser diferido ya que no podemos cambiar las propiedades físicas en un callback de física.
        this.CollisionShape2D.SetDeferred(nameof(CollisionShape2D.Disabled), true);
    }

}
