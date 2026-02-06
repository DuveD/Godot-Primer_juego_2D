using System;
using System.Collections.Generic;
using Primerjuego2D.nucleo.sistema.logros;

public class Perfil
{
  public string Nombre { get; private set; }

  public List<Logro> Logros { get; private set; }

  public DateTime? FechaUltimaPartida { get; set; }

  public int PartidasJugadas { get; set; }

  public int Monedasrecogidas { get; set; }

  public Perfil(string nombre)
  {
    this.Nombre = nombre;
    this.PartidasJugadas = 0;
    this.Monedasrecogidas = 0;
  }
}