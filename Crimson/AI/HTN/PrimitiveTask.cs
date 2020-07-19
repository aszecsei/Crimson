using System;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public class PrimitiveTask<T> : Task<T>
        where T : class, ICloneable
    {
        public virtual void Execute(T context) {}

        public virtual int GetCost(T context) => 0;

        public override int GetHeuristic(TaskPlanner<T> planner, T context) => GetCost(context);
    }
}