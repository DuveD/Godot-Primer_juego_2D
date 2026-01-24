using System.Collections.Generic;
using System.Linq;
using Godot;
using Primerjuego2D.escenas.menuPrincipal;
using Primerjuego2D.escenas.sistema.audio.efectos;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;

namespace Primerjuego2D.escenas.batalla;

public partial class BatallaHUD : CanvasLayer
{
    private Label _MessageLabel;
    private Label MessageLabel => _MessageLabel ??= GetNode<Label>("Message");

    private Label _LabelGameOver;
    private Label LabelGameOver => _LabelGameOver ??= GetNode<Label>("LabelGameOver");

    private Label _ScoreLabel;
    private Label ScoreLabel => _ScoreLabel ??= GetNode<Label>("ScoreLabel");

    private PanelMenuPausa _PanelMenuPausa;
    public PanelMenuPausa PanelMenuPausa => _PanelMenuPausa ??= GetNode<PanelMenuPausa>("PanelMenuPausa");

    Dictionary<CanvasItem, bool> VisibilidadElementosPausa;

    public override void _Ready()
    {
        LoggerJuego.Trace(this.Name + " Ready.");

        this.LabelGameOver.Hide();
    }

    async public void MostrarMensajesIniciarBatalla()
    {
        // Cambiamos el texto al inicial de la partida.
        ActualizarMensaje("BatallaHUD.mensaje.preparate");

        await UtilidadesNodos.EsperarSegundos(this, 2.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        ActualizarMensaje("BatallaHUD.mensaje.vamos");

        // Creamos un timer de 1 segundo y esperamos.
        await UtilidadesNodos.EsperarSegundos(this, 1.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        MostrarMensaje(false);
    }

    public void MostrarMensajeGameOver()
    {
        this.MessageLabel.Hide();
        this.LabelGameOver.Show();
    }

    public void ActualizarMensaje(string mensaje)
    {
        this.MessageLabel.Text = mensaje;
    }

    public void MostrarMensaje(bool mostrar)
    {
        if (mostrar)
            this.MessageLabel.Show();
        else
            this.MessageLabel.Hide();
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
}
