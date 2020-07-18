using System;
using System.Collections.Generic;

namespace Crimson.AI.BehaviorTree
{
    public class BehaviorTreeBuilder<T>
    {
        private readonly T _context;
        private Behavior<T>? _currentNode;
        private readonly Stack<Behavior<T>> _parentNodeStack = new Stack<Behavior<T>>();

        public BehaviorTreeBuilder(T context)
        {
            _context = context;
        }

        public static BehaviorTreeBuilder<T> Begin(T context)
        {
            return new BehaviorTreeBuilder<T>(context);
        }

        private BehaviorTreeBuilder<T> SetChildOnParent(Behavior<T> child)
        {
            var parent = _parentNodeStack.Peek();
            switch (parent)
            {
                case Composite<T> composite:
                    composite.AddChild(child);
                    break;
                case Decorator<T> decorator:
                    decorator.Child = child;
                    EndDecorator();
                    break;
            }

            return this;
        }

        private BehaviorTreeBuilder<T> PushParentNode(Behavior<T> composite)
        {
            if (_parentNodeStack.Count > 0)
                SetChildOnParent(composite);
            
            _parentNodeStack.Push(composite);
            return this;
        }

        private void EndDecorator()
        {
            _currentNode = _parentNodeStack.Pop();
        }

        #region Leaf Nodes (actions and sub trees)

        public BehaviorTreeBuilder<T> Action(Func<T, TaskStatus> func)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested action node. it must be a leaf node");
            return SetChildOnParent(new ExecuteAction<T>(func));
        }

        public BehaviorTreeBuilder<T> Action(Func<T, bool> func)
        {
            return Action(t => func(t) ? TaskStatus.Success : TaskStatus.Failure);
        }

        public BehaviorTreeBuilder<T> Conditional(Func<T, TaskStatus> func)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested conditional node. it must be a leaf node");
            return SetChildOnParent(new ExecuteActionConditional<T>(func));
        }

        public BehaviorTreeBuilder<T> Conditional(Func<T, bool> func)
        {
            return Conditional(t => func(t) ? TaskStatus.Success : TaskStatus.Failure);
        }

        public BehaviorTreeBuilder<T> LogAction(string text)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested action node. it must be a leaf node");
            return SetChildOnParent(new LogAction<T>(text));
        }

        public BehaviorTreeBuilder<T> WaitAction(float waitTime)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested action node. it must be a leaf node");
            return SetChildOnParent(new WaitAction<T>(waitTime));
        }

        public BehaviorTreeBuilder<T> SubTree(BehaviorTree<T> subTree)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't splice an unnested sub tree, there must be a parent tree");
            return SetChildOnParent(new BehaviorTreeReference<T>(subTree));
        }

        #endregion

        #region Decorators

        public BehaviorTreeBuilder<T> ConditionalDecorator(Func<T, TaskStatus> func, bool shouldReevaluate = true)
        {
            return PushParentNode(new ConditionalDecorator<T>(new ExecuteActionConditional<T>(func), shouldReevaluate));
        }

        public BehaviorTreeBuilder<T> ConditionalDecorator(Func<T, bool> func, bool shouldReevaluate = true)
        {
            return ConditionalDecorator(t => func(t) ? TaskStatus.Success : TaskStatus.Failure, shouldReevaluate);
        }

        public BehaviorTreeBuilder<T> AlwaysFail()
        {
            return PushParentNode(new AlwaysFail<T>());
        }

        public BehaviorTreeBuilder<T> AlwaysSucceed()
        {
            return PushParentNode(new AlwaysSucceed<T>());
        }

        public BehaviorTreeBuilder<T> Inverter()
        {
            return PushParentNode(new Inverter<T>());
        }

        public BehaviorTreeBuilder<T> Repeater(int count, bool endOnFailure = false)
        {
            return PushParentNode(new Repeater<T>(count, endOnFailure));
        }

        public BehaviorTreeBuilder<T> Repeater(bool endOnFailure = false)
        {
            return PushParentNode(new Repeater<T>(endOnFailure));
        }

        public BehaviorTreeBuilder<T> UntilFail()
        {
            return PushParentNode(new UntilFail<T>());
        }

        public BehaviorTreeBuilder<T> UntilSuccess()
        {
            return PushParentNode(new UntilSuccess<T>());
        }

        #endregion

        #region Composites

        public BehaviorTreeBuilder<T> Parallel()
        {
            return PushParentNode(new Parallel<T>());
        }


        public BehaviorTreeBuilder<T> ParallelSelector()
        {
            return PushParentNode(new ParallelSelector<T>());
        }


        public BehaviorTreeBuilder<T> Selector(AbortType abortType = AbortType.None)
        {
            return PushParentNode(new Selector<T>(abortType));
        }


        public BehaviorTreeBuilder<T> RandomSelector()
        {
            return PushParentNode(new RandomSelector<T>());
        }


        public BehaviorTreeBuilder<T> Sequence(AbortType abortType = AbortType.None)
        {
            return PushParentNode(new Sequence<T>(abortType));
        }


        public BehaviorTreeBuilder<T> RandomSequence()
        {
            return PushParentNode(new RandomSequence<T>());
        }


        public BehaviorTreeBuilder<T> EndComposite()
        {
            Assert.IsTrue(_parentNodeStack.Peek() is Composite<T>,
                "attempting to end a composite but the top node is a decorator");
            _currentNode = _parentNodeStack.Pop();
            return this;
        }

        #endregion

        public BehaviorTree<T> Build(float updatePeriod = 0.2f)
        {
            Assert.IsNotNull(_currentNode, "can't create a behavior tree without any nodes");
            return new BehaviorTree<T>(_context, _currentNode!, updatePeriod);
        }
    }

    /// <summary>
    /// A default behavior tree builder that uses a blackboard data structure.
    /// </summary>
    public class BehaviorTreeBuilder : BehaviorTreeBuilder<Blackboard>
    {
        public BehaviorTreeBuilder(Blackboard context) : base(context)
        {
        }

        public BehaviorTreeBuilder() : base(new Blackboard())
        {
        }
    }
}