using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Crimson.AI.HTN
{
    public class CompoundTask : ITask, IEnumerable<Method>
    {
        public string Name { get; private set; }
        private readonly List<Method> _methods;
        private readonly List<IConditional> _preConditions = new List<IConditional>();

        public CompoundTask(string name, params Method[] methods)
        {
            Name = name;
            _methods = new List<Method>(methods);
        }

        public void Add(Method method)
        {
            _methods.Add(method);
        }
        
        public List<Method> FindSatisfiedMethods(Blackboard context)
        {
            List<Method> res = new List<Method>();
            for (var i = 0; i < _methods.Count; ++i)
            {
                if (_methods[i].IsSatisfied(context))
                {
                    res.Add(_methods[i]);
                }
            }

            return res;
        }

        public IEnumerator<Method> GetEnumerator()
        {
            return _methods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsSatisfied(Blackboard context)
        {
            for (int i = 0; i < _preConditions.Count; ++i)
            {
                if (_preConditions[i].Update(context) != TaskStatus.Success)
                    return false;
            }

            return true;
        }

        public void AddPreCondition(IConditional condition)
        {
            _preConditions.Add(condition);
        }
    }
}