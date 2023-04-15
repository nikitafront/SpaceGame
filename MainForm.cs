namespace SpaceGame;

public sealed partial class MainForm : Form
{
    private readonly string _pathToAssets = Path.GetFullPath("..\\..\\..\\assets\\");
    private Point _newPlayerLocation = Point.Empty;
    
    private readonly Random rnd = new Random();

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
                Image.FromFile(_pathToAssets + "mainBackgroundImages\\1.jpg"); //TODO make background animated 
        }

        #endregion

        #region PlayerState

        var playerMovingTimerId = rnd.NextInt64();
        
        _player = new PlayerModel(
            new Point(ClientSize.Width / 2, ClientSize.Height - GameMember._size.Height),
            Image.FromFile(Path.GetFullPath(_pathToAssets + "player.png"))
        );

        _player.MovingTimer = new TimersStorage(playerMovingTimerId, true, TimerIntervals.Moving, (s, args) =>
        {
            _player.Location = new Point(
                _player.Location.X + _newPlayerLocation.X,
                _player.Location.Y + _newPlayerLocation.Y
            );
            _newPlayerLocation = new Point(_newPlayerLocation.X * 4 / 10, _newPlayerLocation.Y * 4 / 10);
        });

        #endregion

        #region PlayerMovingState

        KeyDown += (sender, args) =>
        {
            const int offsetValue = 10;
            _newPlayerLocation = Point.Empty;
            switch (args.KeyCode)
            {
                case Keys.Up:
                    _newPlayerLocation.Y += _player.Location.Y <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Down:
                    _newPlayerLocation.Y += _player.Location.Y + GameMember._size.Height * 1.60 >= Size.Height
                        ? 0
                        : offsetValue;
                    break;
                case Keys.Left:
                    _newPlayerLocation.X += _player.Location.X <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Right:
                    _newPlayerLocation.X += _player.Location.X + GameMember._size.Width * 1.35 >= Size.Width
                        ? 0
                        : offsetValue;
                    break;
            }
        };

        #endregion

        #region EasyEnemiesState

        var easyEnemySpawnTimerId = rnd.NextInt64();
        
        var easyEnemiesSpawnTimer = new TimersStorage(
            easyEnemySpawnTimerId,
            true,
            TimerIntervals.EasyEnemySpawn,
            (o, e) =>
            {
                // create new enemy
                var newEnemy = new EnemyModel(
                    new Point((int)rnd.NextInt64(Width), 0),
                    Image.FromFile(Path.GetFullPath(_pathToAssets + "enemy1.png"))
                );
                
                var newEnemyMovingTimerId = rnd.NextInt64();
                
                newEnemy.MovingTimer = new TimersStorage(
                    newEnemyMovingTimerId,
                    true,
                        TimerIntervals.Moving,
                    (obj, evt) =>
                    {
                        newEnemy.PictureBox.Location = newEnemy.Location with { Y = newEnemy.Location.Y + 3 };
                        if (newEnemy.IsCrossing(_player.PictureBox))
                        {
                            _easyEnemies.Remove(newEnemy);
                            TimersStorage.Timers.Remove(newEnemyMovingTimerId);
                            newEnemy.PictureBox.Dispose();
                        }
                    }
                );
                
                _easyEnemies.Add(newEnemy);
                Controls.Add(_easyEnemies.Last().PictureBox);
            });
        
        #endregion

        Controls.Add(_player.PictureBox);
    }
}