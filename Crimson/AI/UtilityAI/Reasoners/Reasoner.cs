using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    public abstract class Reasoner
    {
        public IConsideration DefaultConsideration = new FixedScoreConsideration();
        
        protected List<IConsideration> _considerations = new List<IConsideration>();

        public IAction? Select(Blackboard context)
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
    }
}