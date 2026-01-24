using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesAudio
{
    public static int ObtenerIndiceBus(string busName)
    {
        int busIndex = AudioServer.GetBusIndex(busName);
        if (busIndex == -1)
            LoggerJuego.Error("Bus '" + busName + "' no encontrado.");

        return busIndex;
    }

    public static void AjustarVolumenBus(string busName, float volumenDb)
    {
        int busIndex = ObtenerIndiceBus(busName);
        if (busIndex != -1)
            AudioServer.SetBusVolumeDb(busIndex, volumenDb);
    }
}