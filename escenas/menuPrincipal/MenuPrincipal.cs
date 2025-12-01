using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.configuracion;
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

    private ColorRect _Fondo;
    private ColorRect Fondo => _Fondo ??= GetNode<ColorRect>("Fondo");

    private MenuButton _MenuButtonLenguaje;
    private MenuButton MenuButtonLenguaje => _MenuButtonLenguaje ??= GetNode<MenuButton>("MenuButtonLenguaje");

    private ButtonEmpezarPartida _ButtonEmpezarPartida;
    private ButtonEmpezarPartida ButtonEmpezarPartida => _ButtonEmpezarPartida ??= UtilidadesNodos.ObtenerNodoPorNombre<ButtonEmpezarPartida>(this, "ButtonEmpezarPartida");

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        InicializarMenuButtonLenguaje();

        ButtonEmpezarPartida.GrabFocus();
        this.ButtonEmpezarPartida.FocusEntered += () => this.ButtonEmpezarPartida.OnFocusedEntered();
    }

    private void InicializarMenuButtonLenguaje()
    {
        PopupMenu popupMenu = this.MenuButtonLenguaje.GetPopup();
        popupMenu.IdPressed += MenuButtonLenguajeIdPressed;

        Idioma idioma = GestorIdioma.ObtenerIdiomaDeSistema();
        if (idioma.Codigo == Idioma.ES.Codigo)
            MenuButtonLenguajeIdPressed(ID_OPCION_CASTELLANO);
        else if (idioma.Codigo == Idioma.EN.Codigo)
            MenuButtonLenguajeIdPressed(ID_OPCION_INGLES);
        else
            MenuButtonLenguajeIdPressed(ID_OPCION_CASTELLANO);
    }

    private void MenuButtonLenguajeIdPressed(long id)
    {
        LoggerJuego.Trace("Opción de 'MenuButtonLenguaje' pulsado.");

        // Obtenemos el PopupMenu del MenuButton
        var popupMenu = this.MenuButtonLenguaje.GetPopup();

        // Checkeamos el ítem del id
        UtilidadesNodos.CheckItemPorId(popupMenu, id);

        Idioma idioma;
        switch (id)
        {
            default:
            case ID_OPCION_CASTELLANO:
                idioma = Idioma.ES;
                break;
            case ID_OPCION_INGLES:
                idioma = Idioma.EN;
                break;
        }

        GestorIdioma.CambiarIdioma(idioma);
        Ajustes.Idioma = idioma;
    }

    private void OnButtonEmpezarPartidaPressedAnimationEnd()
    {
        LoggerJuego.Trace("Botón 'ButtonEmpezarPartida' pulsado.");

        EmitSignal(SignalName.BotonEmpezarPartidaPulsado);
    }

    private void OnButtonCargarPartidaPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonCargarPartida' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");
    }

    private async void OnButtonSalirPressed()
    {
        LoggerJuego.Trace("Botón 'ButtonSalir' pulsado.");

        Global.GestorAudio.ReproducirSonido("digital_click.mp3");
        await Task.Delay(500);

        this.GetTree().Quit();
    }
}
