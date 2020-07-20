using System;
using System.Collections.Generic;

namespace Crimson.AI.GOAP
{
    public class Action
    {
        public string Name;
        public int Cost = 1;

        public List<IConditional> PreConditions;

        public Action() {}

        public Action(string name)
        {
            Name = name;
        }

        public Action(string name, int cost) : this(name)
        {
            Cost = cost;
        }

        public void AddPrecondition(IConditional condition)
        {
            PreConditions.Add(condition);
        }
        
        public virtual void Execute(Blackboard context) {}

        public virtual bool Validate(Blackboard context)
        {
            foreach (IConditional conditional in PreConditions)
                if (conditional.Update(context) != TaskStatus.Success)
                    return false;
            return true;
        }

        public override string ToString()
        {
            return $"[Action] {Name} - cost: {Cost}";
        }
    }
}