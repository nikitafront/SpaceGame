using Timer = System.Windows.Forms.Timer;

namespace SpaceGame.architecture;

public class Game
{
    public Timer Timer { get; private set; }
    private bool IsTimerWasRemoved = false;
    public static Game GetCurrentGame { get; private set; }
    private static long PassedMilliseconds { get; set; }
    public PlayerModel Player { get; set; }
    public static List<EnemyModel> EasyEnemies { get; set; } = new();
    public static List<EnemyModel> MiddleEnemies { get; set; } = new();
    public static List<EnemyModel> HardEnemies { get; set; } = new();
    public static List<EnemyModel> Bosses { get; set; } = new();
    public static List<List<EnemyModel>> AllEnemies { get; set; }
    public static List<BonusModel> Bonuses { get; set; } = new();
    public static List<Control> PlayerLifes { get; set; } = new();
    public static List<Control> MenuPauseControls { get; set; } = new();
    public Control.ControlCollection Controls { get; set; }

    #region Constants

    public const long MovingInterval = 200;

    public const int EasyEnemySpeed = 10;
    public const long EasyEnemySpawnInterval = 8_000;
    public const long EasyEnemyShootInterval = 4_000;

    public const int MiddleEnemySpeed = 7;
    public const int BonusSpeed = 5;
    public const long MiddleEnemySpawnInterval = 16_000;

    public const int BossMovingSpeed = 2;
    public const long BossSpawnInterval = 120_000;

    public const long PlayerShootInterval = 600;
    public const int BulletSpeed = 20;

    #endregion

    public Game()
    {
        Timer = new Timer { Enabled = true, Interval = 100 };
        GetCurrentGame = this;
        Timer.Tick += (o, e) =>
        {
            PassedMilliseconds += 100;
            ReDraw(o, e);
        };
        AllEnemies = new() { EasyEnemies, MiddleEnemies, HardEnemies, Bosses };
        MainForm.GetMainForm.KeyDown += (_, args) =>
        {
            if (args.KeyCode == Keys.Escape) MoveToPauseMenu(Timer);
        };
    }

    private async void ReDraw(object? sender, EventArgs args)
    {
        #region initial draw
        
        if (PassedMilliseconds == 100)
        {
            for (int i = 0; i < Player.LifeCount; i++)
            {
                var life =
                    new PictureBox
                    {
                        Location = new Point(10 + 45 * i, 10),
                        Size = GameObject.HeartSize,
                        BackColor = Color.Transparent,
                        Image = Image.FromFile(MainForm.PathToAssets + "life.png")
                    };

                Controls.Add(life);
                PlayerLifes.Add(life);
            }
        }
        #endregion

        if (PassedMilliseconds % MovingInterval == 0)
        {
            Player.Move(Controls, Bonuses);
            if (IsTimerWasRemoved) return;
            AllEnemies.ForEach(l => l.ForEach(i => i.Move(Controls)));

            if (PlayerLifes.Count != Player.LifeCount && PlayerLifes.Count > 0)
            {
                Controls.Remove(PlayerLifes.Last());
                PlayerLifes.Remove(PlayerLifes.Last());
            }
        }

        #region MyRegion

        
        if (PassedMilliseconds % PlayerShootInterval == 0) Player.Shoot(Controls);

        if (PassedMilliseconds % EasyEnemyShootInterval == 0)
            AllEnemies
                .SelectMany(x => x)
                .Where(x => x.Type == GameMemberTypes.EasyEnemy)
                .ToList()
                .ForEach(e => e.Shoot(Controls));

        if (PassedMilliseconds % BulletSpeed == 0)
        {
            Player.FlyBullets(Controls, AllEnemies, Bonuses);
            AllEnemies.ForEach(l => l.ForEach(e => e.FlyBullets(Controls)));
        }

        if (PassedMilliseconds % BonusSpeed == 0) Bonuses.ForEach(b => b.Move(Controls));

        if (PassedMilliseconds % EasyEnemySpawnInterval == 0)
        {
            var newEnemy = await SpawnEasyEnemy();
            lock (EasyEnemies) EasyEnemies.Add(newEnemy);
            Controls.Add(newEnemy.PictureBox);
        }

        #endregion
    }

