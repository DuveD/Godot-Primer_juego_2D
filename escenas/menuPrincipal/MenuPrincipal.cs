namespace Primerjuego2D.escenas.menuPrincipal;

using System;
using Godot;
using Primerjuego2D.escenas.batalla;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;


public partial class MenuPrincipal : Control
{
    public const long ID_OPCION_CASTELLANO = 0;
    public const long ID_OPCION_INGLES = 1;

    [Signal]
    public delegate void BotonEmpezarPartidaPulsadoEventHandler();

    ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private MenuButton _MenuButtonLenguaje;
    private MenuButton MenuButtonLenguaje => _MenuButtonLenguaje ??= GetNode<MenuButton>("MenuButtonLenguaje");

    public override void _Ready()
    {
        Logger.Trace("MenuPrincipal Ready.");

        InicializarMenuButtonLenguaje();
    }

    private void InicializarMenuButtonLenguaje()
    {
        PopupMenu popupMenu = this.MenuButtonLenguaje.GetPopup();
        popupMenu.IdPressed += MenuButtonLenguaje_IdPressed;

        Idioma idioma = GestorIdioma.GetIdiomaActual();
        switch (idioma)
        {
            default:
            case Idioma.Castellano:
                MenuButtonLenguaje_IdPressed(ID_OPCION_CASTELLANO);
                break;
            case Idioma.Ingles:
                MenuButtonLenguaje_IdPressed(ID_OPCION_INGLES);
                break;
        }
    }

    private void MenuButtonLenguaje_IdPressed(long id)
    {
        // Obtenemos el PopupMenu del MenuButton
        var popupMenu = this.MenuButtonLenguaje.GetPopup();

        // Checkeamos el ítem del id
        UtilidadesNodos.CheckItemPorId(popupMenu, id);

        switch (id)
        {
            default:
            case ID_OPCION_CASTELLANO:
                GestorIdioma.SetIdiomaCastellano();
                break;
            case ID_OPCION_INGLES:
                GestorIdioma.SetIdiomaIngles();
                break;
        }
    }

    private void OnButtonEmpezarPartidaPressed()
    {
        Logger.Trace("Botón 'ButtonEmpezarPartida' pulsado.");
        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
    }

    private void OnButtonCargarPartidaPressed()
    {
        Logger.Trace("Botón 'ButtonCargarPartida' pulsado.");
    }

    private void OnButtonSalirPressed()
    {
        Logger.Trace("Botón 'ButtonSalir' pulsado.");
        this.GetTree().Quit();
    }
}
