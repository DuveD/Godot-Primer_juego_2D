using Godot;

namespace Primerjuego2D.escenas.sistema.audio.efectos;

public class EfectoBajoElAgua : EfectoAudio
{
  public const string ID = "bajo_el_agua";

  public override string Id => ID;

  public override string Bus => GestorAudio.BUS_MUSICA;

  public override string Nombre => nameof(EfectoBajoElAgua);

  public override bool Perenne => true;

  protected override AudioEffect CrearEfecto()
  {
    AudioEffectLowPassFilter efecto = new()
    {
      CutoffHz = 800f
    };

    return efecto;
  }
}
