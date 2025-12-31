using System.Collections.Generic;
using Godot;
using Primerjuego2D.escenas.entidades.jugador;
using Primerjuego2D.escenas.modelos.objetos;

namespace Primerjuego2D.escenas.objetos.powerup;

public partial class ImanMonedas : PowerUp
{
    [Export] private float RadioIman = 200f;
    [Export] private float FuerzaAtraccion = 6f;

    private Area2D _areaIman;
    private CollisionShape2D _collisionShape;
    private readonly HashSet<Node2D> _monedasEnRango = new();

    public override void AplicarEfectoPowerUp(Jugador jugador)
    {
        CrearAreaIman(jugador);
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
        _areaIman = new Area2D();
        _areaIman.Name = "AreaImanMonedas";

        _collisionShape = new CollisionShape2D
        {
            Shape = new CircleShape2D { Radius = RadioIman }
        };

        _areaIman.AddChild(_collisionShape);
        jugador.AddChild(_areaIman);

        _areaIman.BodyEntered += AreaImanOnBodyEntered;
        _areaIman.BodyExited += AreaImanOnBodyExited;
    }

    private void AreaImanOnBodyEntered(Node body)
    {
        if (body is Node2D moneda)
        {
            _monedasEnRango.Add(moneda);
        }
    }

    private void AreaImanOnBodyExited(Node body)
    {
        if (body is Node2D moneda)
        {
            _monedasEnRango.Remove(moneda);
        }
    }

    private void LimpiarAreaIman()
    {
        _monedasEnRango.Clear();

        if (_areaIman != null && IsInstanceValid(_areaIman))
        {
            _areaIman.QueueFree();
        }
    }
}
