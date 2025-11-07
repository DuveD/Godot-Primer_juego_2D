using Godot;
using System;
using static GestorIdioma;

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
        ShowMessage(Tr("BatallaHUD.mensaje.esquivaLosEnemigos"));
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

        ShowMessage(Tr("BatallaHUD.mensaje.esquivaLosEnemigos"));
    }

    public void ShowMessage(string message)
    {
        this.MessageLabel.Text = message;
        this.MessageLabel.Show();
    }

    internal void ShowStartMessage()
    {
        ShowMessage(Tr("BatallaHUD.mensaje.preparate"));

        this.MessageTimer.Start();
    }

    async private void OnMessageTimerTimeout()
    {
        ShowMessage(Tr("BatallaHUD.mensaje.vamos"));

        // Creamos un timer de 1 segundo y esperamos.
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        this.MessageLabel.Hide();
    }

    async public void ShowGameOver()
    {
        // Mostramos el mensaje de "Game Over" en el Label del centro de la pantalla.
        ShowMessage(Tr("BatallaHUD.mensaje.gameOver"));

        // Esperamos 1 segundo.
        await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);

        // Cambiamos el texto al inicial de la partida.
        ShowMessage(Tr("BatallaHUD.mensaje.esquivaLosEnemigos"));
        this.MenuButtonLenguaje.Show();

        // Creamos un timer de 1 segundo y esperamos.
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
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
}
