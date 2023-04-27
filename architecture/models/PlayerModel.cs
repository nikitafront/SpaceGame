using SpaceGame.architecture;

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

    public override void FlyBullets(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies)
    {
        var toRemove = new List<PictureBox>();
        
        foreach (var b in bullets)
        {
            b.Location = b.Location with { Y = b.Location.Y - Game.BulletSpeed  };

            if (b.Location.Y < -10)
            {
                toRemove.Add(b);
                continue;
            }

            var enemiesToKill = allEnemies
                .SelectMany(i => i)
                .ToList()
                .Where(e => e.IsCrossing(b));

            if (!enemiesToKill.Any()) continue;
            bullets.Remove(b);
            controls.Remove(b);
            
            foreach (var enemy in enemiesToKill)
            {
                controls.Remove(enemy.PictureBox);
                enemy.bullets.ForEach(controls.Remove);
                allEnemies.Any(l => l.Remove(enemy));
                return;
            }
        }
        
        foreach (var b in toRemove)
        {
            bullets.Remove(b);
            controls.Remove(b);
        }
    }

    public override void Move(Control.ControlCollection controls = null)
    {
        Location = new Point(Location.X + TempLocation.X, Location.Y + TempLocation.Y);
        TempLocation = Point.Empty;
        PictureBox.Location = Location;
    }

    public void AppendTempLocation(Point temp)
    {
        if (Location.Y + temp.Y <= 0 || Location.Y + PlayerSize.Height + temp.Y >= MainForm.GetMainForm.Size.Height
           || Location.X + temp.X <= 0 || Location.X + PlayerSize.Width + temp.X >= MainForm.GetMainForm.Size.Width) return;
        TempLocation.Offset(temp);
    }
}