
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

    public int PorcentajeProgreso =>
        Objetivo == 0 ? 0 : (int)((float)Progreso / Objetivo * 100);

    public override bool ProcesarEvento(string evento, object datos)
    {
        if (Desbloqueado)
            return false;
        if (evento != Evento)
            return false;

        Progreso++;

        bool desbloqueado = false;
        if (Progreso >= Objetivo)
        {
            Desbloquear();
            desbloqueado = true;
        }

        return desbloqueado;
    }
}
