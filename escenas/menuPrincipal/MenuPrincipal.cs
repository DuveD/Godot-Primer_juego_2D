using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.ui.menu;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

public partial class MenuPrincipal : Control
{
    public bool ModoNavegacionTeclado = false;

    #region Nodos escena
    public ContenedorMenuPrincipal ContenedorMenuPrincipal;
    public ContenedorMenuAjustes ContenedorMenuAjustes;
    public ContenedorMenuLogros ContenedorMenuLogros;
    public ContenedorMenuEstadisticas ContenedorMenuEstadisticas;
    public Label LabelVersion;
    #endregion

    public ContenedorMenu UltimoContenedorMostrado;

    public IEnumerable<ContenedorMenu> Menus;

    public override void _Ready()
    {
        CargarNodos();
        LoggerJuego.Trace(this.Name + " Ready.");

        LabelVersion.Text = "v" + Ajustes.Version;

        foreach (ContenedorMenu contenedorMenu in Menus)
        {
            contenedorMenu.ModoNavegacionTecladoChanged += ModoNavegacionTecladoChanged;
        }
    }

    private void CargarNodos()
    {
        ContenedorMenuPrincipal = GetNode<ContenedorMenuPrincipal>("ContenedorMenuPrincipal");
        ContenedorMenuAjustes = GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");
        ContenedorMenuLogros = GetNode<ContenedorMenuLogros>("ContenedorMenuLogros");
        ContenedorMenuEstadisticas = GetNode<ContenedorMenuEstadisticas>("ContenedorMenuEstadisticas");
        Menus = UtilidadesNodos.ObtenerNodosDeTipo<ContenedorMenu>(this);
        LabelVersion = GetNode<Label>("LabelVersion");
    }

    private void ModoNavegacionTecladoChanged(bool modoNavegacionTeclado)
    {
        this.ModoNavegacionTeclado = modoNavegacionTeclado;
    }

    public void MostrarMenuPrincipal()
    {
        MostrarMenu(this.ContenedorMenuPrincipal);
    }

    private void MostrarMenuAjustes()
    {
        MostrarMenu(this.ContenedorMenuAjustes);
    }

    private void MostrarMenuLogros()
    {
        MostrarMenu(this.ContenedorMenuLogros);
    }

    private void MostrarMenuEstadisticas()
    {
        MostrarMenu(this.ContenedorMenuEstadisticas);
    }

    private void MostrarMenu(ContenedorMenu contenedorMenu)
    {
        foreach (var menu in Menus)
            menu.Visible = false;

        bool seleccionarPrimerElemento = !(contenedorMenu is ContenedorMenuPrincipal);
        contenedorMenu.Show(this.ModoNavegacionTeclado, seleccionarPrimerElemento);
    }
}
