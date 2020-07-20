using System;

namespace Crimson.AI.HTN
{
    public class ExecuteTask : PrimitiveTask
    {
        private readonly Action<Blackboard>? _worldStateEffect;
        private readonly Func<Blackboard, TaskStatus>? _action;
        
        public ExecuteTask(string name, Action<Blackboard>? worldStateEffect = null, Func<Blackboard, TaskStatus>? action = null, int cost = 1) : base(name, cost)
        {
            _worldStateEffect = worldStateEffect;
            _action = action;
        }

        public override void Execute(Blackboard context)
        {
            _worldStateEffect?.Invoke(context);
        }

        public override TaskStatus Update(Blackboard context)
        {
            if (_action != null) return _action(context);
            return TaskStatus.Success;
        }
    }
}