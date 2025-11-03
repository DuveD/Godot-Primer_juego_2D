using Godot;
using System;

public partial class MainHUD : CanvasLayer
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
        InicializarAtributosElementos();

        // Cambiamos el texto al inicial de la partida.
        ShowMessage(Tr("MainHUD.mensaje.esquivaLosEnemigos"));
    }

    private void InicializarAtributosElementos()
    {
        PopupMenu popupMenu = this.MenuButtonLenguaje.GetPopup();
        popupMenu.IdPressed += MenuButtonLenguaje_IdPressed;
    }

    private void MenuButtonLenguaje_IdPressed(long id)
    {
        switch (id)
        {
            default:
            case ID_OPCION_CASTELLANO:
                UtilidadesIdioma.SetIdiomaCastellano();
                break;
            case ID_OPCION_INGLES:
                UtilidadesIdioma.SetIdiomaIngles();
                break;
        }
    }

    public void ShowMessage(string message)
    {
        this.MessageLabel.Text = message;
        this.MessageLabel.Show();
    }

    internal void ShowStartMessage()
    {
        ShowMessage(Tr("MainHUD.mensaje.preparate"));

        this.MessageTimer.Start();
    }

    async private void OnMessageTimerTimeout()
    {
        ShowMessage(Tr("MainHUD.mensaje.vamos"));

        // Creamos un timer de 1 segundo y esperamos.
        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        this.MessageLabel.Hide();
    }

    async public void ShowGameOver()
    {
        // Mostramos el mensaje de "Game Over" en el Label del centro de la pantalla.
        ShowMessage(Tr("MainHUD.mensaje.gameOver"));

        // Esperamos 1 segundo.
        await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);

        // Cambiamos el texto al inicial de la partida.
        ShowMessage(Tr("MainHUD.mensaje.esquivaLosEnemigos"));

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
        EmitSignal(SignalName.StartGame);
    }
}
