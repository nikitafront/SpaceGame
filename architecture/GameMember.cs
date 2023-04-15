namespace SpaceGame;

public abstract class GameMember
{
    public static readonly Size _size = new Size(68, 77);
    public Point Location { get => PictureBox.Location; set => PictureBox.Location = value; }
    public TimersStorage MovingTimer { get; set; }
    public PictureBox PictureBox { get; set; }

    protected GameMember(
        Point location,
        Image sprite)
    {
        PictureBox = new PictureBox()
        {
            Location = location,
            Image = sprite,
            Size = _size,
            BackColor = Color.Transparent
        };
    }
    
    protected GameMember(PictureBox pictureBox) { PictureBox = pictureBox; }

    public bool IsDead() => false;
    
    public bool IsCrossing(PictureBox other)
        => other.Left <= PictureBox.Right && other.Left >= PictureBox.Left
             && other.Top >= PictureBox.Top && other.Top <= PictureBox.Bottom;
}