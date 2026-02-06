

using System;
using System.Globalization;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesFechas
{
  public static string ToString(DateTime? dateTime, string formato)
  {
    if (!dateTime.HasValue || String.IsNullOrWhiteSpace(formato))
      return null;

    return dateTime.Value.ToString(formato, CultureInfo.InvariantCulture);
  }

  public static DateTime? ToDateTime(string dateTimeStr, string formato)
  {
    if (String.IsNullOrWhiteSpace(dateTimeStr) || String.IsNullOrWhiteSpace(formato))
      return null;

    if (DateTime.TryParseExact(dateTimeStr, formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
      return dateTime;

    return null;
  }
}