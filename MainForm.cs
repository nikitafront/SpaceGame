using SpaceGame.architecture;

namespace SpaceGame;

public partial class MainForm : Form
{
    public static readonly string PathToAssets = Path.GetFullPath("..\\..\\..\\assets\\");
    
    private readonly Random rnd = new();

    public MainForm()
    {
        var game = new Game
        {
            Controls = Controls
        };

        #region WindowState

        {
            Text = "SpaceGame";
            Size = new Size(1240, 720);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackgroundImage =
                Image.FromFile(PathToAssets + "mainBackgroundImages\\1.jpg");
        }

        #endregion

        #region PlayerState

        game.Player = new PlayerModel(
            new Point(ClientSize.Width / 2, ClientSize.Height - GameMember.PlayerSize.Height),
            GameMember.PlayerSize,
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
                    game.Player.TempLocation.Y = game.Player.Location.Y <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Down:
                    game.Player.TempLocation.Y = game.Player.Location.Y + GameMember.PlayerSize.Height >= Size.Height
                        ? 0
                        : offsetValue;
                    break;
                case Keys.Left:
                    game.Player.TempLocation.X = game.Player.Location.X <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Right:
                    game.Player.TempLocation.X = game.Player.Location.X + GameMember.PlayerSize.Width >= Size.Width
                        ? 0
                        : offsetValue;
                    break;
            }
        };

        #endregion
    }
}