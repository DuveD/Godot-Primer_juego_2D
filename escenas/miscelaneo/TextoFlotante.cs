using Godot;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;

namespace Primerjuego2D.escenas.miscelaneo;

public partial class TextoFlotante : Node2D
{
    // TEXTO
    [Export]
    public StringName Texto;

    // APARIENCIA
    [Export]
    public Color Color = Colors.White;

    // COMPORTAMIENTO
    [Export]
    public float VelocidadSubida = 30f;

    [Export]
    public float Duracion = 1.2f;

    // POSICIÓN INICIAL (GLOBAL)
    [Export] public Vector2 PosicionGlobal;

    private Label _Label;

    private Label Label => _Label ??= GetNode<Label>("Label");

    private float _tiempo;

    public override void _Ready()
    {
        UtilidadesNodos2D.AjustarZIndexNodo(this, ConstantesZIndex.INTERFAZ_TEMPORAL);

        // Texto traducible
        Label.Text = Tr(Texto);

        // Color inicial
        Label.Modulate = new Color(Color.R, Color.G, Color.B, 1f);

        // Posición global
        GlobalPosition = PosicionGlobal;
    }

    public override void _Process(double delta)
    {
        float d = (float)delta;
        _tiempo += d;

        // Subir
        Position += Vector2.Up * VelocidadSubida * d;

        // Desvanecer
        float alpha = 1f - (_tiempo / Duracion);
        Label.Modulate = new Color(Color.R, Color.G, Color.B, Mathf.Max(alpha, 0));

        if (_tiempo >= Duracion)
        {
            QueueFree();
        }
    }
}
