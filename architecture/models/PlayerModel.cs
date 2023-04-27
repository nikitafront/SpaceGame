namespace SpaceGame;

public class PlayerModel : GameMember{

    public PlayerModel(Point location, Size size, Image sprite) : base(location, size, sprite) { }

    public override void Shoot(Control.ControlCollection controls)
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

    public override void FlyBullets(Control.ControlCollection controls)
    {
        var toRemove = new List<PictureBox>();
        
        foreach (var b in bullets)
        {
            b.Location = b.Location with { Y = b.Location.Y - 20 };
            if (b.Location.Y < -10)
                toRemove.Add(b);
        }
        
        foreach (var b in toRemove)
        {
            bullets.Remove(b);
            controls.Remove(b);
        }
    }

    public override void Move()
    {
        Location = new Point(Location.X + TempLocation.X, Location.Y + TempLocation.Y);
        TempLocation = Point.Empty;
        PictureBox.Location = Location;
    }
}