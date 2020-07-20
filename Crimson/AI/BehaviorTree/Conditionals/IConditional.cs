namespace Crimson.AI.BehaviorTree
{
    public interface IConditional
    {
        TaskStatus Update(Blackboard context);
    }
}