namespace Crimson.AI
{
    public interface IConditional
    {
        TaskStatus Update(Blackboard context);
    }
}