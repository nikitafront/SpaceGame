namespace SpaceGame;

public class PlayerModel : GameMember, IShooter
{
    public PlayerModel(Point location, Size size, Image sprite) : base(location, size, sprite) {}
    public PlayerModel(PictureBox pictureBox) : base(pictureBox) {}

    public void Shoot(Control.ControlCollection controls)
    {
        var id = new Random().NextInt64();
        new TimersStorage(
            id,
            true,
            TimerIntervals.Shoot,
            (o, e) =>
            {
                var playerHorizontalCenter = PictureBox.Location.X + PictureBox.Size.Width / 2;
                var newBullet = new PictureBox
                {
                    Location = PictureBox.Location with { X = playerHorizontalCenter },
                    Size = new Size(4, 8),
                    Image = Image.FromFile(Path.GetFullPath(MainForm.PathToAssets + "bullet.png"))
                };
                
                bullets.Add(newBullet);
                controls.Add(newBullet);
            }
        );
    }
}