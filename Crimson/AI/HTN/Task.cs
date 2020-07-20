using System;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public abstract class Task
    {
        public string Name;

        internal readonly List<IConditional> PreConditions = new List<IConditional>();

        public bool IsSatisfied(Blackboard context)
        {
            for (int i = 0; i < PreConditions.Count; ++i)
            {
                if (PreConditions[i].Update(context) != TaskStatus.Success)
                    return false;
            }

            return true;
        }

        public void AddPreCondition(IConditional condition)
        {
            PreConditions.Add(condition);
        }
    }
}