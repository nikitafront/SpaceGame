using SpaceGame.architecture;

namespace SpaceGame;

public class BonusModel : GameObject
{
    public BonusModel(Point location, Size size, Image image) : base(location, size, image) { }
    
    public override void Move(Control.ControlCollection controls)
    {
        Location = Location with { Y = Location.Y + Game.BonusSpeed };
        if (Location.Y > MainForm.GetMainForm.Size.Height)
        {
            controls.Remove(PictureBox);
            return;
        } 
        PictureBox.Location = Location;
    }
}