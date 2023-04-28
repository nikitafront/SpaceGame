using SpaceGame.architecture;

namespace SpaceGame;

public partial class MainForm : Form
{
    public static readonly string PathToAssets = Path.GetFullPath("..\\..\\..\\assets\\");
    public static Size MainFormSize { get; } = new Size(1240, 720);
    public static Size MenuButtonSize { get; } = new Size(180, 60);
    public static Form GetMainForm { get; set; } 
    private readonly Random rnd = new();

    public MainForm()
    {
        GetMainForm = this;
        
        var game = new Game
        {
            Controls = Controls
        };

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

        #region PlayerState

        game.Player = new PlayerModel(
            new Point(ClientSize.Width / 2, ClientSize.Height - GameObject.PlayerSize.Height),
            GameObject.PlayerSize,
            Image.FromFile(Path.GetFullPath(PathToAssets + "player.png"))
        );
        

        Controls.Add(game.Player.PictureBox);

        #endregion

        #region PlayerMovingState

        KeyDown += (sender, args) =>
        {
            const int offsetValue = 50;
            switch (args.KeyCode)
            {
                case Keys.Up:
                    game.Player.AppendTempLocation(new (0, -offsetValue));
                    break;
                case Keys.Down:
                    game.Player.AppendTempLocation(new (0, offsetValue));
                    break;
                case Keys.Left:
                    game.Player.AppendTempLocation(new (-offsetValue, 0));
                    break;
                case Keys.Right:
                    game.Player.AppendTempLocation(new (offsetValue, 0));
                    break;
            }
        };

        #endregion
    }
}