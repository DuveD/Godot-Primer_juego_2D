
using System;
using System.IO;
using Godot;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.sistema.perfil;

public static class GestorPerfiles
{
  private const string SECCION_DATOS_PERFIL = "datos_perfil";

  public static string GenerarIdPerfil()
  {
    return "perfil_" + Guid.NewGuid().ToString("N")[..8];
  }

  public static Perfil CargarPerfil(string idPerfil)
  {
    if (string.IsNullOrWhiteSpace(idPerfil))
      return null;

    if (idPerfil.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
    {
      LoggerJuego.Error($"El id del perfil no es válido: '{idPerfil}'.");
      return null;
    }

    string rutaArchivoPerfil = Path.Combine(Ajustes.RutaPerfiles, idPerfil + ".save");
    if (!File.Exists(rutaArchivoPerfil))
    {
      LoggerJuego.Warn($"No se ha encontrado el archivo del perfil '{idPerfil}'.");
      return null;
    }
    else
    {
      ConfigFile archivoPerfil = new();
      var error = archivoPerfil.Load(rutaArchivoPerfil);
      if (error != Error.Ok)
      {
        LoggerJuego.Error($"No se ha podido cargar el perfil '{idPerfil}': {error}");
        return null;
      }

      // Nombre perfil
      string nombre = (string)archivoPerfil.GetValue(SECCION_DATOS_PERFIL, "nombre", "");

      // Fecha creación
      long fechaCreacionTicks = (long)archivoPerfil.GetValue(SECCION_DATOS_PERFIL, "fecha_creacion", DateTime.Now.Ticks);
      DateTime fechaCreacion = new(fechaCreacionTicks);

      // Fecha última partida
      long fechaUltimaPartidaTicks = (long)archivoPerfil.GetValue(SECCION_DATOS_PERFIL, "fecha_ultima_partida", -1);
      DateTime? fechaUltimaPartida = (fechaUltimaPartidaTicks > 0) ? new(fechaUltimaPartidaTicks) : null;

      Perfil perfil = new(idPerfil, nombre, fechaCreacion, fechaUltimaPartida);
      LoggerJuego.Info($"Perfil '{perfil.Nombre}' ({perfil.Id}) cargado correctamente.");

      return perfil;
    }
  }

  public static void GuardarPerfil(Perfil perfil)
  {
    if (perfil == null)
      return;

    if (!Directory.Exists(Ajustes.RutaPerfiles))
      Directory.CreateDirectory(Ajustes.RutaPerfiles);

    ConfigFile archivoPerfil = new();

    // Nombre perfil
    archivoPerfil.SetValue(SECCION_DATOS_PERFIL, "nombre", perfil.Nombre);

    // Fecha creación
    archivoPerfil.SetValue(SECCION_DATOS_PERFIL, "fecha_creacion", perfil.FechaCreacion.Ticks);

    // Fecha última partida
    long fechaUltimaTicks = perfil.FechaUltimaPartida?.Ticks ?? -1L;
    archivoPerfil.SetValue(SECCION_DATOS_PERFIL, "fecha_ultima_partida", fechaUltimaTicks);

    string rutaArchivoPerfil = Path.Combine(Ajustes.RutaPerfiles, perfil.Id);
    string rutaArchivoPerfilTemporal = rutaArchivoPerfil + ".tmp";
    string rutaArchivoPerfilSave = rutaArchivoPerfil + ".save";

    var error = archivoPerfil.Save(rutaArchivoPerfilTemporal);
    if (error != Error.Ok)
    {
      LoggerJuego.Error($"No se ha podido guardar el perfil '{perfil.Nombre}': {error}");
    }
    else
    {
      File.Move(rutaArchivoPerfilTemporal, rutaArchivoPerfilSave, true);
      LoggerJuego.Info($"Perfil '{perfil.Nombre}' ({perfil.Id}) guardado correctamente."
);
    }
  }

  public static bool EliminarPerfil(string idPerfil)
  {
    if (string.IsNullOrWhiteSpace(idPerfil))
      return false;

    if (idPerfil.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
    {
      LoggerJuego.Error($"Id de perfil inválido: '{idPerfil}'.");
      return false;
    }

    string rutaArchivoPerfil = Path.Combine(Ajustes.RutaPerfiles, idPerfil + ".save");
    if (!File.Exists(rutaArchivoPerfil))
    {
      LoggerJuego.Warn($"No se puede eliminar el perfil '{idPerfil}': archivo no encontrado.");
      return false;
    }

    try
    {
      File.Delete(rutaArchivoPerfil);
      LoggerJuego.Info($"Perfil '{idPerfil}' eliminado correctamente.");
      return true;
    }
    catch (Exception ex)
    {
      LoggerJuego.Error($"Error al eliminar el perfil '{idPerfil}': {ex.Message}");
      return false;
    }
  }
}