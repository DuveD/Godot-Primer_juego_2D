using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.sistema.audio.efectos;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.sistema.audio;

public partial class GestorEfectosAudio : Node
{
    private readonly Dictionary<string, EfectoAudio> _efectos = new Dictionary<string, EfectoAudio>
    {
        {EfectoBajoElAgua.ID, new EfectoBajoElAgua()}
    };

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");
    }

    public void Activar(string id)
    {
        if (_efectos.TryGetValue(id, out var efecto))
            efecto.Activar();
    }

    public void Desactivar(string id)
    {
        if (_efectos.TryGetValue(id, out var efecto))
            efecto.Desactivar();
    }

    public void DesactivarTodos()
    {
        foreach (var efecto in _efectos.Values)
            efecto.Desactivar();
    }
}
