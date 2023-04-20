namespace SpaceGame;

public partial class MainForm : Form
{
    public static readonly string PathToAssets = Path.GetFullPath("..\\..\\..\\assets\\");
    private Point _newPlayerLocation = Point.Empty;

    private readonly Random rnd = new();

    private PlayerModel _player;
    private readonly List<EnemyModel> _easyEnemies = new List<EnemyModel>();
    private readonly List<EnemyModel> _middleEnemies = new List<EnemyModel>();
    private readonly List<EnemyModel> _hardEnemies = new List<EnemyModel>();
    private readonly List<EnemyModel> _bosses = new List<EnemyModel>();

    public MainForm()
    {
        #region WindowState

        {
            Text = "SpaceGame";
            Size = new Size(1240, 720);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackgroundImage =
                Image.FromFile(PathToAssets + "mainBackgroundImages\\1.jpg"); //TODO make background animated
        }

        #endregion

        #region PlayerState

        var playerMovingTimerId = rnd.NextInt64();

        _player = new PlayerModel(
            new Point(ClientSize.Width / 2, ClientSize.Height - GameMember.PlayerSize.Height),
            GameMember.PlayerSize,
            Image.FromFile(Path.GetFullPath(PathToAssets + "player.png"))
        );

        new TimersStorage(
            playerMovingTimerId,
            true,
            TimerIntervals.Moving,
            (s, args) =>
            {
                _player.Location = new Point(
                    _player.Location.X + _newPlayerLocation.X,
                    _player.Location.Y + _newPlayerLocation.Y
                );
                _newPlayerLocation = Point.Empty;
            });

        new TimersStorage(true, TimerIntervals.Shoot, (o, e) => _player.Shoot(Controls));

        new TimersStorage(
            true,
            TimerIntervals.BulletSpeed,
            (o, e) =>
            {
                var toRemove = new List<PictureBox>();

                foreach (var b in _player.bullets)
                {
                    b.Location = b.Location with { Y = b.Location.Y - 20 };
                    if (b.Location.Y < -10)
                        toRemove.Add(b);
                }

                foreach (var b in toRemove)
                {
                    _player.bullets.Remove(b);
                    Controls.Remove(b);
                }
            }
        );

        Controls.Add(_player.PictureBox);

        #endregion

        #region PlayerMovingState

        KeyDown += (sender, args) =>
        {
            const int offsetValue = 50;
            _newPlayerLocation = Point.Empty;
            switch (args.KeyCode)
            {
                case Keys.Up:
                    _newPlayerLocation.Y += _player.Location.Y <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Down:
                    _newPlayerLocation.Y += _player.Location.Y + GameMember.PlayerSize.Height >= Size.Height
                        ? 0
                        : offsetValue;
                    break;
                case Keys.Left:
                    _newPlayerLocation.X += _player.Location.X <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Right:
                    _newPlayerLocation.X += _player.Location.X + GameMember.PlayerSize.Width >= Size.Width
                        ? 0
                        : offsetValue;
                    break;
            }
        };

        #endregion

        #region EnemiesState

        var easyEnemySpawnTimerId = rnd.NextInt64();
        var middleEnemySpawnTimerId = rnd.NextInt64();
        var hardEnemySpawnTimerId = rnd.NextInt64();
        var bossSpawnTimerId = rnd.NextInt64();

        CreateSpawnTimer(easyEnemySpawnTimerId, TimerIntervals.EasyEnemySpawn,
            GameMember.EasyEnemySize, "enemy1.png", _easyEnemies);

        CreateSpawnTimer(middleEnemySpawnTimerId, TimerIntervals.MiddleEnemySpawn,
            GameMember.MiddleEnemySize, "enemy2.png", _middleEnemies);

        CreateSpawnTimer(hardEnemySpawnTimerId, TimerIntervals.HardEnemySpawn,
            GameMember.HardEnemySize, "enemy3.png", _hardEnemies);

        CreateSpawnTimer(bossSpawnTimerId, TimerIntervals.BossSpawn,
            GameMember.BossSize, "enemy4.png", _bosses);

        #endregion

        #region EnemiesMoving

        var easyEnemyMovingTimerId = rnd.NextInt64();

        new TimersStorage(
            easyEnemyMovingTimerId,
            true,
            TimerIntervals.Moving,
            (o, e) =>
            {
                MoveEnemies(_easyEnemies, EnemiesSpeed.EasyEnemySpeed);
                MoveEnemies(_middleEnemies, EnemiesSpeed.MiddleEnemySpeed);
                MoveEnemies(_hardEnemies, EnemiesSpeed.HardEnemySpeed);
                MoveEnemies(_bosses, EnemiesSpeed.BossSpeed);
            });

        #endregion
    }

    private void MoveEnemies(IEnumerable<EnemyModel> enemies, EnemiesSpeed movingSpeed)
    {
        foreach (var enemy in enemies)
            enemy.Location = enemy.Location with { Y = enemy.Location.Y + (int)movingSpeed };
    }

    private void CreateSpawnTimer(
        long id,
        TimerIntervals spawnInterval,
        Size enemySize,
        string spriteFileName,
        ICollection<EnemyModel> enemiesList
    )
    {
        new TimersStorage(
            id,
            true,
            spawnInterval,
            (o, e) =>
            {
                // create new enemy
                var newEnemy = new EnemyModel(
                    new Point((int)rnd.NextInt64(Width - enemySize.Width), 0 - enemySize.Height),
                    enemySize,
                    Image.FromFile(Path.GetFullPath(PathToAssets + spriteFileName))
                );

                enemiesList.Add(newEnemy);
                Controls.Add(enemiesList.Last().PictureBox);
            });
    }
}