namespace Primerjuego2D.nucleo.sistema.logros;

public class LogroUnico(
    string id,
    string nombre,
    string descripcion,
    string evento
    ) : Logro(id, nombre, descripcion, evento)
{
    public override bool ProcesarEvento(string evento, object datos)
    {
        bool desbloqueado = false;

        if (Desbloqueado)
            return desbloqueado;

        if (evento == Evento)
        {
            Desbloquear();
            desbloqueado = true;
        }

        return desbloqueado;
    }
}
