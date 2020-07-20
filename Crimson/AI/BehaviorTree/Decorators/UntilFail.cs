﻿namespace Crimson.AI.BehaviorTree
{
    [AITag("UntilFail")]
    public class UntilFail<T> : Decorator<T>
    {
        public override TaskStatus Update(T context)
        {
            Assert.IsNotNull(Child, "child must not be null");

            var status = Child.Tick(context);
            
            if (status != TaskStatus.Failure)
                return TaskStatus.Running;
            
            return TaskStatus.Success;
        }
    }
}