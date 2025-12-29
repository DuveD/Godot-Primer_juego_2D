using System;
using Godot;
using Primerjuego2D.escenas.modelos;
using Primerjuego2D.escenas.modelos.controles;
using Primerjuego2D.escenas.modelos.interfaces;
using Primerjuego2D.nucleo.configuracion;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

public partial class ContenedorMenuEstadisticas : ContenedorMenu
{
    private Label _LabelPartidasJugadasValor;
    private Label LabelPartidasJugadasValor => _LabelPartidasJugadasValor ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelPartidasJugadasValor");

    private Label _LabelMejorPuntuacionValor;
    private Label LabelMejorPuntuacionValor => _LabelMejorPuntuacionValor ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelMejorPuntuacionValor");

    private Label _LabelMonedasRecogidasValor;
    private Label LabelMonedasRecogidasValor => _LabelMonedasRecogidasValor ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelMonedasRecogidasValor");

    private Label _LabelMonedasEspecialesRecogidasValor;
    private Label LabelMonedasEspecialesRecogidasValor => _LabelMonedasEspecialesRecogidasValor ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelMonedasEspecialesRecogidasValor");

    private Label _LabelEnemigosDerrotadosValor;
    private Label LabelEnemigosDerrotadosValor => _LabelEnemigosDerrotadosValor ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "LabelEnemigosDerrotadosValor");

    private ButtonPersonalizado _ButtonAtras;
    public ButtonPersonalizado ButtonAtras => _ButtonAtras ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        LabelPartidasJugadasValor.Text = GestorEstadisticas.Globales.PartidasJugadas.ToString();
        LabelMejorPuntuacionValor.Text = GestorEstadisticas.Globales.MejorPuntuacion.ToString();
        LabelMonedasRecogidasValor.Text = GestorEstadisticas.Globales.MonedasRecogidas.ToString();
        LabelMonedasEspecialesRecogidasValor.Text = GestorEstadisticas.Globales.MonedasEspecialesRecogidas.ToString();
        LabelEnemigosDerrotadosValor.Text = GestorEstadisticas.Globales.EnemigosDerrotados.ToString();

        EmitSignal(SignalName.FocoElemento, ButtonAtras);
    }

    public override Control ObtenerPrimerElemento()
    {
        return ButtonAtras;
    }

    public override void _Input(InputEvent @event)
    {
        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            UtilidadesNodos.PulsarBoton(ButtonAtras);
        }
    }
}
