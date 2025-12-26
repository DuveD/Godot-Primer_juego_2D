using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.miscelaneo;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.objetos.moneda;

[AtributoNivelLog(NivelLog.Info)]
public partial class Moneda : Area2D
{
	[Export]
	public int Valor { get; set; } = 1;

	[Export]
	public float VelocidadAnimacion { get; set; } = 1.0f;

	// Si es -1, no se autodestruye. Si >0, se destruye automáticamente después de ese tiempo.
	[Export]
	public float TiempoDestruccion { get; set; } = -1f;

	[Signal]
	public delegate void RecogidaEventHandler(Moneda moneda);

	private CollisionShape2D _CollisionShape2D;
	private CollisionShape2D CollisionShape2D => _CollisionShape2D ??= GetNode<CollisionShape2D>("CollisionShape2D");

	private static readonly PackedScene TextoFlotanteScene = GD.Load<PackedScene>(UtilidadesNodos.ObtenerRutaEscena<TextoFlotante>());

	private AnimationPlayer _AnimationPlayerRotacion;
	private AnimationPlayer AnimationPlayerRotacion => _AnimationPlayerRotacion ??= GetNode<AnimationPlayer>("AnimationPlayerRotacion");

	private Timer _TimerDestruccion;

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.OBJETOS);

		this.AnimationPlayerRotacion.SpeedScale = this.VelocidadAnimacion;

		// Configuramos timer de autodestrucción
		if (TiempoDestruccion > 0)
		{
			_TimerDestruccion = new Timer();
			_TimerDestruccion.WaitTime = TiempoDestruccion;
			_TimerDestruccion.OneShot = true;
			_TimerDestruccion.Autostart = true;
			_TimerDestruccion.Timeout += OnTimerDestruccionTimeout;
			AddChild(_TimerDestruccion);
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Jugador)
			OnBodyEnteredJugador();
	}

	private void OnBodyEnteredJugador()
	{
		LoggerJuego.Info("Moneda (" + this.Valor + ") recogida.");

		EmitSignal(SignalName.Recogida, this);

		Global.GestorAudio.ReproducirSonido("retro_coin.mp3");

		MostrarTextoFlotante();

		// Cancelamos el timer si estaba activo
		if (_TimerDestruccion != null)
			_TimerDestruccion.Stop();

		QueueFree();
	}

	private void OnTimerDestruccionTimeout()
	{
		LoggerJuego.Trace("Moneda autodestruida tras " + TiempoDestruccion + " segundos.");
		QueueFree();
	}

	private void MostrarTextoFlotante()
	{
		var texto = TextoFlotanteScene.Instantiate<TextoFlotante>();

		texto.Texto = " +" + this.Valor.ToString();
		texto.Color = Colors.Gold;
		texto.PosicionGlobal = GlobalPosition;

		GetTree().CurrentScene.AddChild(texto);
	}
}
