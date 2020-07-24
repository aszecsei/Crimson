namespace Crimson.AI
{
    /// <summary>
    /// A class responsible for running a task. This might be a behavior tree, a GOAP action or sequence of actions, or
    /// something else that is a subclass of <see cref="Operator"/>.
    /// </summary>
    public class Agent
    {
        public readonly float UpdatePeriod;

        private Blackboard _context;
        private TaskInstance _task;
        private float _elapsedTime;

        internal TaskInstance Task => _task;
        public Blackboard Context => _context;

        public Agent(Blackboard context, Operator task, float updatePeriod = 0.2f)
        {
            _context = context;
            _task = task.Instance();
            UpdatePeriod = _elapsedTime = updatePeriod;
        }

        public void Tick()
        {
            if (UpdatePeriod > 0)
            {
                _elapsedTime -= Time.DeltaTime;
                while (_elapsedTime <= 0)
                {
                    _elapsedTime += UpdatePeriod;
                    _task.Tick(_context);
                }
            }
            else
            {
                _task.Tick(_context);
            }
        }
    }
}