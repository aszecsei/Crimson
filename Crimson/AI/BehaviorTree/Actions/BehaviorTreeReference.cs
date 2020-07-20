namespace Crimson.AI.BehaviorTree
{
    public class BehaviorTreeReference : Behavior
    {
        private BehaviorTree _childTree;

        public BehaviorTreeReference(BehaviorTree tree)
        {
            _childTree = tree;
        }

        public override float Utility => _childTree.Root.Utility;

        public override TaskStatus Update(Blackboard context)
        {
            _childTree.Tick();
            return TaskStatus.Success;
        }
    }
}