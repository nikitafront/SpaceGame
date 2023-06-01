using System.ComponentModel;
using System.Media;
using SpaceGame.architecture;
using SpaceGame.architecture.interfaces;
using static SpaceGame.architecture.Variables;

namespace SpaceGame;

public class PlayerModel : GameObject, IGameMember
{
    public Point TempLocation = Point.Empty;
    public int LifeCount = 3;
    private BulletsDamage _bulletDamage { get; set; }
    private int _bulletIndex { get; set; }
    private bool flag = false;

    public PlayerModel(Point location, Size size, Image sprite) : base(location, size, sprite)
    {
        HitPoints = (int)GameMemberHitPoints.Player;
        _bulletDamage = BulletsDamage.EasyBullet;
        PictureBox.BackColor = Color.Transparent;
    }

    public void Shoot(Control.ControlCollection controls)
    {
        var playerHorizontalCenter = PictureBox.Location.X + PictureBox.Size.Width / 2;
        var newBullet = new PictureBox
        {
            Location = PictureBox.Location with { X = playerHorizontalCenter },
            Size = BulletSizes[_bulletIndex],
            Image = Image.FromFile(Path.GetFullPath(PathToAssets + $"bullet{_bulletIndex}.png")),
            BackColor = Color.Black,
            Tag = _bulletIndex
        };

        bullets.Add(newBullet);
        controls.Add(newBullet);
    }

    public void FlyBullets(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies,
        List<BonusModel> bonuses)
    {
        var toRemove = new List<PictureBox>();

        for (int i = 0; i < bullets.Count; i++)
        {
            var currenBullet = bullets[i];

            currenBullet.Location = currenBullet.Location with { Y = currenBullet.Location.Y - BulletSpeed };

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

            enemiesToKill.ForEach(e => e.Die(controls, allEnemies, bonuses, _bulletDamage));
        }

        foreach (var b in toRemove)
        {
            bullets.Remove(b);
            controls.Remove(b);
        }
    }

    public void Die(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies = null,
        List<BonusModel> bonuses = null,
        BulletsDamage damage = 0)
    {
        if (HitPoints != 0)
            HitPoints--;
        else
        {
            LifeCount--;
            HitPoints = (int)GameMemberHitPoints.Player;

            Location = new Point(
                MainForm.GetMainForm.ClientSize.Width / 2,
                MainForm.GetMainForm.ClientSize.Height - PlayerSize.Height
            );
            
            var worker = new BackgroundWorker();
            worker.DoWork += (_, _) => new SoundPlayer(PathToAssets + "\\sounds\\enemy_die_sound.wav").Play();
            worker.RunWorkerAsync();
        }
    }


    public override void Move(Control.ControlCollection controls)
    {
        Location = new Point(Location.X + TempLocation.X, Location.Y + TempLocation.Y);
        TempLocation = Point.Empty;
        PictureBox.Location = Location;
    }

    public bool Move(Control.ControlCollection controls, List<BonusModel> bonuses)
    {
        if (LifeCount <= 0) return Game.GetCurrentGame.GameOver(controls);

        Game.AllEnemies
            .SelectMany(l => l)
            .Where(IsCrossing)
            .ToList()
            .ForEach(e =>
            {
                e.Die(controls, Game.AllEnemies, Game.Bonuses, _bulletDamage); //for assured enemy kill 
                Die(controls, Game.AllEnemies, Game.Bonuses, e.BulletType);
            });

        Move(controls);

        var bulletDamageValues = Enum.GetValues<BulletsDamage>();

        for (int i = 0; i < bonuses.Count; i++)
        {
            if (!IsCrossing(bonuses[i])) continue;
            
            var worker = new BackgroundWorker();
            worker.DoWork += (_, _) => new SoundPlayer(PathToAssets + "\\sounds\\bonus_sound.wav").Play();
            worker.RunWorkerAsync();

            if (bonuses[i].BonusType == BonusType.Heart) LifeCount++;
            if (bonuses[i].BonusType == BonusType.Bullet)
            {
                var nextBulletIndex = Math.Min(
                    bulletDamageValues.Length - 1,
                    Array.IndexOf(bulletDamageValues, _bulletDamage) + 1
                );

                _bulletDamage = bulletDamageValues[nextBulletIndex];
                _bulletIndex = nextBulletIndex;

                if (ClearBulletBonusTimes.Count < 3)
                    ClearBulletBonusTimes.Enqueue(PassedMilliseconds + ClearBulletBonusInterval);
            }

            controls.Remove(bonuses[i].PictureBox);
            bonuses.Remove(bonuses[i]);

            Game.Score += 50;
            
            PlaySound("bonus_sound.wav");
        }

        if (Game
            .AllEnemies
            .SelectMany(e => e)
            .SelectMany(e => e.bullets)
            .Any(IsCrossing))
        {
            controls.Remove(Game.PlayerLifes.Last());
            Game.PlayerLifes.Remove(Game.PlayerLifes.Last());
            Die(controls);
        }

        if (ClearBulletBonusTimes.Count != 0
            && ClearBulletBonusTimes.Peek() <= PassedMilliseconds)
        {
            var previousBulletIndex = Math.Max(0, Array.IndexOf(bulletDamageValues, _bulletDamage) - 1);

            ClearBulletBonusTimes.Dequeue();
            _bulletDamage = bulletDamageValues[previousBulletIndex];
            _bulletIndex = previousBulletIndex;
        }

        // TODO add sound for player die
        return false;
    }

    public void AppendTempLocation(Point temp)
    {
        if (Location.Y + temp.Y <= 0 || Location.Y + PlayerSize.Height + temp.Y >=
                                     MainForm.GetMainForm.Size.Height
                                     || Location.X + temp.X <= 0 || Location.X + PlayerSize.Width + temp.X >=
                                     MainForm.GetMainForm.Size.Width) return;
        TempLocation.Offset(temp);
    }
}