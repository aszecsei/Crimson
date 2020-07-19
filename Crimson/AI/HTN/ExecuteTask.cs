using System;

namespace Crimson.AI.HTN
{
    public class ExecuteTask<T> : PrimitiveTask<T>
    where T : class, ICloneable
    {
        private readonly Action<T> _action;
        
        public ExecuteTask(string name, Action<T> action) : base(name)
        {
            _action = action;
        }

        public override void Execute(T context)
        {
            _action(context);
        }
    }
}