using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;

namespace Primerjuego2D.escenas.sistema.audio;

[AtributoNivelLog(NivelLog.Info)]
public partial class GestorAudio : Node
{
	#region Control de buses de audio
	public const string BUS_MASTER = "Master";
	public const string BUS_MUSICA = "Musica";
	public const string BUS_SONIDOS = "Sonido";
	private readonly Dictionary<string, int> _indicesBuses = new();
	#endregion

	public const string RUTA_MUSICA = "res://recursos/audio/musica";

	public const string RUTA_SONIDOS = "res://recursos/audio/sonidos";

	public readonly string[] ExtensionesAceptadas = [".mp3", ".wav", ".ogg"];

	private int _indiceCanalSonido = 0;

	[Export]
	private int _numeroCanalesSonido { get; set; } = 5;

	private readonly List<AudioStreamPlayer> _poolCanalesSonidos = new();

	private readonly HashSet<AudioStreamPlayer> _canalesOcupados = new();

	private Node ContenedorCanalesSonidos;

	private AudioStreamPlayer AudioStreamPlayer;
	private AudioStreamPlayer AudioStreamPlayer2;

	private readonly Dictionary<string, AudioStream> _recursosMusica = new();

	private readonly Dictionary<string, AudioStream> _recursosSonidos = new();

	#region Control de reproducción de música
	private float _posicionPausa = 0f;

	private bool _crossfadeEnProceso = false;

	private bool _fadeEnProceso = false;

	private Queue<string> _colaCrossfade = new();
	#endregion

	#region Controles de volumen
	private float _VolumenGeneral;
	public float VolumenGeneral
	{
		get => _VolumenGeneral;
		set
		{
			_VolumenGeneral = Mathf.Clamp(value, 0f, 1f);
			RecalcularVolumenes();
		}
	}

	public float _VolumenMusica;
	public float VolumenMusica
	{
		get => _VolumenMusica;
		set
		{
			_VolumenMusica = Mathf.Clamp(value, 0f, 1f);
			RecalcularVolumenes();
		}
	}

	public float _VolumenSonidos;
	public float VolumenSonidos
	{
		get => _VolumenSonidos;
		set
		{
			_VolumenSonidos = Mathf.Clamp(value, 0f, 1f);
			RecalcularVolumenes();
		}
	}

	private void RecalcularVolumenes()
	{
		// Master siempre recibe el volumen general
		AjustarVolumenBus(BUS_MASTER, _VolumenGeneral);

		// Música y sonidos son relativos al general
		AjustarVolumenBus(BUS_MUSICA, _VolumenMusica * _VolumenGeneral);
		AjustarVolumenBus(BUS_SONIDOS, _VolumenSonidos * _VolumenGeneral);
	}
	#endregion

	public override void _Ready()
	{
		LoggerJuego.Trace(this.Name + " Ready.");

		InicializarCanalesAudio();
		InicializarPoolCanalesSonidos();

		InicializarVolumen();

		CargarRecursos();
	}

	public int ObtenerIndiceBus(string nombreBus)
	{
		if (_indicesBuses.TryGetValue(nombreBus, out var indice))
			return indice;

		indice = AudioServer.GetBusIndex(nombreBus);
		if (indice >= 0)
		{
			_indicesBuses[nombreBus] = indice;
			return indice;
		}
		else
		{
			LoggerJuego.Error($"Bus de audio no encontrado: '{nombreBus}'");
			return -1;
		}
	}

	private void InicializarCanalesAudio()
	{
		AudioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		AudioStreamPlayer.Bus = BUS_MUSICA;

		AudioStreamPlayer2 = GetNode<AudioStreamPlayer>("AudioStreamPlayer2");
		AudioStreamPlayer2.Bus = BUS_MUSICA;
	}

