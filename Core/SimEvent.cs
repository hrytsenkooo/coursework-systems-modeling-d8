namespace coursework.Core
{
    public enum ServiceNode
    {
        Admin,     
        Kitting,    
        Delivery    
    }

    public class SimEvent
    {
        public double Time { get; }
        public EventType Type { get; }
        public ServiceNode? Node { get; } 

        public SimEvent(double time, EventType type, ServiceNode? node = null)
        {
            Time = time;
            Type = type;
            Node = node;
        }
    }
}
