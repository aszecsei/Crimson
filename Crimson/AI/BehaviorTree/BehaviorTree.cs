namespace Crimson.AI.BehaviorTree
{
    public struct BehaviorTree
    {
        public Node Root;

        public Agent Instance(Blackboard? blackboard = null, float updateTime = 0.2f)
        {
            return new Agent(blackboard ?? new Blackboard(), Root, updateTime);
        }
    }
}