	private void InicializarPoolCanalesSonidos()
	{
		_poolCanalesSonidos.Clear();

		ContenedorCanalesSonidos = GetNode<Node>("ContenedorCanalesSonidos");

		for (int i = 0; i < _numeroCanalesSonido; i++)
		{
			var player = new AudioStreamPlayer
			{
				Bus = BUS_SONIDOS
			};
			player.Finished += () => LiberarCanalSonido(player);

			_poolCanalesSonidos.Add(player);

			ContenedorCanalesSonidos.AddChild(player);
		}
	}

	private void LiberarCanalSonido(AudioStreamPlayer player)
	{
		// ...para marcarlo como libre.
		_canalesOcupados.Remove(player);
		player.Stream = null;
		LoggerJuego.Trace($"Player '{player.Name}' terminado.");
	}

	private void InicializarVolumen()
	{
		// Inicializamos los volúmenes desde los ajustes.
		_VolumenGeneral = Ajustes.VolumenGeneral / 100.0f;
		_VolumenMusica = Ajustes.VolumenMusica / 100.0f;
		_VolumenSonidos = Ajustes.VolumenSonidos / 100.0f;

		RecalcularVolumenes();
	}

	private void CargarRecursos()
	{
		// Cargar música
		CargarRecursosMusica();

		// Cargar efectos
		CargarRecursosSonidos();
	}

	private void CargarRecursosMusica()
	{
		var archivosMusica = DirAccess.Open(RUTA_MUSICA).GetFiles();
		if (archivosMusica == null)
		{
			LoggerJuego.Error($"No se pudo abrir la carpeta: {RUTA_MUSICA}");
		}
		else
		{
			archivosMusica = FiltrarExtensionesAceptadas(archivosMusica);
			foreach (string nombreArchivoMusica in archivosMusica)
			{
				var musica = ObtenerMusica(nombreArchivoMusica);
				if (musica != null)
					_recursosMusica[nombreArchivoMusica] = musica;
			}
		}
	}

	private string ObtenerNombreCancionActual()
	{
		if (AudioStreamPlayer.Stream == null) return "<ninguna>";
		return _recursosMusica.FirstOrDefault(kv => kv.Value == AudioStreamPlayer.Stream).Key ?? "<desconocida>";
	}

	private void CargarRecursosSonidos()
	{
		var archivosSonidos = DirAccess.Open(RUTA_SONIDOS).GetFiles();
		if (archivosSonidos == null)
		{
			LoggerJuego.Error($"No se pudo abrir la carpeta: {RUTA_SONIDOS}");
		}
		else
		{
			archivosSonidos = FiltrarExtensionesAceptadas(archivosSonidos);
			foreach (string nombreArchivoSonido in archivosSonidos)
			{
				var sonido = ObtenerSonido(nombreArchivoSonido);
				if (sonido != null)
					_recursosSonidos[nombreArchivoSonido] = sonido;
			}
		}
	}

	private void AjustarVolumenBus(string busName, float linear)
	{
		int busIndex = ObtenerIndiceBus(busName);
		if (busIndex < 0) return;

		linear = Mathf.Max(linear, 0.0001f);
		float db = Mathf.LinearToDb(linear);

		AudioServer.SetBusVolumeDb(busIndex, db);
	}

	private string[] FiltrarExtensionesAceptadas(string[] nombresArchivos)
	{
		if (nombresArchivos == null)
			return [];

		return nombresArchivos.Where(nombreArchivo => ExtensionesAceptadas.Contains(System.IO.Path.GetExtension(nombreArchivo).ToLower())).ToArray();
	}
	private AudioStream ObtenerSonido(string nombreArchivoSonido)
	{
		return ObtenerAudio(RUTA_SONIDOS + "/" + nombreArchivoSonido);
	}

	private AudioStream ObtenerMusica(string nombreArchivoMusica)
	{
		return ObtenerAudio(RUTA_MUSICA + "/" + nombreArchivoMusica);
	}

	private AudioStream ObtenerAudio(string rutaArchivoAudio)
	{
		return ResourceLoader.Load<AudioStream>(rutaArchivoAudio, cacheMode: ResourceLoader.CacheMode.Reuse);
	}

