using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.ui.controles;
using Primerjuego2D.escenas.ui.logros;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.sistema.logros;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class ContenedorMenuLogros : ContenedorMenu
{
    [Export]
    public PackedScene ContenedorLogroUnicoScene;
    [Export]
    public PackedScene ContenedorLogroContadorScene;

    private ScrollContainer _ScrollContainer;
    public ScrollContainer ScrollContainer => _ScrollContainer ??= UtilidadesNodos.ObtenerNodoPorNombre<ScrollContainer>(this, "ScrollContainer");

    private VBoxContainer _VBoxContainerLogros;
    public VBoxContainer VBoxContainerLogros => _VBoxContainerLogros ??= UtilidadesNodos.ObtenerNodoPorNombre<VBoxContainer>(this, "VBoxContainerLogros");

    private ButtonPersonalizado _ButtonAtras;
    public ButtonPersonalizado ButtonAtras => _ButtonAtras ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

    public override void _Ready()
    {
        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");

        CargarLogros();
    }

    private void CargarLogros()
    {
        IEnumerable<Logro> logros = GestorLogros.ObtenerLogros();
        logros = logros.OrderBy(l => !l.Desbloqueado);

        foreach (Logro logro in logros)
        {
            if (logro is LogroUnico logroUnico)
            {
                ContenedorLogroUnico contenedorLogroUnico = ContenedorLogroUnicoScene.Instantiate<ContenedorLogroUnico>();
                contenedorLogroUnico.Inicializar(logroUnico);
                VBoxContainerLogros.AddChild(contenedorLogroUnico);
            }
            else if (logro is LogroContador logroContador)
            {
                ContenedorLogroContador contenedorLogroContador = ContenedorLogroContadorScene.Instantiate<ContenedorLogroContador>();
                contenedorLogroContador.Inicializar(logroContador);
                VBoxContainerLogros.AddChild(contenedorLogroContador);
            }
        }
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

    public override Control ObtenerPrimerElementoConFoco()
    {
        return ScrollContainer;
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        return [ScrollContainer, ButtonAtras];
    }

}