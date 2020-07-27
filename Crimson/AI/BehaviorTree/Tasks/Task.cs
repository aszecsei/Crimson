using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// Wraps an Operator. Used to construct behavior trees using modular operators.
    ///
    /// Essentially the leaf nodes of the tree.
    /// </summary>
    public class Task : Node
    {
        public Operator Operator;
        
        public Task(Operator @operator)
        {
            Operator = @operator;
        }
        
        public override string OperatorType => "Task";

        public override bool IsSatisfied(Blackboard context) => Operator.IsSatisfied(context);

        public override void Execute(Blackboard context) => Operator.Execute(context);

        protected override TaskStatus Tick(Blackboard context) => Operator.Update(context);

        public override void OnStart() => Operator.OnStart();

        public override void OnEnd() => Operator.OnEnd();

        public override int Cost => Operator.Cost;

        public override int Utility => Operator.Utility;
    }
}