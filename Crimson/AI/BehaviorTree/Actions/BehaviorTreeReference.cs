namespace Crimson.AI.BehaviorTree
{
    public class BehaviorTreeReference<T> : Behavior<T>
    {
        private BehaviorTree<T> _childTree;

        public BehaviorTreeReference(BehaviorTree<T> tree)
        {
            _childTree = tree;
        }

        public override float Utility => _childTree.Root.Utility;

        public override TaskStatus Update(T context)
        {
            _childTree.Tick();
            return TaskStatus.Success;
        }
    }
}