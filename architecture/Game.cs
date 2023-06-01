using System.ComponentModel;
using System.Globalization;
using System.Media;
using Timer = System.Windows.Forms.Timer;
using static SpaceGame.architecture.Variables;

namespace SpaceGame.architecture;

public class Game
{
    public static Game GetCurrentGame { get; private set; }
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
    public static long Score { get; set; }
    
    private const long MaximumCommonMultipleEnemySpawnInterval = 2_000;

    public Game()
    {
        GetCurrentGame = this;
        if (Variables.Timer == null)
        {
            Variables.Timer = new Timer { Enabled = true, Interval = 100 };
            Variables.Timer.Tick += (_, _) =>
            {
                PassedMilliseconds += 100;
                ReDraw();
            };
        }

        AllEnemies = new() { EasyEnemies, MiddleEnemies, HardEnemies, Bosses };
        MainForm.GetMainForm.KeyDown += (_, args) =>
        {
            if (args.KeyCode == Keys.Escape) MoveToPauseMenu(Variables.Timer);
        };
    }

    private void ReDraw()
    {
        if (PassedMilliseconds == 100)
            for (int i = 0; i < Player.LifeCount; i++)
                AddLife(i);

        if (PassedMilliseconds % MovingInterval == 0)
        {
            ChangeScoreText();

            if (Player.Move(Controls, Bonuses)) return;

            if (PlayerLifes.Count > Player.LifeCount)
            {
                Controls.Remove(PlayerLifes.Last());
                PlayerLifes.Remove(PlayerLifes.Last());
            }

            if (PlayerLifes.Count < Player.LifeCount)
            {
                AddLife(Player.LifeCount - 1);
            }
        }

        for (int i = 0; i < AllEnemies.Count; i++)
        {
            if (PassedMilliseconds % EnemySpeeds[i] == 0)
                AllEnemies[i].ForEach(e => e.Move(Controls));
            
            if (PassedMilliseconds % EnemyShootIntervals[i] == 0)
                AllEnemies
                    .SelectMany(x => x)
                    .Where(x => (int) x.EnemyType == i)
                    .ToList()
                    .ForEach(e => e.Shoot(Controls));
        }

        if (PassedMilliseconds % PlayerShootInterval == 0) 
            Player.Shoot(Controls);

        if (PassedMilliseconds % BulletSpeed == 0)
        {
            Player.FlyBullets(Controls, AllEnemies, Bonuses);
            AllEnemies.ForEach(l => l.ForEach(e => e.FlyBullets(Controls)));
        }

        if (PassedMilliseconds % BonusSpeed == 0) 
            Bonuses.ForEach(b => b.Move(Controls));

        if (PassedMilliseconds % MaximumCommonMultipleEnemySpawnInterval == 0)
            SpawnEnemy(PassedMilliseconds);
    }

    private void ChangeScoreText()
    {
        var bestScore = File.ReadAllText(PathToAssets + "bestPlayerResult.txt");
        var template = $"Current score: {Score}\nBest score: {bestScore}";
        Controls.Find("ScoreText", true)[0].Text = template;
    }

    private Task<EnemyModel> CreateEnemy(Size enemySize, GameMemberTypes enemyType)
    {
        var task = new Task<EnemyModel>(() =>
        {
            var newEnemyLocation =
                new Point(
                    new Random().Next(0, new MainForm().Size.Width - enemySize.Width),
                    -enemySize.Height
                );
            
            var newEnemy =
                new EnemyModel(
                    newEnemyLocation,
                    enemySize,
                    Image.FromFile(PathToAssets + $"enemy{(int)enemyType}.png"),
                    enemyType
                );
            
            return newEnemy;
        });
        task.Start();
        return task;
    }

    private async void SpawnEnemy(long timerInterval)
    {
        var newEnemies = new List<EnemyModel>();
        
        for (int i = 0; i < AllEnemies.Count; i++)
            if (timerInterval % EnemySpawnIntervals[i] == 0)
            {
                newEnemies.Add(await CreateEnemy(EnemySizes[i], Enum.GetValues<GameMemberTypes>()[i]));
                AllEnemies[i].Add(newEnemies.Last());
            }

        if (newEnemies.Count != 0) 
            newEnemies.ForEach(e => Controls.Add(e.PictureBox));
    }

    private void MoveToPauseMenu(Timer reDrawTimer)
    {
        if (!MenuPauseControls.Any())
        {
            var background = new PictureBox
            {
                Image = Image.FromFile(PathToAssets + "pauseBlackout.png"),
                Size = MainForm.MainFormSize,
                Location = new Point(0, 0),
                BackColor = Color.Transparent
            };

            var buttonResume = new Button
            {
                Image = Image.FromFile(PathToAssets + "pauseMenuButton.png"),
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
                Variables.Timer.Start();
            };

            var buttonExit = new Button
            {
                Image = Image.FromFile(PathToAssets + "pauseMenuButton.png"),
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
                    ((Button)c).Image = Image.FromFile(PathToAssets + "pauseMenuButton-hovered.png");
                c.MouseLeave += (_, _) =>
                    ((Button)c).Image = Image.FromFile(PathToAssets + "pauseMenuButton.png");
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

    public bool GameOver(Control.ControlCollection controls)
    {
        var bestResult = long.Parse(File.ReadAllText(PathToAssets + "bestPlayerResult.txt"));
        File.WriteAllText(
            PathToAssets + "bestPlayerResult.txt",
            Math.Max(Score, bestResult).ToString()
        );

        Variables.Timer?.Stop();

        var gameOverPicture = new PictureBox
        {
            Location = new Point(0, 0),
            Size = MainForm.MainFormSize,
            Image = Image.FromFile(PathToAssets + "gameOver.png"),
            BackColor = Color.Transparent
        };
        gameOverPicture.Click += (sender, args) => Application.Exit();

        controls.Clear();
        controls.Add(gameOverPicture);


        return true;
    }

    private void AddLife(int position)
    {
        var life =
            new PictureBox
            {
                Location = new Point(10 + 45 * (position % 10), 35 * (position / 10)),
                Size = HeartSize,
                BackColor = Color.Transparent,
                Image = Image.FromFile(PathToAssets + "life.png")
            };

        Controls.Add(life);
        PlayerLifes.Add(life);
    }
}