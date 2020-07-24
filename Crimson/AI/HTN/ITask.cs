namespace Crimson.AI.HTN
{
    public interface ITask
    {
        public bool IsSatisfied(Blackboard context);
        public void AddPreCondition(IConditional condition);
        public string Name { get; }
    }
}