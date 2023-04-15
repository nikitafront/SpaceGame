namespace SpaceGame;

public class PlayerModel : GameMember, IShooter
{
    public PlayerModel(Point location, Image sprite) : base(location, sprite) {}
    public PlayerModel(PictureBox pictureBox) : base(pictureBox) {}

    public void Shoot()
    {
        throw new NotImplementedException();
    }
}