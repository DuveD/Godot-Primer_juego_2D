
namespace Primerjuego2D.nucleo.sistema.logros;

public class LogroContador(
    string id,
    string nombre,
    string descripcion,
    string evento,
    int objetivo
    ) : Logro(id, nombre, descripcion, evento)
{
    public int Progreso { get; set; }
    public int Objetivo { get; } = objetivo;

    public int PorcentajeProgero => Progreso / Objetivo * 100;

    public override bool ProcesarEvento(string evento, object datos)
    {
        bool desbloqueado = false;

        if (Desbloqueado)
            return desbloqueado;
        if (evento != Evento)
            return desbloqueado;

        Progreso++;

        if (Progreso >= Objetivo)
        {
            Desbloquear();
            desbloqueado = true;
        }

        return desbloqueado;
    }
}
