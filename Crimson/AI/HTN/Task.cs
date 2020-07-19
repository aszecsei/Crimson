using System;
using System.Collections.Generic;

namespace Crimson.AI.HTN
{
    public abstract class Task<T> where T : class, ICloneable
    {
        public string Name;

        internal readonly List<IConditional<T>> PreConditions = new List<IConditional<T>>();

        public bool IsSatisfied(T context)
        {
            for (int i = 0; i < PreConditions.Count; ++i)
            {
                if (!PreConditions[i].Update(context))
                    return false;
            }

            return true;
        }

        public void AddPreCondition(IConditional<T> condition)
        {
            PreConditions.Add(condition);
        }
    }
}