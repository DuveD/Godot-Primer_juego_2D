using System;
using Godot;
using Primerjuego2D.escenas.batalla;
using Primerjuego2D.escenas.menuPrincipal;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades.log;

public partial class PanelMenuPausa : Control
{
    private ContenedorMenuPausa _ContenedorMenuPausa;
    private ContenedorMenuPausa ContenedorMenuPausa => _ContenedorMenuPausa ??= GetNode<ContenedorMenuPausa>("ContenedorMenuPausa");

    private ContenedorMenuAjustes _ContenedorMenuAjustes;
    public ContenedorMenuAjustes ContenedorMenuAjustes => _ContenedorMenuAjustes ??= GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.VisibilityChanged += OnVisibilityChanged;
    }

    private void OnVisibilityChanged()
    {
        if (this.Visible)
        {
            this.ContenedorMenuPausa.Visible = true;
            this.ContenedorMenuAjustes.Visible = false;
        }
        else
        {
            this.ContenedorMenuPausa.Visible = false;
            this.ContenedorMenuAjustes.Visible = false;
        }
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        // Solo respondemos si el menú es visible.
        if (!this.Visible)
            return;

        // Bloqueamos todos los eventos hacia abajo.
        AcceptEvent();
    }

    public void OnButtonAjustesPressed()
    {
        this.ContenedorMenuPausa.Visible = false;
        this.ContenedorMenuAjustes.Visible = true;
    }

    public void OnButtonAjustesAtrasPressed()
    {
        this.ContenedorMenuAjustes.Visible = false;
        this.ContenedorMenuPausa.Visible = true;
    }
}
