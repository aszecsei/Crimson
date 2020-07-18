using System.Collections.Generic;

namespace Crimson.AI.UtilityAI
{
    public abstract class Reasoner<T>
    {
        public IConsideration<T> DefaultConsideration = new FixedScoreConsideration<T>();
        
        protected List<IConsideration<T>> _considerations = new List<IConsideration<T>>();

        public IAction<T>? Select(T context)
        {
            var consideration = SelectBestConsideration(context);
            return consideration?.Action;
        }

        protected abstract IConsideration<T>? SelectBestConsideration(T context);

        public Reasoner<T> AddConsideration(IConsideration<T> consideration)
        {
            _considerations.Add(consideration);
            return this;
        }

        public Reasoner<T> SetDefaultConsideration(IConsideration<T> defaultConsideration)
        {
            DefaultConsideration = defaultConsideration;
            return this;
        }
    }
}