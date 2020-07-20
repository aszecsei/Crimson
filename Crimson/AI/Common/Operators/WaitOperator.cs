namespace Crimson.AI
{
    [AITag("Wait")]
    public class WaitOperator : Operator
    {
        public float WaitTime;

        private float _startTime;

        public WaitOperator(float duration)
        {
            WaitTime = duration;
        }

        public override void OnStart()
        {
            _startTime = 0;
        }

        public override TaskStatus Update(Blackboard context)
        {
            if (_startTime == 0)
                _startTime = Time.TotalTime;
            if (Time.TotalTime - _startTime >= WaitTime)
                return TaskStatus.Success;
            return TaskStatus.Running;
        }
    }
}