    #region MyRegion

    
    private Task<EnemyModel> SpawnEasyEnemy() //TODO rebuild for abstract enemy (fabric)
    {
        var task = new Task<EnemyModel>(() =>
        {
            var newEnemyLocation =
                new Point(new Random().Next(0, new MainForm().Size.Width - GameObject.EasyEnemySize.Width),
                    -GameObject.EasyEnemySize.Height);
            var newEnemy = new EnemyModel(newEnemyLocation, GameObject.EasyEnemySize,
                Image.FromFile(MainForm.PathToAssets + "enemy1.png"), GameMemberTypes.EasyEnemy);
            return newEnemy;
        });
        task.Start();
        return task;
    }

    private void MoveToPauseMenu(Timer reDrawTimer)
    {
        if (!MenuPauseControls.Any())
        {
            var background = new PictureBox
            {
                Image = Image.FromFile(MainForm.PathToAssets + "pauseBlackout.png"),
                Size = MainForm.MainFormSize,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            var buttonResume = new Button
            {
                Image = Image.FromFile(MainForm.PathToAssets + "pauseMenuButton.png"),
                Size = MainForm.MenuButtonSize,
                Text = "Продолжить игру",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Location = new Point(
                    MainForm.MainFormSize.Width / 2 - MainForm.MenuButtonSize.Width / 2,
                    MainForm.MainFormSize.Height / 2 - MainForm.MenuButtonSize.Height - 10)
            };
            buttonResume.Click += (_, _) =>
            {
                MenuPauseControls.ForEach(c => Controls.Remove(c));
                Timer.Start();
            };

            var buttonExit = new Button
            {
                Image = Image.FromFile(MainForm.PathToAssets + "pauseMenuButton.png"),
                Size = MainForm.MenuButtonSize,
                Text = "Выйти из игры",
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Location = new Point(
                    MainForm.MainFormSize.Width / 2 - MainForm.MenuButtonSize.Width / 2,
                    MainForm.MainFormSize.Height / 2 + 10)
            };
            buttonExit.Click += (_, _) => Application.Exit();

            MenuPauseControls.AddRange(new Control[] { background, buttonExit, buttonResume });
            MenuPauseControls.ForEach(c =>
            {
                if (c.GetType() != typeof(Button)) return;
                c.Font = new Font(c.Font.FontFamily, 12);
                c.MouseEnter += (_, _) =>
                    ((Button)c).Image = Image.FromFile(MainForm.PathToAssets + "pauseMenuButton-hovered.png");
                c.MouseLeave += (_, _) =>
                    ((Button)c).Image = Image.FromFile(MainForm.PathToAssets + "pauseMenuButton.png");
            });
        }

        if (reDrawTimer.Enabled)
        {
            reDrawTimer.Stop();
            Controls.AddRange(MenuPauseControls.ToArray());
            MenuPauseControls.ForEach(c => c.BringToFront());
        }
        else
        {
            reDrawTimer.Start();
            MenuPauseControls.ForEach(c => Controls.Remove(c));
        }
    }


    #endregion
    
    public void GameOver(Control.ControlCollection controls)
    {
        IsTimerWasRemoved = true;
        controls.Clear();
        controls.AddRange(new Control[]
        {
            new PictureBox
            {
                Location = new Point(0, 0),
                Size = MainForm.MainFormSize,
                Image = Image.FromFile(MainForm.PathToAssets + "pauseBlackout.png"),
                BackColor = Color.Transparent
            },
            new PictureBox
            {
                Location = new Point(0, 0),
                Size = MainForm.MainFormSize,
                Image = Image.FromFile(MainForm.PathToAssets + "gameOver.png"),
                BackColor = Color.Transparent
            }
        });
    }
}