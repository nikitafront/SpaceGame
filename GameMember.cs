namespace SpaceGame;

public abstract class GameMember
{
    public Point Location { get => PictureBox.Location; set => PictureBox.Location = value; }
    public TimersStorage MovingTimer { get; set; }
    public PictureBox PictureBox { get; set; }

    protected GameMember(
        Point location,
        Size size, 
        Color backgroundColor, 
        Image sprite)
    {
        PictureBox = new PictureBox()
        {
            Location = location,
            Image = sprite,
            Size = size,
            BackColor = backgroundColor
        };
    }

    public bool IsDead() => false;
}