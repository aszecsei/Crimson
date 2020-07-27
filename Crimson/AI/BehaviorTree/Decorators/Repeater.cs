namespace Crimson.AI.BehaviorTree
{
    [AITag("Repeat")]
    public class Repeater : Decorator
    {
        public readonly int Count;
        public readonly bool RepeatForever;
        public readonly bool EndOnFailure;

        private int _iterationCount;

        public Repeater(int count, bool endOnFailure = false) : base(false)
        {
            Count = count;
            RepeatForever = false;
            EndOnFailure = endOnFailure;
        }

        public Repeater(bool endOnFailure = false) : base(false)
        {
            Count = -1;
            RepeatForever = true;
            EndOnFailure = endOnFailure;
        }

        public override void OnStart()
        {
            base.OnStart();
            _iterationCount = 0;
        }

        protected override TaskStatus Tick(Blackboard context)
        {
            Assert.IsNotNull(ChildInstance, "child must not be null");

            if (!RepeatForever && _iterationCount == Count)
                return TaskStatus.Success;

            var status = ChildInstance!.Tick(context);
            _iterationCount++;

            if (EndOnFailure && status == TaskStatus.Failure)
                return TaskStatus.Success;

            if (!RepeatForever && _iterationCount == Count)
                return TaskStatus.Success;

            return TaskStatus.Running;
        }
    }
}