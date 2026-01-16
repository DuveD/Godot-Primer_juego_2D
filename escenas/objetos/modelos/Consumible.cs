using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.objetos.modelos;

public abstract partial class Consumible : Area2D
{
    private Sprite2D _Sprite2D;
    public Sprite2D Sprite2D => _Sprite2D ??= GetNode<Sprite2D>("Sprite2D");


    private CollisionShape2D _CollisionShape2D;
    public CollisionShape2D CollisionShape2D => _CollisionShape2D ??= GetNode<CollisionShape2D>("CollisionShape2D");

    // Si es -1, no se autodestruye. Si >0, se destruye automáticamente después de ese tiempo.

    [Export]
    public long TiempoDestruccion { get; set; } = -1;

    public Timer _TimerDestruccion;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;

        ConfigurarTimerDestruccion();
    }

    private void ConfigurarTimerDestruccion()
    {       // Configuramos timer de autodestrucción

        if (TiempoDestruccion > 0)
        {
            _TimerDestruccion = new Timer
            {
                WaitTime = TiempoDestruccion,
                OneShot = true,
                Autostart = true
            };
            _TimerDestruccion.Timeout += OnTimerDestruccionTimeout;
            AddChild(_TimerDestruccion);
        }
    }

    private void OnTimerDestruccionTimeout()
    {
        LoggerJuego.Trace(this.Name + " autodestruido tras " + TiempoDestruccion + " segundos.");

        // Usamos CallDeferred para que no choque con signals o procesamiento actual.

        CallDeferred(Node.MethodName.QueueFree);
    }
    private void DetenerYEliminarTimer()
    {
        if (_TimerDestruccion == null)
            return;

        _TimerDestruccion.Stop();
        _TimerDestruccion.QueueFree();
        _TimerDestruccion = null;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Jugador jugador)
        {
            DetenerYEliminarTimer();
            OnRecogida(jugador);
        }
    }

    public abstract void OnRecogida(Jugador jugador);

    public float ObtenerRadioCollisionShape2D()
    {
        return ((CircleShape2D)CollisionShape2D?.Shape)!.Radius;
    }
}