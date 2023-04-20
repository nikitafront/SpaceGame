namespace SpaceGame;

public abstract class GameMember
{
    public static readonly Size EasyEnemySize = new (60, 60);
    public static readonly Size PlayerSize = new (80, 80);
    public static readonly Size MiddleEnemySize = new (100, 100);
    public static readonly Size HardEnemySize = new (120, 120);
    public static readonly Size BossSize = new (200, 200);
    public Point Location { get => PictureBox.Location; set => PictureBox.Location = value; }
    public PictureBox PictureBox { get; set; }
    internal readonly List<PictureBox> bullets = new ();

    protected GameMember(
        Point location,
        Size size,
        Image sprite)
    {
        PictureBox = new PictureBox()
        {
            Location = location,
            Image = sprite,
            Size = size,
            BackColor = Color.Transparent
        };
    }
    
    protected GameMember(PictureBox pictureBox) { PictureBox = pictureBox; }

    public bool IsCrossing(PictureBox other)
        => other.Left <= PictureBox.Right && other.Left >= PictureBox.Left
             && other.Top >= PictureBox.Top && other.Top <= PictureBox.Bottom;
}