	private AudioStreamPlayer BuscarCanalSonidoLibre()
	{
		// 🔍 Buscamos un canal que no esté reproduciendo nada
		foreach (var canal in _poolCanalesSonidos)
		{
			if (!_canalesOcupados.Contains(canal))
				return canal;
		}

		// 🔄 Si no hay libres, uso circular como fallback
		var canal2 = _poolCanalesSonidos[_indiceCanalSonido];
		_indiceCanalSonido = (_indiceCanalSonido + 1) % _poolCanalesSonidos.Count;
		return canal2;
	}

	public void ReproducirSonido(string nombreSonido)
	{
		if (!_recursosSonidos.TryGetValue(nombreSonido, out var sonido))
		{
			LoggerJuego.Error($"Efecto no encontrado: '{nombreSonido}'");
			return;
		}

		var player = BuscarCanalSonidoLibre();
		_canalesOcupados.Add(player);
		player.Stream = sonido;
		player.Play();

		LoggerJuego.Trace($"Reproduciendo sonido '{nombreSonido}'.");
	}

	public async void ReproducirMusica(string nombreCancion, float duracionFade = 0f)
	{
		// Comprobamos que la canción exista
		if (!_recursosMusica.TryGetValue(nombreCancion, out var cancion))
		{
			LoggerJuego.Error($"Música no encontrada: '{nombreCancion}'");
			return;
		}

		// Esperamos a que termine un crossfade o un fade en proceso
		while (_fadeEnProceso || _crossfadeEnProceso)
			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

		_fadeEnProceso = true;

		var player = AudioStreamPlayer; // player principal
		bool reanudando = player.Stream == cancion && _posicionPausa > 0f;
		float posicionInicial = reanudando ? _posicionPausa : 0f;

		// Asignamos la canción al player
		player.Stream = cancion;

		// Reproducimos desde la posición adecuada
		player.Play(posicionInicial);
		_posicionPausa = 0f;

		// Si no hay fade, dejamos volumen neutro (el bus manda)
		if (duracionFade <= 0f)
		{
			player.VolumeDb = 0f;
			LoggerJuego.Trace($"Reproduciendo música '{nombreCancion}' sin fade.");
		}
		else
		{
			LoggerJuego.Trace($"Reanudando música '{nombreCancion}' con fade-in.");
			player.VolumeDb = -80f;

			float tiempo = 0f;
			while (tiempo < duracionFade)
			{
				float t = tiempo / duracionFade;
				player.VolumeDb = Mathf.Lerp(-80f, 0f, t);

				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
				tiempo += (float)GetPhysicsProcessDeltaTime();
			}

			// Aseguramos volumen final
			player.VolumeDb = 0f;
			LoggerJuego.Trace($"Música '{nombreCancion}' reanudada con fade-in.");
		}

		_fadeEnProceso = false;
	}

	public bool MusicaReproduciendo()
	{
		return AudioStreamPlayer.Playing;
	}

	/// <summary>
	/// Pausa la música actual, permitiendo reanudarla después.
	/// </summary>
	public async void PausarMusica(float duracionFade = 0f)
	{
		await PausarMusicaInterno(guardarPosicion: true, duracionFade);
	}

	/// <summary>
	/// Para la música actual y resetea la posición, no se podrá reanudar.
	/// </summary>
	public async void PararMusica(float duracionFade = 0f)
	{
		await PausarMusicaInterno(guardarPosicion: false, duracionFade);
	}

