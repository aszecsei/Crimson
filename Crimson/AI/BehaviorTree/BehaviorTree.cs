namespace Crimson.AI.BehaviorTree
{
    public class BehaviorTree
    {
        public float UpdatePeriod;

        private Blackboard _context;
        private Behavior _root;
        private float _elapsedTime;

        internal Behavior Root => _root;
        public Blackboard Context => _context;

        public BehaviorTree(Blackboard context, Behavior rootNode, float updatePeriod = 0.2f)
        {
            _context = context;
            _root = rootNode;
            UpdatePeriod = _elapsedTime = updatePeriod;
        }

        public void Tick()
        {
            if (UpdatePeriod > 0)
            {
                _elapsedTime -= Time.DeltaTime;
                if (_elapsedTime <= 0)
                {
                    while (_elapsedTime <= 0)
                        _elapsedTime += UpdatePeriod;
                    _root.Tick(_context);
                }
            }
            else
            {
                _root.Tick(_context);
            }
        }
    }
}