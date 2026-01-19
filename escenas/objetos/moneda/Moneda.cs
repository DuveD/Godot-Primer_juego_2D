using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.miscelaneo;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.objetos.moneda;

[AtributoNivelLog(NivelLog.Info)]
public partial class Moneda : Consumible
{
	[Signal]
	public delegate void RecogidaEventHandler(Moneda moneda);

	[Export]
	public int Valor { get; set; } = 1;

	[Export]
	public float VelocidadAnimacion { get; set; } = 1.0f;

	[Export]
	public Color ColorTextoFlotante { get; set; } = Colors.Gold;

	public static readonly PackedScene TextoFlotanteScene = GD.Load<PackedScene>(UtilidadesNodos.ObtenerRutaEscena<TextoFlotante>());

	private AnimationPlayer _AnimationPlayerRotacion;
	public AnimationPlayer AnimationPlayerRotacion => _AnimationPlayerRotacion ??= GetNode<AnimationPlayer>("AnimationPlayerRotacion");

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		base._Ready();

		UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.OBJETOS);

		this.AnimationPlayerRotacion.SpeedScale = this.VelocidadAnimacion;
	}

	public override void OnRecogida(Jugador jugador)
	{
		LoggerJuego.Info("Moneda (" + this.Valor + ") recogida.");

		EmitSignal(SignalName.Recogida, this);

		Global.GestorAudio.ReproducirSonido("retro_coin.mp3");

		MostrarTextoFlotante();

		// Cancelamos el timer si estaba activo.

		TimerDestruccion?.Stop();

		// Usamos CallDeferred para evitar conflictos si el spawn ocurre durante la señal.
		CallDeferred(Node.MethodName.QueueFree);
	}

	public virtual void MostrarTextoFlotante()
	{
		var texto = TextoFlotanteScene.Instantiate<TextoFlotante>();

		texto.Texto = " +" + this.Valor.ToString();
		texto.Color = ColorTextoFlotante;
		texto.PosicionGlobal = GlobalPosition;

		GetTree().CurrentScene.AddChild(texto);
	}
}
