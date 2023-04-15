namespace SpaceGame;

public class EnemyModel : GameMember, IShooter
{
    public EnemyModel(Point location, Image sprite) : base(location, sprite) {}
    public EnemyModel(PictureBox pictureBox) : base(pictureBox) {}

    public void Shoot()
    {
        throw new NotImplementedException();
    }
}