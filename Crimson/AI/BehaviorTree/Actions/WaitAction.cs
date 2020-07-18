namespace Crimson.AI.BehaviorTree
{
    public class WaitAction<T> : Behavior<T>
    {
        public float WaitTime;

        private float _startTime;

        public WaitAction(float waitTime)
        {
            WaitTime = waitTime;
        }

        public override void OnStart()
        {
            _startTime = 0;
        }

        public override TaskStatus Update(T context)
        {
            if (_startTime == 0)
                _startTime = Time.TotalTime;
            if (Time.TotalTime - _startTime >= WaitTime)
                return TaskStatus.Success;
            return TaskStatus.Running;
        }
    }
}