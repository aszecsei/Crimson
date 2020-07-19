namespace Crimson.AI.BehaviorTree
{
    public class BehaviorTree<T>
    {
        public float UpdatePeriod;

        private T _context;
        private Behavior<T> _root;
        private float _elapsedTime;

        internal Behavior<T> Root => _root;
        public T Context => _context;

        public BehaviorTree(T context, Behavior<T> rootNode, float updatePeriod = 0.2f)
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

    public class BehaviorTree : BehaviorTree<Blackboard>
    {
        public BehaviorTree(Blackboard context, Behavior<Blackboard> rootNode, float updatePeriod = 0.2f) : base(context, rootNode, updatePeriod)
        {
        }
    }
}