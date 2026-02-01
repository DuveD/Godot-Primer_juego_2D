using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.miscelaneo.animaciones;
using Primerjuego2D.escenas.objetos.modelos;
using Primerjuego2D.escenas.objetos.moneda;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.objetos.powerup;

public partial class ImanMonedas : PowerUp
{
    [Export]
    private float RadioIman = 1000f;

    [Export]
    private float FuerzaAtraccion = 6f;

    private Area2D _areaIman;
    private CollisionShape2D _collisionShape;
    private readonly HashSet<Moneda> _monedasEnRango = [];

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        base._Ready();
    }

    public override void AplicarEfectoPowerUp(Jugador jugador)
    {
        CrearAreaIman(jugador);
        jugador.MuerteJugador += () => LimpiarAreaIman();
    }

    public override void EfectoPowerUpExpirado(Jugador jugador)
    {
        LimpiarAreaIman();
    }

    public override void ProcessPowerUp(double delta, Jugador jugador)
    {
        foreach (var moneda in _monedasEnRango)
        {
            if (!IsInstanceValid(moneda))
                continue;

            Vector2 direccion = jugador.GlobalPosition - moneda.GlobalPosition;
            moneda.GlobalPosition += direccion * (float)(delta * FuerzaAtraccion);
        }
    }

    private void CrearAreaIman(Jugador jugador)
    {
        _areaIman = new Area2D { Name = "AreaImanMonedas" };
        _areaIman.SetCollisionMaskValue(ConstantesCapasColisiones2D.INTERACTUABLES, true);

        _collisionShape = new CollisionShape2D
        {
            Shape = new CircleShape2D { Radius = RadioIman }
        };

        _areaIman.AddChild(_collisionShape);

        _areaIman.AreaEntered += AreaImanOnAreaEntered;
        _areaIman.AreaExited += AreaImanOnAreaExited;

        jugador.CallDeferred(Node.MethodName.AddChild, _areaIman);
    }

    private void AreaImanOnAreaEntered(Node body)
    {
        if (body is Moneda moneda)
        {
            AreaImanOnMonedaEntered(moneda);
        }
    }

    private void AreaImanOnMonedaEntered(Moneda moneda)
    {
        _monedasEnRango.Add(moneda);

        CrearEstelaEnMoneda(moneda);
    }

    private static void CrearEstelaEnMoneda(Moneda moneda)
    {

        EfectoEstelaSprite2D estela = UtilidadesNodos.ObtenerNodoDeTipo<EfectoEstelaSprite2D>(moneda);
        if (estela == null)
        {
            estela = new EfectoEstelaSprite2D();
            estela.Inicializar(moneda.Sprite2D, 0.15f);
            moneda.AddChild(estela);
        }

        estela.Activar();
    }


    private void AreaImanOnAreaExited(Node body)
    {
        if (body is Moneda moneda)
        {
            AreaImanOnMonedaExited(moneda);
        }
    }

    private void AreaImanOnMonedaExited(Moneda moneda)
    {
        _monedasEnRango.Remove(moneda);
        QuitarEstelaDeMoneda(moneda);
    }

    private void LimpiarAreaIman()
    {
        foreach (var moneda in _monedasEnRango)
        {
            QuitarEstelaDeMoneda(moneda);
        }

        _monedasEnRango.Clear();

        if (_areaIman != null && IsInstanceValid(_areaIman))
        {
            _areaIman.QueueFree();
        }
    }

    private static void QuitarEstelaDeMoneda(Moneda moneda)
    {
        EfectoEstelaSprite2D estela = UtilidadesNodos.ObtenerNodoDeTipo<EfectoEstelaSprite2D>(moneda);
        estela?.QueueFree();
    }
}
