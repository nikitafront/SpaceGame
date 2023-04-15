using Timer = System.Windows.Forms.Timer;

namespace SpaceGame;

public class TimersStorage
{
    private static readonly Dictionary<TimersNames, Timer> timers = new Dictionary<TimersNames, Timer>();

    public Timer this[TimersNames id] => timers[id];
    
    public TimersStorage(
        TimersNames id, 
        bool isEnabled, 
        int interval, 
        Action<object?, EventArgs> onTick
        )
    {
        timers.Add(id , new Timer { Enabled = isEnabled, Interval = interval });
        timers[id].Tick += (e, a) => onTick(e, a);
        if (isEnabled) StartTimer(id);
    }

    public static void StartTimer(TimersNames id)
    {
        if (!timers[id].Enabled) timers[id].Enabled = true;
        timers[id].Start();
    }
}