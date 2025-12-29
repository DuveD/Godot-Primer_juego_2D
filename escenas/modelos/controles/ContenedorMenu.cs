using Godot;
using Primerjuego2D.escenas.modelos.controles;

namespace Primerjuego2D.escenas.modelos;

public abstract partial class ContenedorMenu : CenterContainer
{
    [Signal]
    public delegate void FocoElementoEventHandler(Control control);

    public abstract Control ObtenerPrimerElemento();
}