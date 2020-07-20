using System;

namespace Crimson.AI.HTN
{
    public class ExecuteTask<T> : PrimitiveTask<T>
    where T : class, ICloneable
    {
        private readonly Action<T>? _worldStateEffect;
        private readonly Func<T, TaskStatus>? _action;
        
        public ExecuteTask(string name, Action<T>? worldStateEffect = null, Func<T, TaskStatus>? action = null, int cost = 1) : base(name, cost)
        {
            _worldStateEffect = worldStateEffect;
            _action = action;
        }

        public override void Execute(T context)
        {
            _worldStateEffect?.Invoke(context);
        }

        public override TaskStatus Update(T context)
        {
            if (_action != null) return _action(context);
            return TaskStatus.Success;
        }
    }
}