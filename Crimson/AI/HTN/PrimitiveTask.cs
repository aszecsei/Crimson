using System;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public abstract class PrimitiveTask : Task
    {
        private int _cost;
        
        public PrimitiveTask(string name, int cost = 1)
        {
            Name = name;
            _cost = cost;
        }
        
        public virtual void Execute(Blackboard context) {}

        public virtual int GetCost(Blackboard context) => _cost;

        public virtual int GetHeuristic(Blackboard context) => GetCost(context);

        public abstract TaskStatus Update(Blackboard context);
    }
}