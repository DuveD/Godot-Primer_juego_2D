using System;
using System.Collections.Generic;
using Primerjuego2D.nucleo.sistema.estadisticas;
using Primerjuego2D.nucleo.sistema.logros;

namespace Primerjuego2D.nucleo.sistema.perfil;

public class Perfil(string id, string nombre, DateTime fechaCreacion, DateTime? fechaUltimaPartida)
{
    public string Id { get; private set; } = id;

    public string Nombre { get; private set; } = nombre;

    public DateTime FechaCreacion { get; set; } = fechaCreacion;

    public DateTime? FechaUltimaPartida { get; set; } = fechaUltimaPartida;

    public EstadisticasGlobales EstadisticasGlobales { get; private set; } = new();

    public List<Logro> Logros { get; private set; }
}