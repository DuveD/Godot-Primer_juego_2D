using System;
using Godot;

namespace Primerjuego2D.escenas.miscelaneo.animaciones;

public partial class EstelaSprite2d : AnimatedSprite2D
{
    [Export]
    public float Duracion = 0.3f;

    public override void _Ready()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate:a", 0f, Duracion);
        tween.Finished += QueueFree;
    }
}
