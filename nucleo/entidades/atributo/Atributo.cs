using System;
using System.Collections.Generic;
using System.Linq;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.entidades.atributo;

public class Atributo<T>
{
  public string Nombre { get; internal set; }

  internal bool ValorValido { get; set; } = true;

  public T ValorBase { get; set; }

  private T _cache;

  public T Valor
  {
    get
    {
      if (!ValorValido)
      {
        CalcularValor();
        LoggerJuego.Info("Nuevo valor del atributo " + Nombre + ": " + _cache);
      }

      return _cache;
    }

    set
    {
      InvalidarValor();
      _cache = value;
    }
  }

  private readonly Func<IEnumerable<ModificadorAtributo<T>>> _obtenerModificadores;

  public Atributo(string nombre, T valorBase, Func<IEnumerable<ModificadorAtributo<T>>> obtenerModificadores)
  {
    this.Nombre = nombre;
    this._cache = ValorBase = valorBase;
    this._obtenerModificadores = obtenerModificadores;
  }

  internal void CalcularValor()
  {
    T valorCalculado = ValorBase;

    var modificadoresFiltrados = _obtenerModificadores?.Invoke()?.OrderByDescending(m => m.Prioridad);
    if (modificadoresFiltrados != null)
    {
      // Aplicamos cada modificador en orden.
      foreach (var modificador in modificadoresFiltrados)
      {
        valorCalculado = modificador.AplicarModificador(valorCalculado);
      }
    }

    this._cache = valorCalculado;
    this.ValorValido = true;
  }

  public void InvalidarValor()
  {
    LoggerJuego.Trace("Marcando cómo súcio el atributo " + Nombre);
    this.ValorValido = false;
  }
}