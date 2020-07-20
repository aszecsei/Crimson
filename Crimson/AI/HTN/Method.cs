using System;
using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public class Method : IEnumerable<string>
    {
        private List<IConditional> _preConditions;
        private List<string> _subTasks = new List<string>();

        public Method(params IConditional[] conditions)
        {
            _preConditions = new List<IConditional>(conditions);
        }

        public void Add(Task subTask)
        {
            _subTasks.Add(subTask.Name);
        }
        
        public void Add(string taskName)
        {
            _subTasks.Add(taskName);
        }

        public void Add(IConditional preCondition)
        {
            _preConditions.Add(preCondition);
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
        
        public IEnumerator<string> GetEnumerator()
        {
            return _subTasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}