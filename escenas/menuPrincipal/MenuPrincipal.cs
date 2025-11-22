using Godot;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.menuPrincipal;

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
        Logger.Trace(this.Name + " Ready.");

        InicializarMenuButtonLenguaje();
    }

    private void InicializarMenuButtonLenguaje()
    {
        PopupMenu popupMenu = this.MenuButtonLenguaje.GetPopup();
        popupMenu.IdPressed += MenuButtonLenguajeIdPressed;

        Idioma idioma = GestorIdioma.GetIdiomaActual();
        switch (idioma)
        {
            default:
            case Idioma.Castellano:
                MenuButtonLenguajeIdPressed(ID_OPCION_CASTELLANO);
                break;
            case Idioma.Ingles:
                MenuButtonLenguajeIdPressed(ID_OPCION_INGLES);
                break;
        }
    }

    private void MenuButtonLenguajeIdPressed(long id)
    {
        Logger.Trace("Opción de 'MenuButtonLenguaje' pulsado.");

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
