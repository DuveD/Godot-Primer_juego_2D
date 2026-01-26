using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.batalla;
using Primerjuego2D.escenas.menuPrincipal;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

public partial class PanelMenuPausa : Control
{
    public bool ModoNavegacionTeclado = false;

    private ContenedorMenuPausa ContenedorMenuPausa;

    private ContenedorMenuAjustes ContenedorMenuAjustes;

    private IEnumerable<ContenedorMenu> Menus;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.ContenedorMenuPausa = GetNode<ContenedorMenuPausa>("ContenedorMenuPausa");
        this.ContenedorMenuAjustes = GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");
        this.Menus = UtilidadesNodos.ObtenerNodosDeTipo<ContenedorMenu>(this);

        this.VisibilityChanged += OnVisibilityChanged;

        foreach (ContenedorMenu contenedorMenu in Menus)
        {
            contenedorMenu.ModoNavegacionTecladoChanged += ModoNavegacionTecladoChanged;
        }
    }

    private void ModoNavegacionTecladoChanged(bool modoNavegacionTeclado)
    {
        this.ModoNavegacionTeclado = modoNavegacionTeclado;
    }

    private void OnVisibilityChanged()
    {
        if (this.Visible)
        {
            this.ModoNavegacionTeclado = false;
            this.ContenedorMenuPausa.Show(this.ModoNavegacionTeclado, true);
            this.ContenedorMenuAjustes.Hide();
        }
        else
        {
            this.ContenedorMenuPausa.Hide();
            this.ContenedorMenuAjustes.Hide();
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
        this.ContenedorMenuPausa.Hide();
        this.ContenedorMenuAjustes.Show(this.ModoNavegacionTeclado, true);
    }

    public void OnButtonAjustesAtrasPressed()
    {
        this.ContenedorMenuAjustes.Hide();
        this.ContenedorMenuPausa.Show(this.ModoNavegacionTeclado, false);
    }
}
