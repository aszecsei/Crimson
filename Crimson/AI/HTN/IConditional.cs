namespace Crimson.AI.HTN
{
    public interface IConditional<T>
    {
        bool Update(T context);
    }
}