namespace Crimson.AI.BehaviorTree
{
    [AITag("Repeat")]
    public class Repeater : Decorator
    {
        public int Count;
        public bool RepeatForever;
        public bool EndOnFailure;

        private int _iterationCount;

        public Repeater(int count, bool endOnFailure = false)
        {
            Count = count;
            RepeatForever = false;
            EndOnFailure = endOnFailure;
        }

        public Repeater(bool endOnFailure = false)
        {
            Count = -1;
            RepeatForever = true;
            EndOnFailure = endOnFailure;
        }

        public override void OnStart()
        {
            _iterationCount = 0;
        }

        public override TaskStatus Update(Blackboard context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            if (!RepeatForever && _iterationCount == Count)
                return TaskStatus.Success;

            var status = Child.Tick(context);
            _iterationCount++;

            if (EndOnFailure && status == TaskStatus.Failure)
                return TaskStatus.Success;

            if (!RepeatForever && _iterationCount == Count)
                return TaskStatus.Success;

            return TaskStatus.Running;
        }
    }
}