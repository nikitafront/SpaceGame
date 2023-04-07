namespace SpaceGame;

public sealed partial class Form1 : Form
{
    private event Action<object?, KeyEventArgs, PictureBox, int> playerMoving;
    private readonly string pathToAssets = Path.GetFullPath("..\\..\\..\\assets\\");
    private readonly Size playerSize = new Size(68, 77);
    private Point newPlayerLocation = Point.Empty;

    public Form1()
    {
        playerMoving += (sender, args, picture, offsetValue) =>
        {
            newPlayerLocation = Point.Empty;
            switch (args.KeyCode)
            {
                case Keys.Up:
                    newPlayerLocation.Y = picture.Location.Y <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Down:
                    newPlayerLocation.Y =
                        picture.Location.Y + playerSize.Height * 1.60 >= Size.Height ? 0 : offsetValue;
                    break;
                case Keys.Left:
                    newPlayerLocation.X = picture.Location.X <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Right:
                    newPlayerLocation.X = picture.Location.X + playerSize.Width * 1.35 >= Size.Width ? 0 : offsetValue;
                    break;
            }
        };

        {
            Text = "SpaceGame";
            Size = new Size(1240, 720);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackgroundImage = Image.FromFile(pathToAssets + "mainBackgroundImages\\1.jpg"); //TODO make background animated 
        }

        var player = new PictureBox
        {
            Size = playerSize,
            Location = new Point(ClientSize.Width / 2, ClientSize.Height - playerSize.Height),
            BackColor = Color.Transparent,
            BackgroundImage = Image.FromFile(Path.GetFullPath(pathToAssets + "player.png"))
        };

        var timer = new System.Windows.Forms.Timer { Enabled = true, Interval = 20 };

        timer.Tick += (sender, args) =>
        {
            const int smoothnessCoefficient = 4 / 10;
            
            player.Location = new Point(
                player.Location.X + newPlayerLocation.X,
                player.Location.Y + newPlayerLocation.Y
            );
            
            newPlayerLocation = new Point(
                newPlayerLocation.X * smoothnessCoefficient,
                newPlayerLocation.Y * smoothnessCoefficient
            );
        };

        KeyDown += (sender, args) => playerMoving(sender, args, player, 10);

        Controls.Add(player);
    }
}