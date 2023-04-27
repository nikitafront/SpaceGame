using SpaceGame.architecture;

namespace SpaceGame;

public class EnemyModel : GameMember
{
    public GameMemberTypes Type { get; }

    public EnemyModel(Point location, Size size, Image sprite, GameMemberTypes type) : base(location, size, sprite) {Type = type;}
    public EnemyModel(PictureBox pictureBox) : base(pictureBox) {}

    public override void Shoot(Control.ControlCollection controls)
    {
        var enemyHorizontalCenter = PictureBox.Location.X + PictureBox.Size.Width / 2;
        var newBullet = new PictureBox
        {
            Location = PictureBox.Location with { X = enemyHorizontalCenter },
            Size = new Size(4, 8),
            Image = Image.FromFile(Path.GetFullPath(MainForm.PathToAssets + "bullet.png"))
        };

        bullets.Add(newBullet);
        controls.Add(newBullet);
    }

    public override void FlyBullets(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies = null)
    {
        var toRemove = new List<PictureBox>();
        
        foreach (var b in bullets)
        {
            b.Location = b.Location with { Y = b.Location.Y + Game.BulletSpeed  };
            if (b.Location.Y > MainForm.GetMainForm.Size.Height + 10) toRemove.Add(b);
        }
        
        foreach (var b in toRemove)
        {
            bullets.Remove(b);
            controls.Remove(b);
        }
    }

    public override void Move(Control.ControlCollection controls)
    {
        Location = Location with { Y = Location.Y + Game.EasyEnemySpeed };
        if (Location.Y > MainForm.GetMainForm.Size.Height)
        {
            controls.Remove(PictureBox);
            return;
        } 
        PictureBox.Location = Location;
    }
}