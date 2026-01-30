using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Primerjuego2D.nucleo.sistema.configuracion;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.nucleo.sistema.logros;

public static class GestorLogros
{
    private static readonly object _lock = new();

    private const string SECCION_LOGROS = "logros";

    private static ConfigFile ArchivoLogros { get; } = new ConfigFile();

    public static string EVENTO_LOGRO_PRIMERA_PARTIDA = "primera_partida";

    public static string EVENTO_LOGRO_ENEMIGO_DERROTADO = "enemigo_derrotado";

    public static string EVENTO_LOGRO_MONEDA_OBTENIDA = "moneda_obtenida";

    private static Dictionary<string, List<Logro>> _logros = [];

    public static IEnumerable<Logro> ObtenerLogros()
    {
        lock (_lock)
        {
            return _logros.Values.SelectMany(l => l);
        }
    }

    public static void CargarLogros()
    {
        CargarArchivoLogros();

        LoggerJuego.Info("Logros cargados.");
    }

    private static void CargarArchivoLogros()
    {
        RegistrarLogros();

        if (File.Exists(Ajustes.RutaArchivoLogros))
        {
            var err = ArchivoLogros.Load(Ajustes.RutaArchivoLogros);
            if (err != Error.Ok)
            {
                LoggerJuego.Error($"No se pudo cargar el archivo de logros: {err}");
                return;
            }

            foreach (Logro logro in ObtenerLogros())
            {
                Godot.Collections.Dictionary datosLogro = (Godot.Collections.Dictionary)ArchivoLogros.GetValue(SECCION_LOGROS, logro.Id, new Godot.Collections.Dictionary());
                if (datosLogro.Count == 0)
                    continue;

                logro.Desbloqueado = (bool)datosLogro.GetValueOrDefault("desbloqueado", false);
                if (logro is LogroContador logroContador)
                {
                    logroContador.Progreso = (int)datosLogro.GetValueOrDefault("progreso", 0);
                }
            }

            LoggerJuego.Trace("Archivo de logros cargado correctamente.");
        }
    }

    private static void RegistrarLogros()
    {
        Registrar(new LogroUnico(
            "primeraPartida",
            "Logro.primeraPartida.nombre",
            "Logro.primeraPartida.descripcion",
            EVENTO_LOGRO_PRIMERA_PARTIDA
        ));

        RegistrarLogrosEnemigosDerrotados();

        RegistrarLogrosMonedasObtenidas();
    }

    private static void RegistrarLogrosEnemigosDerrotados()
    {
        Registrar(new LogroContador(
            "enemigosDerrotados10",
            "Logro.enemigosDerrotados10.nombre",
            "Logro.enemigosDerrotados10.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            10
        ));

        Registrar(new LogroContador(
            "enemigosDerrotados25",
            "Logro.enemigosDerrotados25.nombre",
            "Logro.enemigosDerrotados25.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            25
        ));

        Registrar(new LogroContador(
            "enemigosDerrotados50",
            "Logro.enemigosDerrotados50.nombre",
            "Logro.enemigosDerrotados50.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            50
        ));

        Registrar(new LogroContador(
            "enemigosDerrotados100",
            "Logro.enemigosDerrotados100.nombre",
            "Logro.enemigosDerrotados100.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            100
        ));

        Registrar(new LogroContador(
            "enemigosDerrotados500",
            "Logro.enemigosDerrotados500.nombre",
            "Logro.enemigosDerrotados500.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            500
        ));

        Registrar(new LogroContador(
            "enemigosDerrotados1000",
            "Logro.enemigosDerrotados1000.nombre",
            "Logro.enemigosDerrotados1000.descripcion",
            EVENTO_LOGRO_ENEMIGO_DERROTADO,
            1000
        ));
    }

