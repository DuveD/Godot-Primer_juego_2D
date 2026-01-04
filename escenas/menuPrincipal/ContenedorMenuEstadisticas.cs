using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.estadisticas;
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
        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");

        LabelPartidasJugadasValor.Text = GestorEstadisticas.Globales.PartidasJugadas.ToString();
        LabelMejorPuntuacionValor.Text = GestorEstadisticas.Globales.MejorPuntuacion.ToString();
        LabelMonedasRecogidasValor.Text = GestorEstadisticas.Globales.MonedasRecogidas.ToString();
        LabelMonedasEspecialesRecogidasValor.Text = GestorEstadisticas.Globales.MonedasEspecialesRecogidas.ToString();
        LabelEnemigosDerrotadosValor.Text = GestorEstadisticas.Globales.EnemigosDerrotados.ToString();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            UtilidadesNodos.PulsarBoton(ButtonAtras);
        }
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return ButtonAtras;
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [ButtonAtras];
    }

}
