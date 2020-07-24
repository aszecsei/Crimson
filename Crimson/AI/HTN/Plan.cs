using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    /// <summary>
    /// Wraps a sequence of primitive tasks. Runnable from an <see cref="Agent"/>.
    /// </summary>
    public class Plan : Operator, IEnumerable<PrimitiveTask>
    {
        private readonly List<PrimitiveTask> _plan;
        private TaskInstance[] _instances = new TaskInstance[0];
        private int _currentOperator = 0;

        public Plan(List<PrimitiveTask> plan)
        {
            _plan = plan;
        }

        public override void OnStart()
        {
            base.OnStart();
            _currentOperator = 0;
            
            _instances = new TaskInstance[_plan.Count];
            for (var i = 0; i < _plan.Count; ++i)
                _instances[i] = _plan[i].Instance();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _currentOperator = 0;
        }

        public override TaskStatus Update(Blackboard context)
        {
            if (_currentOperator >= _instances.Length)
                return TaskStatus.Invalid;
            
            var result = _instances[_currentOperator].Update(context);
            _currentOperator += 1;
            return result;
        }

        public IEnumerator<PrimitiveTask> GetEnumerator()
        {
            return _plan.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}