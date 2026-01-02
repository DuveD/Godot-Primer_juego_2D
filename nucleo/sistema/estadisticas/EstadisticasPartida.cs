
using System;

namespace Primerjuego2D.nucleo.sistema.estadisticas;

public class EstadisticasPartida
{
    public int PuntuacionFinal { get; private set; } = 0;
    public int MonedasRecogidas { get; private set; } = 0;
    public int MonedasEspecialesRecogidas { get; private set; } = 0;
    public int EnemigosDerrotados { get; private set; } = 0;

    public void RegistrarMoneda(bool especial)
    {
        MonedasRecogidas++;
        if (especial)
            MonedasEspecialesRecogidas++;
    }

    public void RegistrarPuntuacion(int puntos)
    {
        PuntuacionFinal += puntos;
    }

    public void RegistrarEnemigoDerrotado()
    {
        EnemigosDerrotados++;
    }
}
