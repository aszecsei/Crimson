using System;
using System.Collections.Generic;

namespace Crimson.AI.GOAP
{
    public abstract class Action : Operator
    {
        public override string OperatorType => "Action";

        public List<IConditional> PreConditions = new List<IConditional>();

        public void AddPrecondition(IConditional condition)
        {
            PreConditions.Add(condition);
        }

        public override bool IsSatisfied(Blackboard context)
        {
            foreach (IConditional conditional in PreConditions)
                if (conditional.Update(context) != TaskStatus.Success)
                    return false;
            return true;
        }
    }
}