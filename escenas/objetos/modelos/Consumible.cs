using Godot;
using Primerjuego2D.escenas.entidades.jugador;

namespace Primerjuego2D.escenas.objetos.modelos;

public abstract partial class Consumible : Area2D
{
    private Sprite2D _Sprite2D;
    public Sprite2D Sprite2D => _Sprite2D ??= GetNode<Sprite2D>("Sprite2D");


    private CollisionShape2D _CollisionShape2D;
    public CollisionShape2D CollisionShape2D => _CollisionShape2D ??= GetNode<CollisionShape2D>("CollisionShape2D");

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Jugador jugador)
        {
            OnRecogida(jugador);
        }
    }

    public abstract void OnRecogida(Jugador jugador);
}