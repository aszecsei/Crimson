using System;
using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public class CompoundTask : Task, IEnumerable<Method>
    {
        private List<Method> _methods;

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
    }
}