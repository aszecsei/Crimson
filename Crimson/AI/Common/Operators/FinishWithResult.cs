using System;

namespace Crimson.AI
{
    [AITag("FinishWithResult")]
    public class FinishWithResult : Operator
    {
        private readonly TaskStatus _status;
        
        public FinishWithResult(string result)
        {
            _status = result switch
            {
                "success" => TaskStatus.Success,
                "failure" => TaskStatus.Failure,
                "invalid" => TaskStatus.Invalid,
                "running" => TaskStatus.Running,
                _ => throw new ArgumentException("value can only be 'success', 'failure', 'invalid', or 'running'",
                    nameof(result))
            };
        }

        public FinishWithResult(TaskStatus result)
        {
            _status = result;
        }
        
        public override TaskStatus Update(Blackboard context)
        {
            return _status;
        }
    }
}