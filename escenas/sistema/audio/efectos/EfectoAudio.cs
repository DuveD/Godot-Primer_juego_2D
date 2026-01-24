using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.sistema.audio.efectos;

public abstract class EfectoAudio
{
    public abstract string Id { get; }

    public abstract string Bus { get; }

    private int? _indiceBus;
    public int? IndiceBus
    {
        get
        {
            if (_indiceBus == null)
                _indiceBus = Global.GestorAudio.ObtenerIndiceBus(Bus);
            return _indiceBus;
        }
    }

    public abstract string Nombre { get; }

    public abstract bool Perenne { get; }

    protected AudioEffect Efecto;


    protected int ObtenerIndiceEfecto()
    {
        if (IndiceBus == null || IndiceBus == -1)
            return -1;

        int count = AudioServer.GetBusEffectCount(IndiceBus.Value);
        for (int i = 0; i < count; i++)
        {
            if (AudioServer.GetBusEffect(IndiceBus.Value, i) == Efecto)
                return i;
        }

        return -1;
    }

    protected void EliminarEfecto()
    {
        if (Efecto != null)
        {
            if (IndiceBus == null || IndiceBus == -1)
                return;

            int effectIdx = ObtenerIndiceEfecto();
            if (effectIdx != -1)
                AudioServer.RemoveBusEffect(IndiceBus.Value, effectIdx);

            Efecto = null;

            LoggerJuego.Trace($"Efecto {this.Nombre} del bus {this.Bus} eliminado.");
        }
    }

    public void Activar()
    {
        if (IndiceBus == null || IndiceBus == -1)
            return;

        if (Efecto == null)
            CrearEfectoInterno();

        int indiceEfecto = ObtenerIndiceEfecto();
        if (indiceEfecto >= 0)
            AudioServer.SetBusEffectEnabled(IndiceBus.Value, indiceEfecto, true);

        LoggerJuego.Trace($"Efecto {this.Nombre} del bus {this.Bus} activado.");
    }

    protected void CrearEfectoInterno()
    {
        if (IndiceBus == null || IndiceBus == -1)
            return;

        if (Efecto != null)
            return;

        Efecto = CrearEfecto();

        if (Efecto != null)
        {
            AudioServer.AddBusEffect(IndiceBus.Value, Efecto);
        }

        LoggerJuego.Trace($"Efecto {this.Nombre} creado.");
    }

    protected abstract AudioEffect CrearEfecto();

    public void Desactivar()
    {
        if (IndiceBus == null || IndiceBus == -1)
            return;

        int indiceEfecto = ObtenerIndiceEfecto();
        if (indiceEfecto >= 0)
            AudioServer.SetBusEffectEnabled(IndiceBus.Value, indiceEfecto, false);

        if (!Perenne)
            EliminarEfecto();

        LoggerJuego.Trace($"Efecto {this.Nombre} desactivado.");
    }

    public virtual bool Activo
    {
        get
        {
            if (IndiceBus == null || IndiceBus == -1)
                return false;

            int idx = ObtenerIndiceEfecto();
            if (idx < 0) return false;
            return AudioServer.IsBusEffectEnabled(IndiceBus.Value, idx);
        }
    }
}
