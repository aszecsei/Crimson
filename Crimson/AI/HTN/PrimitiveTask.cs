using System;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public abstract class PrimitiveTask : Operator, ITask
    {
        private readonly List<IConditional> _preConditions = new List<IConditional>();
        
        public override int Cost { get; }

        protected PrimitiveTask(string name, int cost = 1)
        {
            Name = name;
            Cost = cost;
        }

        public override bool IsSatisfied(Blackboard context)
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

        public virtual int GetCost(Blackboard context) => Cost;

        public virtual int GetHeuristic(Blackboard context) => GetCost(context);
    }
}