using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using static GestorIdioma;
using System.Linq;

public partial class BatallaHUD : CanvasLayer
{
    public const long ID_OPCION_CASTELLANO = 0;
    public const long ID_OPCION_INGLES = 1;

    [Signal]
    public delegate void StartGameEventHandler();

    private Label _MessageLabel;
    private Label MessageLabel => _MessageLabel ??= GetNode<Label>("Message");

    private Timer _MessageTimer;
    private Timer MessageTimer => _MessageTimer ??= GetNode<Timer>("MessageTimer");

    private Button _StartButton;
    private Button StartButton => _StartButton ??= GetNode<Button>("StartButton");

    private Label _ScoreLabel;
    private Label ScoreLabel => _ScoreLabel ??= GetNode<Label>("ScoreLabel");

    private MenuButton _MenuButtonLenguaje;
    private MenuButton MenuButtonLenguaje => _MenuButtonLenguaje ??= GetNode<MenuButton>("MenuButtonLenguaje");

    public override void _Ready()
    {
        InicializarMenuButtonLenguaje();

        // Cambiamos el texto al inicial de la partida.
        ShowMessage("BatallaHUD.mensaje.esquivaLosEnemigos");
    }

    private void InicializarMenuButtonLenguaje()
    {
        PopupMenu popupMenu = this.MenuButtonLenguaje.GetPopup();
        popupMenu.IdPressed += MenuButtonLenguaje_IdPressed;

        Idioma idioma = GestorIdioma.GetIdiomaActual();
        switch (idioma)
        {
            default:
            case Idioma.Castellano:
                MenuButtonLenguaje_IdPressed(ID_OPCION_CASTELLANO);
                break;
            case Idioma.Ingles:
                MenuButtonLenguaje_IdPressed(ID_OPCION_INGLES);
                break;
        }
    }

    private void MenuButtonLenguaje_IdPressed(long id)
    {
        var popupMenu = MenuButtonLenguaje.GetPopup();

        // 🔹 Primero desmarcamos todos los ítems
        for (int i = 0; i < popupMenu.ItemCount; i++)
            popupMenu.SetItemChecked(i, false);

        // 🔹 Obtenemos el índice del ítem a partir de su ID
        int index = popupMenu.GetItemIndex((int)id);

        // 🔹 Marcamos solo el seleccionado
        popupMenu.SetItemChecked(index, true);

        switch (id)
        {
            default:
            case ID_OPCION_CASTELLANO:
                GestorIdioma.SetIdiomaCastellano();
                break;
            case ID_OPCION_INGLES:
                GestorIdioma.SetIdiomaIngles();
                break;
        }
    }

    public bool MostrarMessageLabel = false;

    public void ShowMessage(string tagMensaje)
    {
        this.MessageLabel.Text = tagMensaje;
        this.MessageLabel.Show();
        this.MostrarMessageLabel = true;
    }

    internal void ShowStartMessage()
    {
        ShowMessage("BatallaHUD.mensaje.preparate");

        this.MessageTimer.Start();
    }

    async private void OnMessageTimerTimeout()
    {
        ShowMessage("BatallaHUD.mensaje.vamos");

        // Creamos un timer de 1 segundo y esperamos.
        await UtilidadesNodos.EsperarSegundos(this, 1.0);
        this.MessageLabel.Hide();
    }

    async public void ShowGameOver()
    {
        // Mostramos el mensaje de "Game Over" en el Label del centro de la pantalla.
        ShowMessage("BatallaHUD.mensaje.gameOver");

        // Esperamos 2 segundos.
        await UtilidadesNodos.EsperarSegundos(this, 2.0);

        // Cambiamos el texto al inicial de la partida.
        ShowMessage("BatallaHUD.mensaje.esquivaLosEnemigos");

        // Mostramos lel botón de selección de idioma.
        this.MenuButtonLenguaje.Show();

        // Creamos un timer de 1 segundo y esperamos.
        await UtilidadesNodos.EsperarSegundos(this, 1.0);

        // Mostramos el botón de start.
        this.StartButton.Show();
    }

    public void UpdateScore(int score)
    {
        this.ScoreLabel.Text = score.ToString();
    }

    private void OnStartButtonPressed()
    {
        this.StartButton.Hide();
        this.MenuButtonLenguaje.Hide();
        EmitSignal(SignalName.StartGame);
    }

    Dictionary<CanvasItem, bool> visibilidadElementosPausa;

    public void OnPauseButtonPressed()
    {
        if (Ajustes.JuegoPausado)
        {
            this.visibilidadElementosPausa = this.GetChildren()
                .OfType<CanvasItem>()
                .ToDictionary(item => item, item => item.Visible);

            UtilidadesNodos.EsconderMenos(this, this.ScoreLabel);
        }
        else
        {
            var elementosVisibles = this.visibilidadElementosPausa
    .Where(kv => !kv.Key.Visible && kv.Value == true)
    .Select(kv => kv.Key)
    .ToList();

            foreach (var elemento in elementosVisibles)
                elemento.Show();
        }
    }
}