    private static void RegistrarLogrosMonedasObtenidas()
    {
        Registrar(new LogroContador(
            "monedasObtenidas10",
            "Logro.monedasObtenidas10.nombre",
            "Logro.monedasObtenidas10.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            10
        ));

        Registrar(new LogroContador(
            "monedasObtenidas25",
            "Logro.monedasObtenidas25.nombre",
            "Logro.monedasObtenidas25.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            25
        ));

        Registrar(new LogroContador(
            "monedasObtenidas50",
            "Logro.monedasObtenidas50.nombre",
            "Logro.monedasObtenidas50.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            50
        ));

        Registrar(new LogroContador(
            "monedasObtenidas100",
            "Logro.monedasObtenidas100.nombre",
            "Logro.monedasObtenidas100.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            100
        ));

        Registrar(new LogroContador(
            "monedasObtenidas150",
            "Logro.monedasObtenidas150.nombre",
            "Logro.monedasObtenidas150.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            150
        ));

        Registrar(new LogroContador(
            "monedasObtenidas200",
            "Logro.monedasObtenidas200.nombre",
            "Logro.monedasObtenidas200.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            200
        ));

        Registrar(new LogroContador(
            "monedasObtenidas300",
            "Logro.monedasObtenidas300.nombre",
            "Logro.monedasObtenidas300.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            300
        ));

        Registrar(new LogroContador(
            "monedasObtenidas400",
            "Logro.monedasObtenidas400.nombre",
            "Logro.monedasObtenidas400.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            400
        ));

        Registrar(new LogroContador(
            "monedasObtenidas500",
            "Logro.monedasObtenidas500.nombre",
            "Logro.monedasObtenidas500.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            500
        ));

        Registrar(new LogroContador(
            "monedasObtenidas600",
            "Logro.monedasObtenidas600.nombre",
            "Logro.monedasObtenidas600.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            600
        ));

        Registrar(new LogroContador(
            "monedasObtenidas700",
            "Logro.monedasObtenidas700.nombre",
            "Logro.monedasObtenidas700.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            700
        ));

        Registrar(new LogroContador(
            "monedasObtenidas800",
            "Logro.monedasObtenidas800.nombre",
            "Logro.monedasObtenidas800.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            800
        ));

        Registrar(new LogroContador(
            "monedasObtenidas900",
            "Logro.monedasObtenidas900.nombre",
            "Logro.monedasObtenidas900.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            900
        ));

        Registrar(new LogroContador(
            "monedasObtenidas1000",
            "Logro.monedasObtenidas1000.nombre",
            "Logro.monedasObtenidas1000.descripcion",
            EVENTO_LOGRO_MONEDA_OBTENIDA,
            1000
        ));
    }

    private static void Registrar(Logro logro)
    {
        if (!_logros.TryGetValue(logro.Evento, out var lista))
        {
            lista = [];
            _logros[logro.Evento] = lista;
        }

        lista.Add(logro);
    }

    public static Task<List<Logro>> EmitirEventoAsync(string evento, object datos = null)
    {
        return Task.Run(() => EmitirEvento(evento, datos));
    }

    public static List<Logro> EmitirEvento(string evento, object datos = null)
    {
        lock (_lock)
        {
            List<Logro> logrosDesbloqueados = [];

            if (!_logros.TryGetValue(evento, out var logrosEvento))
                return logrosDesbloqueados;

            if (logrosEvento != null && logrosEvento.Count > 0)
            {
                foreach (var logro in logrosEvento)
                {
                    bool desbloqueado = logro.ProcesarEvento(evento, datos);

                    if (desbloqueado)
                        logrosDesbloqueados.Add(logro);

                    GuardarLogro(logro);
                }

                GuardarLogros();
            }

            return logrosDesbloqueados;
        }
    }

    private static void GuardarLogro(Logro logro, bool guardar = false)
    {
        lock (_lock)
        {
            Godot.Collections.Dictionary datosLogro = [];
            datosLogro.Add("desbloqueado", logro.Desbloqueado);
            if (logro is LogroContador logroContador)
            {
                datosLogro.Add("progreso", logroContador.Progreso);
            }

            ArchivoLogros.SetValue(SECCION_LOGROS, logro.Id, datosLogro);

            if (!Directory.Exists(Ajustes.RutaJuego))
                Directory.CreateDirectory(Ajustes.RutaJuego);

            if (guardar)
                GuardarLogros();
        }
    }

    private static void GuardarLogros()
    {
        lock (_lock)
        {
            var err = ArchivoLogros.Save(Ajustes.RutaArchivoLogros);
            if (err != Error.Ok)
                LoggerJuego.Error($"No se ha podido guardar el archivo de logros: {err}");
            else
                LoggerJuego.Trace("Logros guardados.");
        }
    }
}
