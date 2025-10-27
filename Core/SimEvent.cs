namespace coursework.Core
{
    public class SimEvent
    {
        public double Time { get; }
        public EventType Type { get; }

        public SimEvent(double time, EventType type)
        {
            Time = time;
            Type = type;
        }
    }
}
