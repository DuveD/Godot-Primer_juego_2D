using Godot;

namespace Primerjuego2D.escenas.miscelaneo.animaciones;

public partial class AnimacionCrtShutdown : CanvasLayer
{
    [Signal]
    public delegate void AnimacionFinalizadaEventHandler();

    private AnimationPlayer _AnimationPlayer;
    private AnimationPlayer AnimationPlayer => _AnimationPlayer ??= this.GetNode<AnimationPlayer>("AnimationPlayer");

    public AnimacionCrtShutdown()
    {
        Visible = false;
    }

    public override void _Ready()
    {
        AnimationPlayer.AnimationFinished += OnAnimacionFinalizada;
    }

    private void OnAnimacionFinalizada(StringName animName)
    {
        EmitSignal(SignalName.AnimacionFinalizada);
    }

    public void Reproducir()
    {
        Visible = true;
        AnimationPlayer.Play("apagar_tv");
    }
}
