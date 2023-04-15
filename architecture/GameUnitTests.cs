using NUnit.Framework;

namespace SpaceGame.architecture;

[TestFixture]
public class GameUnitTests
{
    [TestCase(
        new[]
        {
            0, 0,
            0, 0
        },
        new[] { 5, 5 },
        new[] { 5, 5 },
        true
    )]
    [TestCase(
        new[]
        {
            0, 0,
            100, 100
        },
        new[] { 5, 5 },
        new[] { 5, 5 },
        false
    )]
    [TestCase(
        new[]
        {
            0, 0,
            6, 0
        },
        new[] { 5, 5 },
        new[] { 5, 5 },
        false
    )]
    [TestCase(
        new[]
        {
            0, 0,
            4, 4
        },
        new[] { 5, 5 },
        new[] { 5, 5 },
        true
    )]
    
    [Test]
    public void MembersCross_Test(int[] locations, int[] widths, int[] heights, bool expected)
    {
        if (locations.Length != 4 || widths.Length != 2 || heights.Length != 2)
            throw new ArgumentException("Ну чо за тестовые данные?");

        var f = new PictureBox
        {
            Location = new Point(locations[0], locations[1]),
            Size = new Size(widths[0], heights[0])
        };

        var s = new PictureBox
        {
            Location = new Point(locations[2], locations[3]),
            Size = new Size(widths[1], heights[1])
        };

        Assert.AreEqual(expected, new EnemyModel(f).IsCrossing(s));
    }
}