using System;

namespace Crimson.AI.HTN
{
    public class FunctionConditional : IConditional
    {
        private readonly Func<Blackboard, TaskStatus> _func;

        public FunctionConditional(Func<Blackboard, TaskStatus> func)
        {
            _func = func;
        }

        public FunctionConditional(Func<Blackboard, bool> func)
        {
            _func = x => func(x) ? TaskStatus.Success : TaskStatus.Failure;
        }
        
        public TaskStatus Update(Blackboard context)
        {
            return _func(context);
        }
    }
}