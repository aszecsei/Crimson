namespace Crimson.AI.UtilityAI
{
    public class UtilityAI<T>
    {
        public float UpdatePeriod;

        private T _context;
        private Reasoner<T> _rootReasoner;
        private float _elapsedTime;

        public UtilityAI(T context, Reasoner<T> rootReasoner, float updatePeriod = 0.2f)
        {
            _rootReasoner = rootReasoner;
            _context = context;
            UpdatePeriod = _elapsedTime = updatePeriod;
        }

        public void Tick()
        {
            _elapsedTime -= Time.DeltaTime;
            while (_elapsedTime <= 0)
            {
                _elapsedTime += UpdatePeriod;
                var action = _rootReasoner.Select(_context);
                action?.Execute(_context);
            }
        }
    }

    /// <summary>
    /// Convenience class used for Blackboard-based contexts.
    /// </summary>
    public class UtilityAI : UtilityAI<Blackboard>
    {
        public UtilityAI(Blackboard context, Reasoner<Blackboard> rootReasoner, float updatePeriod = 0.2f) : base(context, rootReasoner, updatePeriod)
        {
        }

        public UtilityAI(Reasoner<Blackboard> rootReasoner, float updatePeriod = 0.2f) : base(new Blackboard(),
            rootReasoner, updatePeriod)
        {
        }
    }
}