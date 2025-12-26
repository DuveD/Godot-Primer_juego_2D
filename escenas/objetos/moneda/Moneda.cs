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
	// Señal "Recogida" para indicar que la moneda ha sido recogida por el jugador.

	[Signal]
	public delegate void RecogidaEventHandler();

	private CollisionShape2D _CollisionShape2D;

	private CollisionShape2D CollisionShape2D => _CollisionShape2D ??= GetNode<CollisionShape2D>("CollisionShape2D");

	private static readonly PackedScene TextoFlotanteScene = GD.Load<PackedScene>(UtilidadesNodos.ObtenerRutaEscena<TextoFlotante>());

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.OBJETOS);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Jugador)
		{
			OnBodyEnteredJugador();
		}
	}

	private void OnBodyEnteredJugador()
	{
		LoggerJuego.Info("Moneda recogida.");

		// Emitimos la señal de que el jugador ha recogido la moneda.

		EmitSignal(SignalName.Recogida);

		Global.GestorAudio.ReproducirSonido("retro_coin.mp3");

		// Mostramos el texto flotante al recoger la moneda.
		MostrarTextoFlotante();

		// Cuando el jugador recoja la moneda, la destruimos.
		QueueFree();
	}

	private void MostrarTextoFlotante()
	{
		var texto = TextoFlotanteScene.Instantiate<TextoFlotante>();

		texto.Texto = " +1";
		texto.Color = Colors.Gold;
		texto.PosicionGlobal = GlobalPosition;

		GetTree().CurrentScene.AddChild(texto);
	}
}