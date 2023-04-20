namespace SpaceGame;

public class EnemyModel : GameMember, IShooter
{
    public EnemyModel(Point location, Size size, Image sprite) : base(location, size, sprite) {}
    public EnemyModel(PictureBox pictureBox) : base(pictureBox) {}

    public void Shoot(Control.ControlCollection control)
    {
        throw new NotImplementedException();
    }
}