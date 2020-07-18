namespace Crimson.AI.BehaviorTree
{
    public interface IConditional<T>
    {
        TaskStatus Update(T context);
    }
}