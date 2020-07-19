namespace Crimson.AI
{
    public interface ISensor<T>
    {
        void Sense(Entity self, T context);
        SensorPriority Priority { get; }
    }
    
    public interface ISensor : ISensor<Blackboard> {}

    public enum SensorPriority
    {
        Critical,
        High,
        Medium,
        Low,
    }
}