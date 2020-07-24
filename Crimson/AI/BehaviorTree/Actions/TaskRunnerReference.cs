namespace Crimson.AI.BehaviorTree
{
    public class TaskRunnerReference : Behavior
    {
        private Agent _childAgent;

        public TaskRunnerReference(Agent agent)
        {
            _childAgent = agent;
        }

        public override int Cost => _childAgent.Task.Cost;
        public override int Utility => _childAgent.Task.Utility;

        public override TaskStatus Update(Blackboard context)
        {
            _childAgent.Tick();
            return TaskStatus.Success;
        }
    }
}