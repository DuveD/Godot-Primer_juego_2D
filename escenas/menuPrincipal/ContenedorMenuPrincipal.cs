using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.escenas.menuPrincipal.botones;
using Primerjuego2D.escenas.miscelaneo.animaciones;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuPrincipal : ContenedorMenu
{
    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    private ButtonEmpezarPartida _ButtonEmpezarPartida;
    private ButtonAjustes _ButtonAjustes;
    private ButtonEstadisticas _ButtonEstadisticas;
    private ButtonSalir _ButtonSalir;

    private AnimacionCrtShutdown _AnimacionCrtShutdown;


    public ButtonPersonalizado UltimoBotonPulsado;

    public override void _Ready()
    {
        base._Ready();

        _ButtonEmpezarPartida = UtilidadesNodos.ObtenerNodoPorNombre<ButtonEmpezarPartida>(this, "ButtonEmpezarPartida");
        _ButtonAjustes = UtilidadesNodos.ObtenerNodoPorNombre<ButtonAjustes>(this, "ButtonAjustes");
        _ButtonEstadisticas = UtilidadesNodos.ObtenerNodoPorNombre<ButtonEstadisticas>(this, "ButtonEstadisticas");
        _ButtonSalir = UtilidadesNodos.ObtenerNodoPorNombre<ButtonSalir>(this, "ButtonSalir");
        _AnimacionCrtShutdown = GetNode<AnimacionCrtShutdown>("../AnimacionCrtShutdown");

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [.. UtilidadesNodos.ObtenerNodosDeTipo<ButtonPersonalizado>(this)];
    }

    protected override void PostReady()
    {
        base.PostReady();

        ConfigurarBotonesMenu();

        LoggerJuego.Trace(this.Name + " PostReady.");
    }

    private void ConfigurarBotonesMenu()
    {
        LoggerJuego.Trace("Configuramos el focus de los botones del menú.");

        foreach (var boton in this.ElementosConFoco.OfType<ButtonPersonalizado>())
            boton.Pressed += () => this.UltimoBotonPulsado = boton;
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return this.UltimoBotonPulsado ?? _ButtonEmpezarPartida;
    }

    private void OnButtonEmpezarPartidaPressedAnimationEnd()
    {
        LoggerJuego.Trace("Botón 'ButtonEmpezarPartida' pulsado.");

        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
    }

    private async void OnButtonSalirPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonSalir' pulsado.");

        Global.GestorAudio.PausarMusica(0.5f);

        _AnimacionCrtShutdown.Reproducir();

        await ToSignal(_AnimacionCrtShutdown, AnimacionCrtShutdown.SignalName.AnimacionFinalizada);
        await Task.Delay(300);

        this.GetTree().Quit();
    }
}
