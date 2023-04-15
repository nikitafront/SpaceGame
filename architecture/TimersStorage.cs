using Timer = System.Windows.Forms.Timer;

namespace SpaceGame;

public class TimersStorage
{
    public static readonly Dictionary<long, Timer> Timers = new Dictionary<long, Timer>();

    public Timer this[long id] => Timers[id];
    
    public TimersStorage(
        long id,
        bool isEnabled, 
        TimerIntervals interval, 
        Action<object?, EventArgs> onTick
        )
    {
        var timer = new Timer { Enabled = isEnabled, Interval = (int) interval };
        Timers.Add(id, timer);
        Timers[id].Tick += (e, a) => onTick(e, a);
        if (isEnabled) StartTimer(id);
    }

    public static void StartTimer(long id)
    {
        if (!Timers[id].Enabled) Timers[id].Enabled = true;
        Timers[id].Start();
    }
}