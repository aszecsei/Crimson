using System;
using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public class CompoundTask<T> : Task<T>, IEnumerable<Method<T>>
        where T : class, ICloneable
    {
        private List<Method<T>> _methods;

        public CompoundTask(string name, params Method<T>[] methods)
        {
            Name = name;
            _methods = new List<Method<T>>(methods);
        }

        public void Add(Method<T> method)
        {
            _methods.Add(method);
        }
        
        public List<Method<T>> FindSatisfiedMethods(T context)
        {
            List<Method<T>> res = new List<Method<T>>();
            for (var i = 0; i < _methods.Count; ++i)
            {
                if (_methods[i].IsSatisfied(context))
                {
                    res.Add(_methods[i]);
                }
            }

            return res;
        }

        public IEnumerator<Method<T>> GetEnumerator()
        {
            return _methods.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}