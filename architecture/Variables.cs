using Timer = System.Windows.Forms.Timer;

namespace SpaceGame.architecture;

public static class Variables
{
    public static Timer? Timer { get; set; }
    public static long PassedMilliseconds { get; set; }
    
    public static readonly Size PlayerSize = new (80, 80);
    public static readonly Size BonusSize = new (66, 66);
    public static readonly Size HeartSize = new (38, 33);
    public static readonly Size[] EnemySizes = 
    {
        new (60, 60),
        new (100, 100),
        new (120, 120),
        new (200, 200),
    };
    public static readonly Size[] BulletSizes =
    {
        new (4, 8), 
        new (8, 12),
        new (10, 16), 
        new (14, 22)
    };
    public static readonly int[] EnemySpeeds = { 10, 5, 4, 2 };
    public static readonly int[] EnemySpawnIntervals = { 6_000, 14_000, 30_000, 50_000 };
    public static readonly int[] EnemyShootIntervals = { 2_000, 4_000, 6_000, 6_000 };

    public static readonly long PlayerShootInterval = 600;
    public static readonly long MovingInterval = 200;
    public static readonly int BulletSpeed = 20;
    public static readonly int BonusSpeed = 5;

    public static readonly long ClearBulletBonusInterval = 12_000;
    public static readonly Queue<long> ClearBulletBonusTimes = new Queue<long>();
}