using SpaceGame.architecture;

namespace SpaceGame;

public class BonusModel : GameObject
{
    public BonusType BonusType { get; }

    public BonusModel(Point location, Size size, Image image, BonusType bonusType) : base(location, size, image)
    {
        BonusType = bonusType;
        PictureBox.BackColor = Color.Transparent;
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