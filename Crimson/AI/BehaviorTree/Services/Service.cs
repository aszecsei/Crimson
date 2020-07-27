namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// Designed to perform "background" tasks that update an AI's knowledge.
    ///
    /// Services are being executed when the underlying branch of the behavior tree becomes active, but unlike tasks
    /// they don't return any results and can't directly affect execution flow.
    /// </summary>
    public abstract class Service : Node
    {
        private const float DEFAULT_INTERVAL = 0.5f;
        private const float DEFAULT_RANDOM_DEVIATION = 0.1f;
        
        private readonly float _interval;
        private readonly float _randomDeviation;
        private float _waitTime = 0f;
        private float _startTime = 0f;

        protected Service(float interval = DEFAULT_INTERVAL, float randomDeviation = DEFAULT_RANDOM_DEVIATION)
        {
            _interval = interval;
            _randomDeviation = randomDeviation;
        }

        public override void OnStart()
        {
            _startTime = 0f;
            ScheduleNextTick();
        }

        protected sealed override TaskStatus Tick(Blackboard context)
        {
            if (Mathf.Approximately(_startTime, 0))
                _startTime = Time.TotalTime;
            
            float elapsedTime = Time.TotalTime - _startTime;
            if (elapsedTime >= _waitTime)
            {
                _startTime = Time.TotalTime + (elapsedTime - _waitTime);
                ScheduleNextTick();
                Tick(context);
                return TaskStatus.Success;
            }
                
            return TaskStatus.Running;
        }

        private void ScheduleNextTick()
        {
            _waitTime = _interval + Utils.Random.Range(-_randomDeviation, _randomDeviation);
        }
    }
}