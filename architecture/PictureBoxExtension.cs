namespace SpaceGame.architecture;

public static class PictureBoxExtension
{
    public static bool IsCrossingWith(this PictureBox first, PictureBox second) 
        => !(second.Left > first.Left + first.Width 
                 || first.Left > second.Left + second.Width 
                 || second.Top > first.Top + first.Height 
                 || first.Top > second.Top + second.Height);
}