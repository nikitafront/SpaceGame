using SpaceGame.architecture;

namespace SpaceGame;

public class BonusModel : GameObject
{
    public BonusType BonusType { get; }

    public BonusModel(Point location, Size size, int rndInt) : base(location, size, null)
    {
        PictureBox.BackColor = Color.Transparent;
        
        const int heartDropChance = 1;
        const int bulletDropChance = 45;

        if (rndInt <= heartDropChance)
            BonusType = BonusType.Heart;
        else if (rndInt <= bulletDropChance)
            BonusType = BonusType.Bullet;
        
        Image = Image.FromFile(Variables.PathToAssets + $"bonus{(BonusType == BonusType.Bullet ? "Bullet" : "Heart")}.png");
        PictureBox.Image = Image;

    }
    
    public override void Move(Control.ControlCollection controls)
    {
        Location = Location with { Y = Location.Y + Variables.BonusSpeed };
        if (Location.Y > MainForm.GetMainForm.Size.Height)
        {
            controls.Remove(PictureBox);
            return;
        } 
        PictureBox.Location = Location;
    }
}