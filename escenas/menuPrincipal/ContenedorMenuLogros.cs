using System;
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

    private ContenedorLogro _PrimerLogroLista;
    private ContenedorLogro _UltimoLogroLista;

    private List<ContenedorLogro> _ListaContenedoresLogros = [];

    private ScrollContainer _ScrollContainer;
    public ScrollContainer ScrollContainer => _ScrollContainer ??= UtilidadesNodos.ObtenerNodoPorNombre<ScrollContainer>(this, "ScrollContainer");

    private VBoxContainer _VBoxContainerLogros;
    public VBoxContainer VBoxContainerLogros => _VBoxContainerLogros ??= UtilidadesNodos.ObtenerNodoPorNombre<VBoxContainer>(this, "VBoxContainerLogros");

    private ButtonPersonalizado _ButtonAtras;
    public ButtonPersonalizado ButtonAtras => _ButtonAtras ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonPersonalizado>(this, "ButtonAtras");

    public override void _Ready()
    {
        CargarLogros();

        base._Ready();

        LoggerJuego.Trace(this.Name + " Ready.");
    }

    private void CargarLogros()
    {
        IEnumerable<Logro> logros = GestorLogros.ObtenerLogros();
        logros = logros.OrderBy(l => !l.Desbloqueado);

        ContenedorLogro contenedorLogroAnterior = null;
        foreach (Logro logro in logros)
        {
            ContenedorLogro contenedorLogro = logro switch
            {
                LogroUnico => ContenedorLogroUnicoScene.Instantiate<ContenedorLogroUnico>(),
                LogroContador => ContenedorLogroContadorScene.Instantiate<ContenedorLogroContador>(),
                _ => null
            };

            if (contenedorLogro == null)
                continue;

            contenedorLogro.Inicializar(logro);
            VBoxContainerLogros.AddChild(contenedorLogro);
            _ListaContenedoresLogros.Add(contenedorLogro);

            if (this._PrimerLogroLista == null)
            {
                this._PrimerLogroLista = contenedorLogro;
                CambiarFocoButtonAtrasAUltimoContenedorLogroFocused(contenedorLogro);
            }

            contenedorLogro.FocusEntered += () => CambiarFocoButtonAtrasAUltimoContenedorLogroFocused(contenedorLogro);

            EnlazarFoco(contenedorLogro, contenedorLogroAnterior);
            contenedorLogroAnterior = contenedorLogro;
        }

        if (contenedorLogroAnterior != null)
        {
            this._UltimoLogroLista = contenedorLogroAnterior;
            this._UltimoLogroLista.FocusNeighborBottom = this._UltimoLogroLista.GetPathTo(this.ButtonAtras);
            this.ButtonAtras.FocusNeighborTop = this.ButtonAtras.GetPathTo(this._UltimoLogroLista);
        }
    }

    private void EnlazarFoco(ContenedorLogro actual, ContenedorLogro anterior)
    {
        if (anterior == null)
            return;

        actual.FocusNeighborTop = actual.GetPathTo(anterior);
        actual.FocusNeighborLeft = actual.GetPathTo(ButtonAtras);
        anterior.FocusNeighborBottom = anterior.GetPathTo(actual);
    }


    private void CambiarFocoButtonAtrasAUltimoContenedorLogroFocused(ContenedorLogro contenedorLogro)
    {
        this.ButtonAtras.FocusNeighborRight = this.ButtonAtras.GetPathTo(contenedorLogro);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        if (@event.IsActionPressed(ConstantesAcciones.ESCAPE))
        {
            UtilidadesNodos.PulsarBoton(ButtonAtras);
            AcceptEvent();
        }
    }

    public override Control ObtenerPrimerElementoConFoco()
    {
        return this._PrimerLogroLista != null ? _PrimerLogroLista : ButtonAtras;
    }

    public override List<Control> ObtenerElementosConFoco()
    {
        List<Control> elementos = [];

        elementos.AddRange(_ListaContenedoresLogros);
        elementos.Add(ButtonAtras);

        return elementos;
    }
}