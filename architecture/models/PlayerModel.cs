﻿using SpaceGame.architecture;
using SpaceGame.architecture.interfaces;

namespace SpaceGame;

public class PlayerModel : GameObject, IGameMember
{
    public Point TempLocation = Point.Empty;
    
    public PlayerModel(Point location, Size size, Image sprite) : base(location, size, sprite)
    {
    }

    public void Shoot(Control.ControlCollection controls)
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

    public void FlyBullets(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies, List<BonusModel> bonuses)
    {
        var toRemove = new List<PictureBox>();

        for (int i = 0; i < bullets.Count; i++)
        {
            var currenBullet = bullets[i];
            
            currenBullet.Location = currenBullet.Location with { Y = currenBullet.Location.Y - Game.BulletSpeed };

            if (currenBullet.Location.Y < -10)
            {
                toRemove.Add(currenBullet);
                continue;
            }

            var enemiesToKill = allEnemies
                .SelectMany(i => i)
                .Where(e => e.IsCrossing(currenBullet))
                .ToList();

            if (!enemiesToKill.Any()) continue;
            bullets.Remove(currenBullet);
            controls.Remove(currenBullet);

            enemiesToKill.ForEach(e => e.Die(controls, allEnemies, bonuses));
        }

        foreach (var b in toRemove)
        {
            bullets.Remove(b);
            controls.Remove(b);
        }
    }

    public void Die(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies, List<BonusModel> bonuses = null) { throw new NotImplementedException(); }

    public override void Move(Control.ControlCollection controls = null)
    {
        Location = new Point(Location.X + TempLocation.X, Location.Y + TempLocation.Y);
        TempLocation = Point.Empty;
        PictureBox.Location = Location;
    }

    public void AppendTempLocation(Point temp)
    {
        if (Location.Y + temp.Y <= 0 || Location.Y + PlayerSize.Height + temp.Y >= MainForm.GetMainForm.Size.Height
                                     || Location.X + temp.X <= 0 || Location.X + PlayerSize.Width + temp.X >=
                                     MainForm.GetMainForm.Size.Width) return;
        TempLocation.Offset(temp);
    }
}