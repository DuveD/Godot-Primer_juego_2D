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

    private ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private ContenedorMenuPrincipal _ContenedorBotonesPrincipal;
    public ContenedorMenuPrincipal ContenedorMenuPrincipal => _ContenedorBotonesPrincipal ??= GetNode<ContenedorMenuPrincipal>("ContenedorMenuPrincipal");

    private ContenedorMenuAjustes _ContenedorMenuAjustes;
    public ContenedorMenuAjustes ContenedorMenuAjustes => _ContenedorMenuAjustes ??= GetNode<ContenedorMenuAjustes>("ContenedorMenuAjustes");

    private ContenedorMenuLogros _ContenedorMenuLogros;
    public ContenedorMenuLogros ContenedorMenuLogros => _ContenedorMenuLogros ??= GetNode<ContenedorMenuLogros>("ContenedorMenuLogros");

    private ContenedorMenuEstadisticas _ContenedorMenuEstadisticas;
    public ContenedorMenuEstadisticas ContenedorMenuEstadisticas => _ContenedorMenuEstadisticas ??= GetNode<ContenedorMenuEstadisticas>("ContenedorMenuEstadisticas");

    private IEnumerable<ContenedorMenu> Menus => UtilidadesNodos.ObtenerNodosDeTipo<ContenedorMenu>(this);

    public ContenedorMenu UltimoContenedorMostrado;

    private Label _LabelVersion;
    private Label LabelVersion => _LabelVersion ??= GetNode<Label>("LabelVersion");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        LabelVersion.Text = "v" + Ajustes.Version;

        foreach (ContenedorMenu contenedorMenu in Menus)
        {
            contenedorMenu.ModoNavegacionTecladoChanged += ModoNavegacionTecladoChanged;
        }
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
