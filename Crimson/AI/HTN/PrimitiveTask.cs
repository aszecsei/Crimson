using System;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public abstract class PrimitiveTask<T> : Task<T>
        where T : class, ICloneable
    {
        private int _cost;
        
        public PrimitiveTask(string name, int cost = 1)
        {
            Name = name;
            _cost = cost;
        }
        
        public virtual void Execute(T context) {}

        public virtual int GetCost(T context) => _cost;

        public virtual int GetHeuristic(T context) => GetCost(context);

        public abstract TaskStatus Update(T context);
    }
}