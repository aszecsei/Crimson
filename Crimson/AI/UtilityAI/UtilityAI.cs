namespace Crimson.AI.UtilityAI
{
    public class UtilityAI
    {
        public float UpdatePeriod;

        private Blackboard _context;
        private Reasoner _rootReasoner;
        private float _elapsedTime;

        public UtilityAI(Blackboard context, Reasoner rootReasoner, float updatePeriod = 0.2f)
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
}