using System.ComponentModel;
using System.Media;
using SpaceGame.architecture;
using static SpaceGame.architecture.Variables;

namespace SpaceGame;

public partial class MainForm : Form
{
    public static Size MainFormSize { get; } = new Size(1240, 720);
    public static Size MenuButtonSize { get; } = new Size(180, 60);
    public static Form GetMainForm { get; set; }
    private static Label ScoreText { get; set; }
    private readonly Random rnd = new();
    private readonly BackgroundWorker worker = new ();

    public MainForm()
    {
        var sp = new SoundPlayer(PathToAssets + "\\sounds\\main_theme.wav");
        if (GetMainForm == null)
            sp.Play();

        GetMainForm = this;

        #region WindowState

        {
            Text = "SpaceGame";
            Size = MainFormSize;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackgroundImage =
                Image.FromFile(PathToAssets + "mainBackgroundImages\\1.jpg");
        }

        #endregion

        var manualBonuses = new PictureBox
        {
            Location = new(0, 0),
            Size = MainFormSize,
            Image = Image.FromFile(PathToAssets + "manualBonuses.png")
        };

        var manualArrows = new PictureBox
        {
            Location = new(0, 0),
            Size = MainFormSize,
            Image = Image.FromFile(PathToAssets + "manualArrows.png")
        };

        var manualMean = new PictureBox
        {
            Location = new(0, 0),
            Size = MainFormSize,

            Image = Image.FromFile(PathToAssets + "manualMean.png")
        };

        var manualPreview = new PictureBox
        {
            Location = new(0, 0),
            Size = MainFormSize,
            Image = Image.FromFile(PathToAssets + "manualPreview.png"),
            BackColor = Color.FromArgb(255, 0, 0, 0)
        };

        Controls.Add(manualPreview);
        Controls.Add(manualMean);
        Controls.Add(manualArrows);
        Controls.Add(manualBonuses);

        foreach (Control control in Controls)
            control.Click += (_, _) => Controls.Remove(control);

        Controls[^1].Click += (_, _) =>
        {
            sp.Stop();
            StartGame(this);
        };
    }

    private void StartGame(Form form)
    {
        var game = new Game
        {
            Controls = form.Controls,
            Player = new PlayerModel(
                new Point(form.ClientSize.Width / 2, form.ClientSize.Height - Variables.PlayerSize.Height),
                Variables.PlayerSize,
                Image.FromFile(Path.GetFullPath(PathToAssets + "player.png"))
            )
        };

        form.KeyDown += (_, args) =>
        {
            const int offsetValue = 50;
            switch (args.KeyCode)
            {
                case Keys.Up:
                    game.Player.AppendTempLocation(new(0, -offsetValue));
                    break;
                case Keys.Down:
                    game.Player.AppendTempLocation(new(0, offsetValue));
                    break;
                case Keys.Left:
                    game.Player.AppendTempLocation(new(-offsetValue, 0));
                    break;
                case Keys.Right:
                    game.Player.AppendTempLocation(new(offsetValue, 0));
                    break;
            }
        };

        ScoreText = new Label
        {
            Name = "ScoreText",
            Size = new Size(500, 80),
            Location = new Point(0, form.Size.Height - 100),
            Text = "Current score:\nBest result:",
            Font = new Font(new FontFamily("ArcadeClassic"), 22),
            BackColor = Color.Transparent,
            ForeColor = Color.DarkRed
        };

        form.Controls.Add(game.Player.PictureBox);
        form.Controls.Add(ScoreText);
    }
}