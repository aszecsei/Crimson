using System;
using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public class Method<T> : IEnumerable<string>
        where T : class, ICloneable
    {
        private List<IConditional<T>> _preConditions;
        private List<string> _subTasks = new List<string>();

        public Method(params IConditional<T>[] conditions)
        {
            _preConditions = new List<IConditional<T>>(conditions);
        }

        public void Add(Task<T> subTask)
        {
            _subTasks.Add(subTask.Name);
        }
        
        public void Add(string taskName)
        {
            _subTasks.Add(taskName);
        }

        public void Add(IConditional<T> preCondition)
        {
            _preConditions.Add(preCondition);
        }

        public bool IsSatisfied(T context)
        {
            for (int i = 0; i < _preConditions.Count; ++i)
            {
                if (!_preConditions[i].Update(context))
                    return false;
            }

            return true;
        }
        
        public IEnumerator<string> GetEnumerator()
        {
            return _subTasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int GetHeuristic(TaskPlanner<T> planner, T context)
        {
            int res = 0;
            for (var i = 0; i < _subTasks.Count; ++i)
                res += planner[_subTasks[i]].GetHeuristic(planner, context);
            return res;
        }
    }
}