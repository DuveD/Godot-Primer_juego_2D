using System.IO;
using Godot;
using Primerjuego2D.nucleo.modelos.estadisticas;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.configuracion;

public static class GestorEstadisticas
{
    private const string SECCION_ESTADISTICAS = "estadisticas";

    private static ConfigFile ArchivoEstadisticas { get; } = new ConfigFile();

    public static EstadisticasPartida PartidaActual { get; private set; }
    public static EstadisticasGlobales Globales { get; private set; }

    public static void CargarEstadisticas()
    {
        if (File.Exists(Ajustes.RutaArchivoEstadisticas))
        {
            var err = ArchivoEstadisticas.Load(Ajustes.RutaArchivoEstadisticas);
            if (err != Error.Ok)
            {
                Globales = new EstadisticasGlobales();
            }
            else
            {
                Globales = new EstadisticasGlobales
                {
                    PartidasJugadas = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "partidas_jugadas", 0),
                    MejorPuntuacion = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "mejor_puntuacion", 0),
                    MonedasRecogidas = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "monedas_recogidas", 0),
                    MonedasEspecialesRecogidas = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "monedas_especiales_recogidas", 0),
                    EnemigosDerrotados = (int)ArchivoEstadisticas.GetValue(SECCION_ESTADISTICAS, "enemigos_derrotados", 0)
                };
            }
        }
        else
        {
            Globales = new EstadisticasGlobales();
        }
    }

    public static void InicializarPartida()
    {
        if (PartidaActual != null)
            LoggerJuego.Info("Ya existen unas estadísticas de una partida vigente. Se sobreescribe.");

        PartidaActual = new EstadisticasPartida();
    }

    public static void FinalizarPartida()
    {
        ActualizarGlobales();
        GuardarGlobales();
        PartidaActual = null;
    }

    private static void ActualizarGlobales()
    {
        Globales.PartidasJugadas++;
        Globales.MonedasRecogidas += PartidaActual.MonedasRecogidas;
        Globales.MonedasEspecialesRecogidas += PartidaActual.MonedasEspecialesRecogidas;
        Globales.EnemigosDerrotados += PartidaActual.EnemigosDerrotados;

        if (PartidaActual.PuntuacionFinal > Globales.MejorPuntuacion)
            Globales.MejorPuntuacion = PartidaActual.PuntuacionFinal;
    }

    private static void GuardarGlobales()
    {
        ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "partidas_jugadas", Globales.PartidasJugadas);
        ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "mejor_puntuacion", Globales.MejorPuntuacion);
        ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "monedas_recogidas", Globales.MonedasRecogidas);
        ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "monedas_especiales_recogidas", Globales.MonedasEspecialesRecogidas);
        ArchivoEstadisticas.SetValue(SECCION_ESTADISTICAS, "enemigos_derrotados", Globales.EnemigosDerrotados);

        if (!Directory.Exists(Ajustes.RutaJuego))
            Directory.CreateDirectory(Ajustes.RutaJuego);

        var err = ArchivoEstadisticas.Save(Ajustes.RutaArchivoEstadisticas);
        if (err != Error.Ok)
            LoggerJuego.Error($"No se ha podido guardar el archivo de estadísticas: {err}");
        else
            LoggerJuego.Trace("Estadísticas guardadas.");
    }
}