namespace SpaceGame;

public class PlayerModel : GameMember, IShooter
{
    public PlayerModel(
        Point location,
        Size size,
        Color backgroundColor, 
        Image sprite) : base(location, size, backgroundColor, sprite) {}

    public void Shoot()
    {
        throw new NotImplementedException();
    }
}