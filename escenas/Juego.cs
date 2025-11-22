namespace Primerjuego2D.escenas;

using System;
using Godot;
using Primerjuego2D.escenas.batalla;
using Primerjuego2D.escenas.menuPrincipal;
using Primerjuego2D.escenas.sistema;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

public partial class Juego : Control
{
    [Export]
    public Control ContenedorEscena { get; set; }

    public static GestorColor GestorColor { get; private set; }
    [Export]
    public GestorColor _GestorColorNodo { get; set; } // Nodo de la escena

    public static miscelaneo.camara.TemblorCamara Camara { get; private set; }
    [Export]
    public miscelaneo.camara.TemblorCamara _CamaraNodo { get; set; }    // Nodo de la escena

    public override void _Ready()
    {
        GestorColor ??= _GestorColorNodo;
        Camara ??= _CamaraNodo;

        AjustaViewPortYCamara();

        CargarMenuPrincipal();
    }

    private void AjustaViewPortYCamara()
    {
        // Ajustamos el tamaño del juego al tamaño de la pantalla.
        this.Size = GetViewportRect().Size;

        // Ajustamos el tamaño de la cámara al tamaño del juego.
        Juego.Camara.AjustarCamara(this.Size);
        GetViewport().SizeChanged += () => Juego.Camara.AjustarCamara(this.Size); ;
    }


    public void CargarMenuPrincipal()
    {
        Logger.Trace("Cargando menú principal.");

        string rutaMenuprincipal = UtilidadesNodos.ObtenerRutaEscena<MenuPrincipal>();
        MenuPrincipal menuPrincipal = (MenuPrincipal)CambiarPantalla(rutaMenuprincipal);

        menuPrincipal.BotonEmpezarPartidaPulsado += CargarBatalla;
    }

    public void CargarBatalla()
    {
        Logger.Trace("Cargando batalla.");

        string rutaBatalla = UtilidadesNodos.ObtenerRutaEscena<Batalla>();
        Batalla batalla = (Batalla)CambiarPantalla(rutaBatalla);

        batalla.GameOverFinalizado += CargarMenuPrincipal;
    }

    public Node CambiarPantalla(string rutaEscena)
    {
        // Cargamos la escena desde la ruta proporcionada.
        PackedScene pantalla = GD.Load(rutaEscena) as PackedScene;

        if (pantalla == null)
        {
            Logger.Error($"No se pudo cargar la escena en la ruta: {rutaEscena}");
            return null;
        }

        // Instanciamos la escena cargada.
        Node instanciaEscena = pantalla.Instantiate();
        if (instanciaEscena == null)
        {
            Logger.Error($"No se pudo instanciar la escena desde la ruta: {rutaEscena}");
            return null;
        }

        // Limpiamos el contenedor de escenas actual.
        UtilidadesNodos.BorrarHijos(this.ContenedorEscena);

        // Añadimos la nueva escena al contenedor.
        this.ContenedorEscena.AddChild(instanciaEscena);

        return instanciaEscena;
    }

}