	// Para pausar la música, guardamos la posición actual y detenemos la reproducción.
	/// <summary>
	/// Pausa o para la música, con fade-out opcional.
	/// </summary>
	/// <param name="guardarPosicion">Si es true, guarda la posición para reanudar; si es false, resetea.</param>
	/// <param name="duracionFade">Duración del fade-out en segundos.</param>
	private async Task PausarMusicaInterno(bool guardarPosicion, float duracionFade = 0f)
	{
		if (_fadeEnProceso || !AudioStreamPlayer.Playing) return;

		_fadeEnProceso = true;

		// Guardamos posición solo si se debe reanudar después
		if (guardarPosicion)
			_posicionPausa = AudioStreamPlayer.GetPlaybackPosition();
		else
			_posicionPausa = 0f;

		// Fade-out opcional
		if (duracionFade > 0f)
		{
			LoggerJuego.Trace($"Iniciando fade-out para la canción '{ObtenerNombreCancionActual()}'.");

			float inicioVol = AudioStreamPlayer.VolumeDb;
			float tiempo = 0f;

			while (tiempo < duracionFade)
			{
				float t = tiempo / duracionFade;
				AudioStreamPlayer.VolumeDb = Mathf.Lerp(inicioVol, -80f, t);

				await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
				tiempo += (float)GetPhysicsProcessDeltaTime();
			}

			LoggerJuego.Trace($"Fade-out terminado para la canción '{ObtenerNombreCancionActual()}'.");

			// Aseguramos volumen final
			AudioStreamPlayer.VolumeDb = -80f;
		}

		AudioStreamPlayer.Stop();

		_fadeEnProceso = false;
	}

	// Cross-fade entre la canción actual y la nueva durante la duración especificada.
	public void Crossfade(string nombreCancion, float duracion = 2.0f)
	{
		if (string.IsNullOrEmpty(nombreCancion))
			return;

		if (!_recursosMusica.TryGetValue(nombreCancion, out var cancion))
		{
			LoggerJuego.Error($"Música no encontrada: {nombreCancion}");
			return;
		}

		if (AudioStreamPlayer.Stream == cancion)
		{
			LoggerJuego.Trace($"La canción '{nombreCancion}' ya se está reproduciendo.");
			return;
		}

		_colaCrossfade.Enqueue(nombreCancion);
		if (!_crossfadeEnProceso)
			_ = EjecutarCrossfade(duracion);
	}
	private async Task EjecutarCrossfade(float duracion)
	{
		if (_crossfadeEnProceso)
		{
			LoggerJuego.Warn($"Ya hay un cross-fade en proceso.");
			return;
		}

		_crossfadeEnProceso = true;

		while (_colaCrossfade.Count > 0)
		{
			string nombreCancion = _colaCrossfade.Dequeue();
			if (!_recursosMusica.TryGetValue(nombreCancion, out var cancion))
			{
				LoggerJuego.Error($"Música no encontrada: {nombreCancion}.");
				continue;
			}

			LoggerJuego.Trace($"Iniciando cross-fade para: {nombreCancion}.");
			await CrossfadeInterno(cancion, duracion);
			LoggerJuego.Trace($"Cross-fade terminado para: {nombreCancion}.");
		}

		_crossfadeEnProceso = false;
	}

	private async Task CrossfadeInterno(AudioStream cancion, float duracion)
	{
		// Players claros: el actual y el nuevo
		var playerActual = AudioStreamPlayer;
		var playerNuevo = AudioStreamPlayer2;

		// Configuramos el player nuevo
		playerNuevo.Stream = cancion;
		playerNuevo.VolumeDb = -80f;
		playerNuevo.Play();

		float tiempo = 0f;
		while (tiempo < duracion)
		{
			float t = tiempo / duracion;
			playerActual.VolumeDb = Mathf.Lerp(0f, -80f, t);
			playerNuevo.VolumeDb = Mathf.Lerp(-80f, 0f, t);

			await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
			tiempo += (float)GetPhysicsProcessDeltaTime();
		}

		// Normalizamos volúmenes
		playerActual.VolumeDb = -80f;
		playerNuevo.VolumeDb = 0f;

		// Detenemos el antiguo player
		playerActual.Stop();
		playerActual.Stream = null;

		// Swap para mantener AudioStreamPlayer como el principal activo
		(AudioStreamPlayer, AudioStreamPlayer2) = (playerNuevo, playerActual);
	}

}
