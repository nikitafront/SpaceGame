using System.Media;
using Microsoft.VisualBasic;
using Plugin.Maui.Audio;
using SpaceGame.architecture;

namespace SpaceGame;

public abstract class GameObject
{
    public Point Location { get; set; }
    public Size Size { get; }
    public Image Image { get; set; }
    public int HitPoints { get; set; }
    public PictureBox PictureBox { get; }
    public readonly List<PictureBox> bullets = new ();
    private static SoundPlayer _soundPlayer = new ();

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
            Image = sprite,
            BackColor = Color.Black
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

    public static void PlaySound(string soundName)
    {
        new Thread(() =>
        { 
            _soundPlayer = new SoundPlayer(Variables.PathToAssets + $"sounds\\{soundName}");
            _soundPlayer.LoadAsync();
            _soundPlayer.PlaySync();
        }).Start();
    }
}