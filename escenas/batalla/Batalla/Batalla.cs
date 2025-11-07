using Godot;
using System;
using System.Text.RegularExpressions;

public partial class Batalla : Node
{
    [Export]
    public PackedScene EnemyScene { get; set; }

    private Timer _EnemyTimer;
    private Timer EnemyTimer => _EnemyTimer ??= GetNode<Timer>("EnemyTimer");

    private Timer _StartTimer;
    private Timer StartTimer => _StartTimer ??= GetNode<Timer>("StartTimer");

    private Timer _ScoreTimer;
    private Timer ScoreTimer => _ScoreTimer ??= GetNode<Timer>("ScoreTimer");

    private BatallaHUD _BatallaHUD;
    private BatallaHUD BatallaHUD => _BatallaHUD ??= GetNode<BatallaHUD>("BatallaHUD");

    private Player _Player;
    private Player Player => _Player ??= GetNode<Player>("Player");

    private Marker2D _StartPosition;
    private Marker2D StartPosition => _StartPosition ??= GetNode<Marker2D>("StartPosition");

    private PathFollow2D _MobSpawnLocation;
    private PathFollow2D MobSpawnLocation => _MobSpawnLocation ??= GetNode<PathFollow2D>("EnemyPath/EnemySpawnLocation");

    private int _score;

    public override void _Ready()
    {
    }

    public void NewGame()
    {
        Enemy.DeleteAllEnemies(this);

        this._score = 0;

        this.Player.Start(this.StartPosition.Position);
        this.StartTimer.Start();

        this.BatallaHUD.UpdateScore(_score);
        this.BatallaHUD.ShowStartMessage();
    }

    public void GameOver()
    {
        this.EnemyTimer.Stop();
        this.ScoreTimer.Stop();

        this.BatallaHUD.ShowGameOver();
    }

    private void OnScoreTimerTimeout()
    {
        this._score++;
        this.BatallaHUD.UpdateScore(_score);
    }

    private void OnStartTimerTimeout()
    {
        this.EnemyTimer.Start();
        this.ScoreTimer.Start();
    }

    private void OnEnemyTimerTimeout()
    {
        // Creamos una nueva instancia de un enemigo.
        Enemy enemy = EnemyScene.Instantiate<Enemy>();

        // Elegimos una localización aleatória del path 2D de los enemigos.
        this.MobSpawnLocation.ProgressRatio = Randomizer.GetRandomFloat();

        // Set the mob's position to a random location.
        enemy.Position = this.MobSpawnLocation.Position;

        // Informamos la dirección del sprite enemigo. Perpendicular a la dirección del path 2D de los enemigos. 
        float direction = this.MobSpawnLocation.RotationDegrees + 90;

        // Randomizamos la dirección, de -45 a 45.
        direction += (float)Randomizer.GetRandomInt(-45, 45);
        enemy.RotationDegrees = direction;

        // Informamos el vector de velocidad y dirección.
        Vector2 velocity = new Vector2((float)Randomizer.GetRandomDouble(150.0, 250.0), 0);
        float directionRad = (float)UtilidadesMatematicas.DegreesToRadians(direction);
        enemy.LinearVelocity = velocity.Rotated(directionRad);

        // Spawneamos el enemigo en la escena principal.
        this.AddChild(enemy);
    }
}
