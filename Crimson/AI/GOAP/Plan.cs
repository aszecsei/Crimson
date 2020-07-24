using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI.GOAP
{
    /// <summary>
    /// Wraps a sequence of primitive tasks. Runnable from an <see cref="Agent"/>.
    /// </summary>
    public class Plan : Operator, IEnumerable<Action>
    {
        private readonly Action[] _plan;
        private TaskInstance[] _instances = new TaskInstance[0];
        private int _currentOperator = 0;

        public Plan(Action[] plan)
        {
            _plan = plan;
        }

        public override void OnStart()
        {
            base.OnStart();
            _currentOperator = 0;
            
            _instances = new TaskInstance[_plan.Length];
            for (var i = 0; i < _plan.Length; ++i)
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

        public IEnumerator<Action> GetEnumerator()
        {
            return ((IEnumerable<Action>)_plan).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}