using Godot;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.miscelaneo.animaciones;

public partial class EfectoEstelaSprite2D : Node2D
{
  public AnimatedSprite2D Sprite { get; private set; }

  public PackedScene EstelaSprite2DScene;

  public float Intervalo { get; set; } = 0.05f;

  public bool Activo
  {
    get => _activo;
    set
    {
      _activo = value;
      SetProcess(value);
    }
  }
  private bool _activo = false;

  private double _tiempo;

  public override void _Ready()
  {
    SetProcess(false);
    EstelaSprite2DScene = GD.Load<PackedScene>("res://escenas/miscelaneo/animaciones/EstelaSprite2d.tscn");
  }

  public void Inicializar(AnimatedSprite2D sprite, float intervalo = 0.05f)
  {
    Sprite = sprite;
    Intervalo = intervalo;
  }

  public override void _Process(double delta)
  {
    if (!Activo || Sprite == null || !Sprite.Visible)
      return;

    _tiempo += delta;
    if (_tiempo >= Intervalo)
    {
      _tiempo = 0;
      EmitirEstelaSprite2D();
    }
  }

  private void EmitirEstelaSprite2D()
  {
    EstelaSprite2d estelaSprite2d = EstelaSprite2DScene.Instantiate<EstelaSprite2d>();

    estelaSprite2d.TopLevel = true;

    estelaSprite2d.GlobalRotation = Sprite.GlobalRotation;
    estelaSprite2d.Scale = Sprite.Scale;

    estelaSprite2d.SpriteFrames = Sprite.SpriteFrames;
    estelaSprite2d.Animation = Sprite.Animation;
    estelaSprite2d.Frame = Sprite.Frame;
    estelaSprite2d.FlipH = Sprite.FlipH;
    estelaSprite2d.FlipV = Sprite.FlipV;
    estelaSprite2d.Modulate = Sprite.Modulate;

    AddChild(estelaSprite2d);
    estelaSprite2d.GlobalPosition = Sprite.GlobalPosition;
  }

  public void Activar()
  {
    _tiempo = 0;
    Activo = true;
  }

  public void Desactivar()
  {
    Activo = false;
  }
}
