using Godot;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;

namespace Primerjuego2D.escenas.ui.logros;

public partial class ContenedorLogroUnico : PanelContainer
{
    private Label _LabelNombre;
    public Label LabelNombre => _LabelNombre ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "Nombre");

    private Label _LabelDescripcion;
    public Label LabelDescripcion => _LabelDescripcion ??= UtilidadesNodos.ObtenerNodoPorNombre<Label>(this, "Descripcion");

    public void Inicializar(LogroUnico logro)
    {
        LabelNombre.Text = logro.Nombre;
        LabelDescripcion.Text = logro.Descripcion;

        // Opcional: visual según estado
        Modulate = logro.Desbloqueado
            ? Colors.White
            : new Color(1, 1, 1, 0.4f);
    }
}