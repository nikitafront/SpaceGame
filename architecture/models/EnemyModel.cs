using System.ComponentModel;
using System.Media;
using SpaceGame.architecture;
using SpaceGame.architecture.interfaces;
using static SpaceGame.architecture.Variables;

namespace SpaceGame;

public class EnemyModel : GameObject, IGameMember
{
    public GameMemberTypes EnemyType { get; }
    public BulletsDamage BulletType { get; }

    public EnemyModel(Point location, Size size, Image sprite, GameMemberTypes enemyType) : base(location, size, sprite)
    {
        EnemyType = enemyType;
        switch (enemyType)
        {
            case GameMemberTypes.EasyEnemy:
                HitPoints = (int)GameMemberHitPoints.EasyEnemy;
                BulletType = BulletsDamage.EasyBullet;
                break;
            case GameMemberTypes.MiddleEnemy:
                HitPoints = (int)GameMemberHitPoints.MiddleEnemy;
                BulletType = BulletsDamage.MiddleBullet;
                break;
            case GameMemberTypes.HardEnemy:
                HitPoints = (int)GameMemberHitPoints.HardEnemy;
                BulletType = BulletsDamage.HardBullet;
                break;
            case GameMemberTypes.Boss:
                HitPoints = (int)GameMemberHitPoints.Boss;
                BulletType = BulletsDamage.AtomicBomb;
                break;
            default: throw new InvalidEnumArgumentException(nameof(enemyType), (int)enemyType, typeof(GameMemberTypes));
        }
    }
    
    public EnemyModel(PictureBox pictureBox) : base(pictureBox)
    {
    }

    public void Shoot(Control.ControlCollection controls)
    {
        var enemyHorizontalCenter = PictureBox.Location.X + PictureBox.Size.Width / 2;
        var newBullet = new PictureBox
        {
            Location = PictureBox.Location with { X = enemyHorizontalCenter },
            Size = BulletSizes[(int)EnemyType],
            Image = Image.FromFile(Path.GetFullPath(PathToAssets + $"bullet{(int)EnemyType}.png")),
            BackColor = Color.Black
        };
        if (EnemyType == GameMemberTypes.Boss) newBullet.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);

        bullets.Add(newBullet);
        controls.Add(newBullet);
    }

    public void FlyBullets(Control.ControlCollection controls, List<List<EnemyModel>> allEnemies = null,
        List<BonusModel> bonuses = null)
    {
        var toRemove = new List<PictureBox>();

        foreach (var b in bullets)
        {
            b.Location = b.Location with { Y = b.Location.Y + BulletSpeed };
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
        Location = Location with { Y = Location.Y + EnemySpeeds[(int)EnemyType] };
        if (Location.Y > MainForm.GetMainForm.Size.Height)
        {
            controls.Remove(PictureBox);
            return;
        }

        PictureBox.Location = Location;
    }

    public void Die(Control.ControlCollection controls,
        List<List<EnemyModel>> allEnemies,
        List<BonusModel> bonuses = null,
        BulletsDamage damage = 0)
    {
        if (HitPoints > (int) damage) HitPoints -= (int) damage;
        else
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (_, _) => new SoundPlayer(PathToAssets + "\\sounds\\enemy_die_sound.wav").Play();
            worker.RunWorkerAsync();
            
            controls.Remove(PictureBox);
            bullets.ForEach(controls.Remove);
            allEnemies.Any(i => i.Remove(this));

            switch (EnemyType)
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

            var rndInt = new Random().Next(0, 100);

            if (rndInt <= MaximalBonusDropChance)
            {
                MainForm.GetMainForm.Text = rndInt.ToString();
                var newBonus = new BonusModel(Location, BonusSize, rndInt);

                bonuses.Add(newBonus);
                controls.Add(newBonus.PictureBox);
            }

            PlaySound("enemy_die_sound.wav");
        }
    }
}