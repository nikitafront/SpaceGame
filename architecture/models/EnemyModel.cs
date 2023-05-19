using System.ComponentModel;
using SpaceGame.architecture;
using SpaceGame.architecture.interfaces;

namespace SpaceGame;

public class EnemyModel : GameObject, IGameMember
{
    public GameMemberTypes Type { get; }

    public EnemyModel(Point location, Size size, Image sprite, GameMemberTypes type) : base(location, size, sprite)
    {
        Type = type;
        switch (type)
        {
            case GameMemberTypes.EasyEnemy: 
                HitPoints = (int) GameMemberHitPoints.EasyEnemy;
                BulletType = BulletTypes.EasyBullet;
                break;
            case GameMemberTypes.MiddleEnemy: 
                HitPoints = (int) GameMemberHitPoints.MiddleEnemy;
                BulletType = BulletTypes.MiddleBullet;
                break;
            case GameMemberTypes.HardEnemy: 
                HitPoints = (int) GameMemberHitPoints.HardEnemy;
                BulletType = BulletTypes.HardBullet;
                break;
            case GameMemberTypes.Boss: 
                HitPoints = (int) GameMemberHitPoints.Boss;
                BulletType = BulletTypes.HardBullet;
                break;
            default: throw new InvalidEnumArgumentException(nameof(type), (int) type, typeof(GameMemberTypes));
        }
    }
    public EnemyModel(PictureBox pictureBox) : base(pictureBox) {}

    public void Shoot(Control.ControlCollection controls)
    {
        var enemyHorizontalCenter = PictureBox.Location.X + PictureBox.Size.Width / 2;
        var newBullet = new PictureBox
        {
            Location = PictureBox.Location with { X = enemyHorizontalCenter },
            Size = new Size(4, 8),
            Image = Image.FromFile(Path.GetFullPath(MainForm.PathToAssets + "easyBullet.png"))
        };

        bullets.Add(newBullet);
        controls.Add(newBullet);
    }
    public void FlyBullets(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies = null, List<BonusModel> bonuses = null)
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
        Location = Location with { Y = Location.Y + (int) Game.EasyEnemySpeed };
        if (Location.Y > MainForm.GetMainForm.Size.Height)
        {
            controls.Remove(PictureBox);
            return;
        } 
        PictureBox.Location = Location;
    }

    public void Die(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies, List<BonusModel> bonuses) {
        if (HitPoints >= 2) HitPoints--;
        else
        {
            controls.Remove(PictureBox);
            bullets.ForEach(controls.Remove);

            switch (Type)
            {
                case GameMemberTypes.EasyEnemy:
                    Game.Score += 100;
                    break;
                case GameMemberTypes.MiddleEnemy:
                    Game.Score += 300;
                    break;
                case GameMemberTypes.HardEnemy:
                    Game.Score += 500;
                    break;
                case GameMemberTypes.Boss:
                    Game.Score += 1000;
                    break;
            }

            allEnemies.Any(l => l.Remove(this));

            var rndInt = new Random().Next(0, 100);
            const int dropChance = 100;
            if (rndInt <= dropChance)
            {
                var bonus = new BonusModel(Location, BonusSize, Image.FromFile(MainForm.PathToAssets + "bonus.png"));
                controls.Add(bonus.PictureBox);
                bonuses.Add(bonus);
            }
        }
    }
}