using SpaceGame.architecture;

namespace SpaceGame;

public class EnemyModel : GameMember
{
    public EnemyModel(Point location, Size size, Image sprite) : base(location, size, sprite) {}
    public EnemyModel(PictureBox pictureBox) : base(pictureBox) {}

    public override void Shoot(Control.ControlCollection control)
    {
        throw new NotImplementedException();
    }

    public override void FlyBullets(Control.ControlCollection controls)
    {
    }

    public override void Move()
    {
        Location = Location with { Y = Location.Y + Game.EasyEnemySpeed };
        PictureBox.Location = Location;
    }
}