using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.sistema.audio.efectos;
using Primerjuego2D.nucleo.constantes;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla.HUD;

public partial class BatallaHUD : CanvasLayer
{
    private Label MessageLabel;

    private Label LabelGameOver;

    private Label ScoreLabel;

    private PanelMenuPausa PanelMenuPausa;

    private HBoxContainer HBoxContainerVidas;

    Dictionary<CanvasItem, bool> VisibilidadElementosPausa;

    [Export]
    public PackedScene VidaSpriteScene;

    private List<Control> _spritesVidas = new();


    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.MessageLabel = GetNode<Label>("Message");
        this.LabelGameOver = GetNode<Label>("LabelGameOver");
        this.ScoreLabel = GetNode<Label>("ScoreLabel");
        this.PanelMenuPausa = GetNode<PanelMenuPausa>("PanelMenuPausa");
        this.HBoxContainerVidas = UtilidadesNodos.ObtenerNodoPorNombre<HBoxContainer>(this, "HBoxContainerVidas");

        UtilidadesLayer.AjustarLayerNodo(this, ConstantesLayer.HUD);
        UtilidadesLayer.AjustarLayerNodo(PanelMenuPausa, ConstantesLayer.POPUPS);

        this.LabelGameOver.Hide();
    }

    async public void MostrarMensajesIniciarBatalla()
    {
        this.MessageLabel.Show();

        // Cambiamos el texto al inicial de la partida.
        this.MessageLabel.Text = "BatallaHUD.mensaje.preparate";

        await UtilidadesNodos.EsperarSegundos(this, 2.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        this.MessageLabel.Text = "BatallaHUD.mensaje.vamos";

        // Creamos un timer de 1 segundo y esperamos.
        await UtilidadesNodos.EsperarSegundos(this, 1.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        this.MessageLabel.Hide();
    }

    public void MostrarMensajeGameOver()
    {
        this.MessageLabel.Hide();
        this.LabelGameOver.Show();
    }

    public void ActualizarPuntuacion(int score)
    {
        this.ScoreLabel.Text = score.ToString();
    }

    private void EsconderHUD()
    {
        this.VisibilidadElementosPausa = this.GetChildren()
            .OfType<CanvasItem>()
            .Where(item => item != this.PanelMenuPausa && item != this.ScoreLabel)
            .ToDictionary(item => item, item => item.Visible);

        UtilidadesNodos.EsconderMenos(this, this.ScoreLabel);

        MostrarContenedorPausa();
    }

    private void MostrarHUD()
    {
        EsconderContenedorPausa();

        var elementosVisibles = this.VisibilidadElementosPausa
            .Where(kv => !kv.Key.Visible && kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var elemento in elementosVisibles)
            elemento.Show();
    }

    public void MostrarContenedorPausa()
    {
        Global.GestorAudio.ReproducirSonido("pause.mp3");
        Global.GestorEfectosAudio.Activar(EfectoBajoElAgua.ID);
        this.PanelMenuPausa.Show();
    }

    public void EsconderContenedorPausa()
    {
        Global.GestorAudio.ReproducirSonido("unpause.mp3");
        Global.GestorEfectosAudio.Desactivar(EfectoBajoElAgua.ID);
        this.PanelMenuPausa.Hide();
    }

    public void OnCambioVida(int vida)
    {
        // Limpiar sprites antiguos

        var spritesVidas = UtilidadesNodos.ObtenerNodosDeTipo<SpriteVida>(HBoxContainerVidas);
        foreach (var spriteVida in spritesVidas)
            spriteVida.QueueFree();

        _spritesVidas.Clear();

        // Crear nuevas vidas
        for (int i = 0; i < vida; i++)
        {
            SpriteVida spriteVida = VidaSpriteScene.Instantiate<SpriteVida>();
            HBoxContainerVidas.AddChild(spriteVida);
            _spritesVidas.Add(spriteVida);
        }
    }
}
