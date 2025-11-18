namespace Primerjuego2D.escenas;

using System;
using Godot;
using Primerjuego2D.escenas.batalla;
using Primerjuego2D.escenas.menuPrincipal;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

public partial class Juego : Control
{
    [Export]
    public Control ContenedorEscena { get; set; }

    public override void _Ready()
    {
        // Ajustamos el tamaño del Control al tamaño de la pantalla.
        this.Size = GetViewportRect().Size;

        CargarMenuPrincipal();
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
