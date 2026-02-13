using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.menuPrincipal;
using Primerjuego2D.escenas.ui;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class PanelMenuPausa : Control
{
    private ContenedorMenuPausa ContenedorMenuPausa;

    private ContenedorMenuAjustes ContenedorMenuAjustes;

    private ContenedorConfirmacion ContenedorConfirmacionSalir;

    private IEnumerable<ContenedorMenu> Menus;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.ContenedorMenuPausa = GetNode<ContenedorMenuPausa>("ContenedorMenuPausa");
        this.ContenedorMenuAjustes = GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");
        this.ContenedorConfirmacionSalir = GetNode<ContenedorConfirmacion>("ContenedorConfirmacionSalir");
        this.Menus = UtilidadesNodos.ObtenerNodosDeTipo<ContenedorMenu>(this);

        this.VisibilityChanged += OnVisibilityChanged;
    }

    private void ModoNavegacionTecladoChanged(bool modoNavegacionTeclado)
    {
        Global.NavegacionTeclado = modoNavegacionTeclado;
    }

    private void OnVisibilityChanged()
    {
        if (this.Visible)
        {
            Global.NavegacionTeclado = false;
            this.ContenedorMenuPausa.Show(true);
            this.ContenedorMenuAjustes.Hide();
            this.ContenedorConfirmacionSalir.Hide();
        }
        else
        {
            this.ContenedorMenuPausa.Hide();
            this.ContenedorMenuAjustes.Hide();
            this.ContenedorConfirmacionSalir.Hide();
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        // Bloqueamos todos los eventos hacia abajo.
        AcceptEvent();
    }

    public void OnButtonAjustesPressed()
    {
        this.ContenedorMenuPausa.Hide();
        this.ContenedorMenuAjustes.Show(true);
        this.ContenedorConfirmacionSalir.Hide();
    }

    public void OnButtonAjustesAtrasPressed()
    {
        this.ContenedorMenuPausa.Show(false);
        this.ContenedorMenuAjustes.Hide();
        this.ContenedorConfirmacionSalir.Hide();
    }

    public void OnButtonSalirPressed()
    {
        this.ContenedorMenuPausa.Hide();
        this.ContenedorMenuAjustes.Hide();
        this.ContenedorConfirmacionSalir.Show(true);
    }

    public void OnButtonSalirCancelarPressed()
    {
        this.ContenedorMenuPausa.Show(false);
        this.ContenedorMenuAjustes.Hide();
        this.ContenedorConfirmacionSalir.Hide();
    }
}