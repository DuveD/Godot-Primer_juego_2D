using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.utilidades.log.LoggerJuego;
using ButtonPersonalizado = Primerjuego2D.escenas.ui.controles.ButtonPersonalizado;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuAjustes : ContenedorMenu
{
	private ControlSlider _ControlVolumenGeneral;
	public ControlSlider ControlVolumenGeneral => _ControlVolumenGeneral ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenGeneral");

	private ControlSlider _ControlVolumenMusica;
	private ControlSlider ControlVolumenMusica => _ControlVolumenMusica ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenMusica");

	private ControlSlider _ControlVolumenSonido;
	private ControlSlider ControlVolumenSonido => _ControlVolumenSonido ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSlider>(this, "ControlVolumenSonido");

	private ControlSeleccion _ControlLenguaje;
	private ControlSeleccion ControlLenguaje => _ControlLenguaje ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSeleccion>(this, "ControlLenguaje");

	private ControlSeleccion _ControlNivelLog;
	private ControlSeleccion ControlNivelLog => _ControlNivelLog ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlSeleccion>(this, "ControlNivelLog");

	private ControlCheckButton _ControlEscribirLogEnFichero;
	private ControlCheckButton ControlEscribirLogEnFichero => _ControlEscribirLogEnFichero ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlCheckButton>(this, "ControlEscribirLogEnFichero");

	private ControlCheckButton _ControlVerColisiones;
	private ControlCheckButton ControlVerColisiones => _ControlVerColisiones ??= UtilidadesNodos.ObtenerNodoPorNombre<ControlCheckButton>(this, "ControlVerColisiones");

	private ButtonPersonalizado _ButtonAtras;
	private ButtonPersonalizado ButtonAtras => _ButtonAtras ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

	private ButtonPersonalizado _ButtonGuardar;
	private ButtonPersonalizado ButtonGuardar => _ButtonGuardar ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonGuardar");

	#region Valor ajustes
	public int VolumenGeneral;
	public int VolumenMusica;
	public int VolumenSonidos;
	public Idioma Lenguaje;
	public NivelLog NivelLog;
	public bool EscribirLogEnFichero;
	public bool VerColisiones;
	#endregion

	public override void _Ready()
	{
		base._Ready();

		LoggerJuego.Trace(this.Name + " Ready.");

		CargarOpcionesLenguaje();
		CargarOpcionesNivelLog();

		// Cargar ajustes actuales.

		CargarValoresDeAjustes();
	}

	public override Control ObtenerPrimerElementoConFoco()
	{
		return ControlVolumenGeneral.SliderVolumen;
	}

	public override List<Control> ObtenerElementosConFoco()
	{
		List<Control> elementosConFoco;
		elementosConFoco = [.. UtilidadesNodos.ObtenerNodosDeTipo<Button>(this)];
		elementosConFoco.AddRange(UtilidadesNodos.ObtenerNodosDeTipo<SpinBox>(this));
		elementosConFoco.AddRange(UtilidadesNodos.ObtenerNodosDeTipo<HSlider>(this));

		return elementosConFoco;
	}

	private void CargarOpcionesLenguaje()
	{
		var opcionesLenguajes = GestorIdioma.IdiomasDisponibles.Values.ToDictionary(
			idioma => (Variant)idioma.Codigo,
			idioma => idioma.TagNombre
		);
		ControlLenguaje.AgregarOpciones(opcionesLenguajes);
	}

	private void CargarOpcionesNivelLog()
	{
		var opcionesNivelLog = Enum.GetValues<NivelLog>().ToDictionary(
			nivel => (Variant)(int)nivel,
			nivel => nivel.ToString()
		);
		ControlNivelLog.AgregarOpciones(opcionesNivelLog);
	}

	private void CargarValoresDeAjustes()
	{
		ControlVolumenGeneral.ValorCambiado -= OnControlVolumenGeneralValorCambiado;
		ControlVolumenMusica.ValorCambiado -= OnControlVolumenMusicaValorCambiado;
		ControlVolumenSonido.ValorCambiado -= OnControlVolumenSonidosValorCambiado;
		ControlLenguaje.ValorCambiado -= OnControlLenguajeValorCambiado;
		ControlNivelLog.ValorCambiado -= OnControlNivelLogValorCambiado;

		VolumenGeneral = Ajustes.VolumenGeneral;
		ControlVolumenGeneral.Valor = VolumenGeneral;

		VolumenMusica = Ajustes.VolumenMusica;
		ControlVolumenMusica.Valor = VolumenMusica;

		VolumenSonidos = Ajustes.VolumenSonidos;
		ControlVolumenSonido.Valor = VolumenSonidos;

		Lenguaje = Ajustes.Idioma;
		ControlLenguaje.Valor = Ajustes.Idioma.Codigo;

		NivelLog = Ajustes.NivelLog;
		ControlNivelLog.Valor = (int)Ajustes.NivelLog;

		EscribirLogEnFichero = Ajustes.EscribirLogEnFichero;
		ControlEscribirLogEnFichero.Valor = EscribirLogEnFichero;

		VerColisiones = Ajustes.VerColisiones;
		ControlVerColisiones.Valor = VerColisiones;

		ControlVolumenGeneral.ValorCambiado += OnControlVolumenGeneralValorCambiado;
		ControlVolumenMusica.ValorCambiado += OnControlVolumenMusicaValorCambiado;
		ControlVolumenSonido.ValorCambiado += OnControlVolumenSonidosValorCambiado;
		ControlLenguaje.ValorCambiado += OnControlLenguajeValorCambiado;
		ControlNivelLog.ValorCambiado += OnControlNivelLogValorCambiado;
		ControlEscribirLogEnFichero.ValorCambiado += OnControlEscribirLogEnFicheroValorCambiado;
		ControlVerColisiones.ValorCambiado += OnControlVerColisionesValorCambiado;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		// Solo respondemos si el menú es visible.
		if (!this.Visible)
			return;

		if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
		{
			if (this.ModoNavegacionTeclado)
			{
				UtilidadesNodos.PulsarBoton(ButtonAtras);
				AcceptEvent();
			}
		}
	}

	private void ActivarBotonGuardarSiCambio()
	{
		bool hayCambios =
			!VolumenGeneral.Equals((int)ControlVolumenGeneral.Valor) ||
			!VolumenMusica.Equals((int)ControlVolumenMusica.Valor) ||
			!VolumenSonidos.Equals((int)ControlVolumenSonido.Valor) ||
			!Lenguaje.Codigo.Equals(ControlLenguaje.Valor.AsString()) ||
			!NivelLog.Equals((NivelLog)(int)ControlNivelLog.Valor);

		if (hayCambios)
		{
			ActivarNavegacionButtonGuardar();
		}
		else
		{
			DesactivarNavegacionButtonGuardar();
		}
	}

	private void ActivarNavegacionButtonGuardar()
	{
		ButtonGuardar.Disabled = false;
		ButtonGuardar.FocusMode = FocusModeEnum.All;

		// Informamos al ControlNivelLog que su vecino de abajo es el botón Guardar

		ControlNivelLog.OptionButton.FocusNeighborBottom = ControlNivelLog.OptionButton.GetPathTo(ButtonGuardar);
		// Informamos al botón Atrás que su vecino a la derecha es el botón Guardar

		ButtonAtras.FocusNeighborRight = ButtonAtras.GetPathTo(ButtonGuardar);
	}

	private void DesactivarNavegacionButtonGuardar()
	{
		ButtonGuardar.Disabled = true;
		ButtonGuardar.FocusMode = FocusModeEnum.None;

		// Informamos al ControlNivelLog que su vecino de abajo es el botón Atrás

		ControlNivelLog.OptionButton.FocusNeighborBottom = ControlNivelLog.OptionButton.GetPathTo(ButtonAtras);
		// Informamos al botón Atrás que su vecino a la derecha es el ControlNivelLog

		ButtonAtras.FocusNeighborRight = ButtonAtras.GetPathTo(ControlNivelLog.OptionButton);
	}

	private void OnControlVolumenGeneralValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenGeneral = (float)(valor / 100.0f);
		//ActivarBotonGuardarSiCambio();

		Ajustes.VolumenGeneral = (int)valor;
	}

	private void OnControlVolumenMusicaValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenMusica = (float)(valor / 100.0f);
		//ActivarBotonGuardarSiCambio();

		Ajustes.VolumenMusica = (int)valor;
	}

	private void OnControlVolumenSonidosValorCambiado(double valor)
	{
		Global.GestorAudio.VolumenSonidos = (float)(valor / 100.0f);
		//ActivarBotonGuardarSiCambio();

		Ajustes.VolumenSonidos = (int)valor;
	}

	private void OnControlLenguajeValorCambiado(Variant valor)
	{
		string codigoIdioma = (string)valor;
		Idioma idiomaSeleccionado = GestorIdioma.ObtenerIdiomaDeCodigo(codigoIdioma);
		GestorIdioma.CambiarIdioma(idiomaSeleccionado);
		//ActivarBotonGuardarSiCambio();

		Ajustes.Idioma = idiomaSeleccionado;
	}

	private void OnControlNivelLogValorCambiado(Variant valor)
	{
		NivelLog nivelLogSeleccionado = (NivelLog)(int)valor;
		LoggerJuego.NivelLogJuego = nivelLogSeleccionado;
		//ActivarBotonGuardarSiCambio();

		Ajustes.NivelLog = nivelLogSeleccionado;
	}

	private void OnControlEscribirLogEnFicheroValorCambiado(bool valor)
	{
		Ajustes.EscribirLogEnFichero = valor;
	}

	private void OnControlVerColisionesValorCambiado(bool valor)
	{
		Ajustes.VerColisiones = valor;

		GetTree().DebugCollisionsHint = valor;
	}

	public void OnButtonGuardarPressed()
	{
		LoggerJuego.Trace("Botón Ajustes 'Guardar' pulsado.");

		Ajustes.GuardarAjustesAlGuardarPropiedad = false;
		Ajustes.VolumenGeneral = (int)ControlVolumenGeneral.Valor;
		Ajustes.VolumenMusica = (int)ControlVolumenMusica.Valor;
		Ajustes.VolumenSonidos = (int)ControlVolumenSonido.Valor;
		Ajustes.Idioma = GestorIdioma.ObtenerIdiomaDeCodigo((string)ControlLenguaje.Valor);
		Ajustes.NivelLog = (NivelLog)(int)ControlNivelLog.Valor;

		Ajustes.GuardarAjustes();
		Ajustes.GuardarAjustesAlGuardarPropiedad = true;

		CargarValoresDeAjustes();

		DesactivarNavegacionButtonGuardar();

		ButtonAtras.GrabFocusSilencioso();
	}
}