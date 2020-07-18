namespace Crimson.AI.GOAP
{
    public class Action<T> : Action
    {
        protected T Context;

        public Action(T context, string name) : base(name)
        {
            Context = context;
        }

        public Action(T context, string name, int cost) : this(context, name)
        {
            Cost = cost;
        }

        public virtual void Execute() {}
    }
}