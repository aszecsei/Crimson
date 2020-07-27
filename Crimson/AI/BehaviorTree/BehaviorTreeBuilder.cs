using System;
using System.Collections.Generic;

namespace Crimson.AI.BehaviorTree
{
    public class BehaviorTreeBuilder
    {
        private Node? _currentNode;
        private readonly Stack<Composite> _parentNodeStack = new Stack<Composite>();

        private BehaviorTreeBuilder() {}

        public static BehaviorTreeBuilder Begin()
        {
            return new BehaviorTreeBuilder();
        }

        private BehaviorTreeBuilder SetChildOnParent(Node child)
        {
            var parent = _parentNodeStack.Peek();
            parent.AddChild(child);
            _currentNode = child;
            return this;
        }

        private BehaviorTreeBuilder PushParentNode(Composite composite)
        {
            if (_parentNodeStack.Count > 0)
                SetChildOnParent(composite);
            
            _parentNodeStack.Push(composite);
            return this;
        }

        #region Leaf Nodes (actions and sub trees)

        public BehaviorTreeBuilder Task(Operator @operator)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested task node. it must be a leaf node");
            return SetChildOnParent(new Task(@operator));
        }

        public BehaviorTreeBuilder Wait(float duration)
        {
            return Task(new WaitOperator(duration));
        }

        public BehaviorTreeBuilder Log(string text, bool isError = false)
        {
            return Task(new LogOperator(text, isError));
        }
        
        public BehaviorTreeBuilder Task(Func<Blackboard, TaskStatus> action, int utility = 1, int cost = 1)
        {
            return Task(new ExecuteOperator(action, utility, cost));
        }
        
        public BehaviorTreeBuilder FinishWithResult(TaskStatus result)
        {
            return Task(new FinishWithResult(result));
        }

        public BehaviorTreeBuilder FinishWithResult(string result)
        {
            return Task(new FinishWithResult(result));
        }

        public BehaviorTreeBuilder TaskRunner(Agent agent)
        {
            return Task(new TaskRunnerOperator(agent));
        }

        #endregion

        #region Decorators

        public BehaviorTreeBuilder Decorator(Decorator deco)
        {
            Assert.IsNotNull(_currentNode, "can't add a decorator without creating a node first");
            _currentNode!.Add(deco);
            return this;
        }
        
        public BehaviorTreeBuilder ConditionalDecorator(IConditional cond, bool shouldReevaluate = true, bool isInversed = false, AbortMode abortMode = AbortMode.None)
        {
            return Decorator(new ConditionalDecorator(cond, shouldReevaluate, isInversed, abortMode));
        }

        public BehaviorTreeBuilder AlwaysFail()
        {
            return Decorator(new AlwaysFail());
        }

        public BehaviorTreeBuilder AlwaysSucceed()
        {
            return Decorator(new AlwaysSucceed());
        }

        public BehaviorTreeBuilder Inverter()
        {
            return Decorator(new Inverter());
        }

        public BehaviorTreeBuilder Repeater(int count, bool endOnFailure = false)
        {
            return Decorator(new Repeater(count, endOnFailure));
        }

        public BehaviorTreeBuilder Repeater(bool endOnFailure = false)
        {
            return Decorator(new Repeater(endOnFailure));
        }

        public BehaviorTreeBuilder UntilFail()
        {
            return Decorator(new UntilFail());
        }

        public BehaviorTreeBuilder UntilSuccess()
        {
            return Decorator(new UntilSuccess());
        }

        #endregion

        #region Services

        public BehaviorTreeBuilder Service(Service service)
        {
            Assert.IsNotNull(_currentNode, "can't add a service without creating a node first");
            _currentNode!.Add(service);
            return this;
        }

        #endregion

        #region Composites

        public BehaviorTreeBuilder Composite(Composite composite)
        {
            return PushParentNode(composite);
        }

        public BehaviorTreeBuilder Parallel()
        {
            return PushParentNode(new Parallel());
        }


        public BehaviorTreeBuilder ParallelSelector()
        {
            return PushParentNode(new ParallelSelector());
        }


        public BehaviorTreeBuilder Selector()
        {
            return PushParentNode(new Selector());
        }


        public BehaviorTreeBuilder RandomSelector()
        {
            return PushParentNode(new RandomSelector());
        }


        public BehaviorTreeBuilder Sequence()
        {
            return PushParentNode(new Sequence());
        }


        public BehaviorTreeBuilder RandomSequence()
        {
            return PushParentNode(new RandomSequence());
        }


        public BehaviorTreeBuilder EndComposite()
        {
            _currentNode = _parentNodeStack.Pop();
            return this;
        }

        #endregion

        public BehaviorTree Build(float updatePeriod = 0.2f)
        {
            Assert.IsNotNull(_currentNode, "can't create a behavior tree without any nodes");
            return new BehaviorTree
            {
                Root = _currentNode!
            };
        }
    }
}