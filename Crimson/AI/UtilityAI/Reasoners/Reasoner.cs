using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    public abstract class Reasoner : Operator
    {
        public IConsideration DefaultConsideration = new FixedScoreConsideration();
        
        protected List<IConsideration> _considerations = new List<IConsideration>();

        public Action? Select(Blackboard context)
        {
            var consideration = SelectBestConsideration(context);
            return consideration?.Action;
        }

        protected abstract IConsideration SelectBestConsideration(Blackboard context);

        public Reasoner AddConsideration(IConsideration consideration)
        {
            _considerations.Add(consideration);
            return this;
        }

        public Reasoner SetDefaultConsideration(IConsideration defaultConsideration)
        {
            DefaultConsideration = defaultConsideration;
            return this;
        }

        public override TaskStatus Update(Blackboard context)
        {
            var action = Select(context);
            return action?.Update(context) ?? TaskStatus.Invalid;
        }
    }
}