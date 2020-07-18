using System;
using System.Collections.Generic;

namespace Crimson.AI.GOAP
{
    public class Action
    {
        public string Name;
        public int Cost = 1;
        
        internal Dictionary<string, bool> PreConditions = new Dictionary<string, bool>();
        internal Dictionary<string, bool> PostConditions = new Dictionary<string, bool>();
        
        public Action() {}

        public Action(string name)
        {
            Name = name;
        }

        public Action(string name, int cost) : this(name)
        {
            Cost = cost;
        }

        public void SetPrecondition(string conditionName, bool value)
        {
            PreConditions.Add(conditionName, value);
        }

        public void SetPostcondition(string conditionName, bool value)
        {
            PostConditions.Add(conditionName, value);
        }

        public virtual bool Validate() => true;

        public override string ToString()
        {
            return $"[Action] {Name} - cost: {Cost}";
        }
    }
}