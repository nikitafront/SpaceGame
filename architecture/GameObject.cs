namespace SpaceGame;

public abstract class GameObject
{
    public static readonly Size PlayerSize = new (80, 80);
    public static readonly Size EasyEnemySize = new (60, 60);
    public static readonly Size MiddleEnemySize = new (100, 100);
    public static readonly Size HardEnemySize = new (120, 120);
    public static readonly Size BossSize = new (200, 200);
    public static readonly Size BonusSize = new (40, 40);
    public Point Location { get; set; }
    public Size Size { get; set; }
    public Image Image { get; set; }
    public PictureBox PictureBox { get; set; }
    internal readonly List<PictureBox> bullets = new ();

    protected GameObject(
        Point location,
        Size size,
        Image sprite)
    {
        Location = location;
        Size = size;
        Image = sprite;

        PictureBox = new PictureBox
        {
            Location = location,
            Size = size,
            Image = sprite
        };
    }

    protected GameObject(PictureBox pictureBox) { PictureBox = pictureBox; }

    public abstract void Move(Control.ControlCollection controls);

    public bool IsCrossing(PictureBox other)
        => !(other.Left > PictureBox.Left + PictureBox.Width 
             || PictureBox.Left > other.Left + other.Width 
             || other.Top > PictureBox.Top + PictureBox.Height 
             || PictureBox.Top > other.Top + other.Height);
    
    public bool IsCrossing(GameObject other)
        => IsCrossing(other.PictureBox);

    public bool IsCrossing(List<GameObject> members)
        => members.Any(m => IsCrossing(m.PictureBox));
}