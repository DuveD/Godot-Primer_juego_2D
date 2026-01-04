using Godot;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;

namespace Primerjuego2D.escenas.ui.logros;

public partial class ContenedorLogroContador : PanelContainer
{
    private Label _LabelNombre;
    public Label LabelNombre => _LabelNombre ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "Nombre");

    private Label _LabelDescripcion;
    public Label LabelDescripcion => _LabelDescripcion ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "Descripcion");

    private ProgressBar _ProgressBar;
    public ProgressBar ProgressBar => _ProgressBar ??= UtilidadesNodos.ObtenerNodoPorNombre<ProgressBar>(this, "ProgressBar");

    public void Inicializar(LogroContador logro)
    {
        LabelNombre.Text = logro.Nombre;
        LabelDescripcion.Text = logro.Descripcion;
        ProgressBar.MinValue = 0;
        ProgressBar.MaxValue = 100;
        ProgressBar.Value = logro.PorcentajeProgreso;

        // Opcional: visual según estado
        Modulate = logro.Desbloqueado
            ? Colors.White
            : new Color(1, 1, 1, 0.4f);
    }
}