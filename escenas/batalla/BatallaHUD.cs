namespace Primerjuego2D.escenas.batalla;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Godot;
using Primerjuego2D.nucleo.ajustes;
using Primerjuego2D.nucleo.localizacion;
using Primerjuego2D.nucleo.utilidades;
using Primerjuego2D.nucleo.utilidades.log;
using static Primerjuego2D.nucleo.localizacion.GestorIdioma;

public partial class BatallaHUD : CanvasLayer
{
    private Label _MessageLabel;
    private Label MessageLabel => _MessageLabel ??= GetNode<Label>("Message");

    private Timer _MessageTimer;
    private Timer MessageTimer => _MessageTimer ??= GetNode<Timer>("MessageTimer");

    private Label _ScoreLabel;
    private Label ScoreLabel => _ScoreLabel ??= GetNode<Label>("ScoreLabel");

    private Label _MensajePausa;
    private Label MensajePausa => _MensajePausa ??= GetNode<Label>("MensajePausa");

    private Batalla _Batalla;
    private Batalla Batalla => _Batalla ??= GetParent<Batalla>();

    private BatallaControlador _Batallacontrolador;
    private BatallaControlador BatallaControlador => _Batallacontrolador ??= GetNode<BatallaControlador>("../BatallaControlador");

    Dictionary<CanvasItem, bool> visibilidadElementosPausa;

    public override void _Ready()
    {
        Logger.Trace("BatallaHUD Ready.");
    }

    async public void MostrarMensajePreparate()
    {
        await UtilidadesNodos.EsperarSegundos(this, 1.0);
        await UtilidadesNodos.EsperarRenaudar(this);

        // Cambiamos el texto al inicial de la partida.
        this.MessageLabel.Text = "BatallaHUD.mensaje.preparate";
    }

    async private void MostrarMensajeIniciarBatalla()
    {
        this.MessageLabel.Text = "BatallaHUD.mensaje.vamos";

        // Creamos un timer de 1 segundo y esperamos.
        await UtilidadesNodos.EsperarSegundos(this, 1.0);
        this.MessageLabel.Hide();
    }

    async public void ShowGameOver()
    {
        await UtilidadesNodos.EsperarRenaudar(this);

        // Mostramos el mensaje de "Game Over" en el Label del centro de la pantalla.
        this.MessageLabel.Text = "BatallaHUD.mensaje.gameOver";
        this.MessageLabel.Show();

        // Esperamos 2 segundos.
        await UtilidadesNodos.EsperarSegundos(this, 2.0);
        await UtilidadesNodos.EsperarRenaudar(this);
    }

    public void ActualizarPuntuacion(int score)
    {
        this.ScoreLabel.Text = score.ToString();
    }

    public void OnPauseBattle()
    {
        if (Ajustes.JuegoPausado)
        {
            this.visibilidadElementosPausa = this.GetChildren()
                .OfType<CanvasItem>()
                .Where(item => item != this.MensajePausa && item != this.ScoreLabel)
                .ToDictionary(item => item, item => item.Visible);

            UtilidadesNodos.EsconderMenos(this, this.ScoreLabel);

            this.MensajePausa.Show();
        }
        else
        {
            var elementosVisibles = this.visibilidadElementosPausa
                .Where(kv => !kv.Key.Visible && kv.Value == true)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var elemento in elementosVisibles)
                elemento.Show();

            this.MensajePausa.Hide();
        }
    }
}
