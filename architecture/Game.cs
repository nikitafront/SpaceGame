using System.Windows.Forms.VisualStyles;
using Timer = System.Windows.Forms.Timer;

namespace SpaceGame.architecture;

public class Game
{
    private Timer Timer { get; }
    private long PassedMilliseconds { get; set; }

    public PlayerModel Player { get; set; }
    public List<EnemyModel> EasyEnemies { get; set; } = new ();
    public List<EnemyModel> MiddleEnemies { get; set; } = new ();
    public List<EnemyModel> HardEnemies { get; set; } = new ();
    public List<EnemyModel> Bosses { get; set; } = new ();
    public List<List<EnemyModel>> AllEnemies { get; set; }
    public Control.ControlCollection Controls { get; set; }

    #region Constants
    public const long MovingInterval = 200;
    
    public const int EasyEnemySpeed = 10;
    public const long EasyEnemySpawnInterval = 8_000;
    public const long EasyEnemyShootInterval = 4_000;
    
    public const int MiddleEnemySpeed = 7;
    public const long MiddleEnemySpawnInterval = 16_000;
    
    
    public const int BossMovingSpeed = 2;
    public const long BossSpawnInterval = 120_000;
    
    public const long PlayerShootInterval = 600;
    public const int BulletSpeed = 20;
    #endregion

    public Game()
    {
        Timer = new Timer{ Enabled = true, Interval = 100 };
        Timer.Tick += (o, e) =>
        {
            PassedMilliseconds += 100;
            ReDraw(o, e);
        };
        AllEnemies = new() { EasyEnemies, MiddleEnemies, HardEnemies, Bosses };
    }

    private async void ReDraw(object? sender, EventArgs args)
    {
        if (PassedMilliseconds % MovingInterval == 0)
        {
            Player.Move();
            AllEnemies.ForEach(l => l.ForEach(i => i.Move(Controls)));
        }
        
        if (PassedMilliseconds % PlayerShootInterval == 0) Player.Shoot(Controls);
        
        if (PassedMilliseconds % EasyEnemyShootInterval == 0) 
            AllEnemies
                .SelectMany(x => x)
                .Where(x => x.Type == GameMemberTypes.EasyEnemy)
                .ToList()
                .ForEach(e => e.Shoot(Controls));

        if (PassedMilliseconds % BulletSpeed == 0)
        {
            Player.FlyBullets(Controls, AllEnemies);
            AllEnemies.ForEach(l => l.ForEach(e => e.FlyBullets(Controls)));
        }

        if (PassedMilliseconds % EasyEnemySpawnInterval == 0)
        {
            var newEnemy = await SpawnEasyEnemy();
            lock (EasyEnemies) EasyEnemies.Add(newEnemy);
            Controls.Add(newEnemy.PictureBox);
        }
    }
    private Task<EnemyModel> SpawnEasyEnemy() //TODO rebuild for abstract enemy (fabric)
    {
        var task = new Task<EnemyModel>(() =>
        {
            var newEnemyLocation =
                new Point(new Random().Next(0, new MainForm().Size.Width - GameMember.EasyEnemySize.Width),
                    -GameMember.EasyEnemySize.Height);
            var newEnemy = new EnemyModel(newEnemyLocation, GameMember.EasyEnemySize,
                Image.FromFile(MainForm.PathToAssets + "enemy1.png"), GameMemberTypes.EasyEnemy);
            return newEnemy;
        });
        task.Start();
        return task;
    }
}