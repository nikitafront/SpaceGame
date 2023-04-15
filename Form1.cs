namespace SpaceGame;

public sealed partial class Form1 : Form
{
    private readonly string pathToAssets = Path.GetFullPath("..\\..\\..\\assets\\");
    private readonly Size playerSize = new Size(68, 77);
    private Point newPlayerLocation = Point.Empty;

    private PlayerModel player;

    public Form1()
    {
        {
            Text = "SpaceGame";
            Size = new Size(1240, 720);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackgroundImage = Image.FromFile(pathToAssets + "mainBackgroundImages\\1.jpg"); //TODO make background animated 
        }

        var player = new PlayerModel(
            new Point(ClientSize.Width / 2, ClientSize.Height - playerSize.Height),
            playerSize,
            Color.Transparent,
            Image.FromFile(Path.GetFullPath(pathToAssets + "player.png"))
            );

        player.MovingTimer = new TimersStorage(TimersNames.Moving, true, 40, (s, args) =>
            {
                player.Location = new Point(
                    player.Location.X + newPlayerLocation.X,
                    player.Location.Y + newPlayerLocation.Y
                );
                newPlayerLocation = new Point(newPlayerLocation.X * 4 / 10, newPlayerLocation.Y * 4 / 10);
            });
        
        KeyDown += (sender, args) =>
        {
            const int offsetValue = 10;
            newPlayerLocation = Point.Empty;
            switch (args.KeyCode)
            {
                case Keys.Up:
                    newPlayerLocation.Y += player.Location.Y <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Down:
                    newPlayerLocation.Y += player.Location.Y + playerSize.Height * 1.60 >= Size.Height ? 0 : offsetValue;
                    break;
                case Keys.Left:
                    newPlayerLocation.X += player.Location.X <= 0 ? 0 : -offsetValue;
                    break;
                case Keys.Right:
                    newPlayerLocation.X += player.Location.X + playerSize.Width * 1.35 >= Size.Width ? 0 : offsetValue;
                    break;
            }
        };

        Controls.Add(player.PictureBox);
    }
}