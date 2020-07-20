using System;
using System.Collections.Generic;

namespace Crimson.AI.BehaviorTree
{
    public class BehaviorTreeBuilder
    {
        private readonly Blackboard _context;
        private Behavior? _currentNode;
        private readonly Stack<Behavior> _parentNodeStack = new Stack<Behavior>();

        public BehaviorTreeBuilder(Blackboard context)
        {
            _context = context;
        }

        public static BehaviorTreeBuilder Begin(Blackboard context)
        {
            return new BehaviorTreeBuilder(context);
        }

        private BehaviorTreeBuilder SetChildOnParent(Behavior child)
        {
            var parent = _parentNodeStack.Peek();
            switch (parent)
            {
                case Composite composite:
                    composite.AddChild(child);
                    break;
                case Decorator decorator:
                    decorator.Child = child;
                    EndDecorator();
                    break;
            }

            return this;
        }

        private BehaviorTreeBuilder PushParentNode(Behavior composite)
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

        public BehaviorTreeBuilder Action(Func<Blackboard, TaskStatus> func)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested action node. it must be a leaf node");
            return SetChildOnParent(new ExecuteAction(func));
        }

        public BehaviorTreeBuilder Action(Func<Blackboard, bool> func)
        {
            return Action(t => func(t) ? TaskStatus.Success : TaskStatus.Failure);
        }

        public BehaviorTreeBuilder Action(Behavior action)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested action node. it must be a leaf node");
            return SetChildOnParent(action);
        }

        public BehaviorTreeBuilder Conditional(Func<Blackboard, TaskStatus> func)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested conditional node. it must be a leaf node");
            return SetChildOnParent(new ExecuteActionConditional(func));
        }

        public BehaviorTreeBuilder Conditional(Func<Blackboard, bool> func)
        {
            return Conditional(t => func(t) ? TaskStatus.Success : TaskStatus.Failure);
        }

        public BehaviorTreeBuilder LogAction(string text)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested action node. it must be a leaf node");
            return SetChildOnParent(new LogAction(text));
        }

        public BehaviorTreeBuilder WaitAction(float waitTime)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't create an unnested action node. it must be a leaf node");
            return SetChildOnParent(new WaitAction(waitTime));
        }

        public BehaviorTreeBuilder SubTree(BehaviorTree subTree)
        {
            Assert.IsFalse(_parentNodeStack.Count == 0, "can't splice an unnested sub tree, there must be a parent tree");
            return SetChildOnParent(new BehaviorTreeReference(subTree));
        }

        #endregion

        #region Decorators

        public BehaviorTreeBuilder ConditionalDecorator(Func<Blackboard, TaskStatus> func, bool shouldReevaluate = true)
        {
            return PushParentNode(new ConditionalDecorator(new ExecuteActionConditional(func), shouldReevaluate));
        }

        public BehaviorTreeBuilder ConditionalDecorator(Func<Blackboard, bool> func, bool shouldReevaluate = true)
        {
            return ConditionalDecorator(t => func(t) ? TaskStatus.Success : TaskStatus.Failure, shouldReevaluate);
        }

        public BehaviorTreeBuilder ConditionalDecorator(IConditional cond, bool shouldReevaluate = true)
        {
            return PushParentNode(new ConditionalDecorator(cond, shouldReevaluate));
        }

        public BehaviorTreeBuilder AlwaysFail()
        {
            return PushParentNode(new AlwaysFail());
        }

        public BehaviorTreeBuilder AlwaysSucceed()
        {
            return PushParentNode(new AlwaysSucceed());
        }

        public BehaviorTreeBuilder Inverter()
        {
            return PushParentNode(new Inverter());
        }

        public BehaviorTreeBuilder Repeater(int count, bool endOnFailure = false)
        {
            return PushParentNode(new Repeater(count, endOnFailure));
        }

        public BehaviorTreeBuilder Repeater(bool endOnFailure = false)
        {
            return PushParentNode(new Repeater(endOnFailure));
        }

        public BehaviorTreeBuilder UntilFail()
        {
            return PushParentNode(new UntilFail());
        }

        public BehaviorTreeBuilder UntilSuccess()
        {
            return PushParentNode(new UntilSuccess());
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


        public BehaviorTreeBuilder Selector(AbortType abortType = AbortType.None)
        {
            return PushParentNode(new Selector(abortType));
        }


        public BehaviorTreeBuilder RandomSelector()
        {
            return PushParentNode(new RandomSelector());
        }


        public BehaviorTreeBuilder Sequence(AbortType abortType = AbortType.None)
        {
            return PushParentNode(new Sequence(abortType));
        }


        public BehaviorTreeBuilder RandomSequence()
        {
            return PushParentNode(new RandomSequence());
        }


        public BehaviorTreeBuilder EndComposite()
        {
            Assert.IsTrue(_parentNodeStack.Peek() is Composite,
                "attempting to end a composite but the top node is a decorator");
            _currentNode = _parentNodeStack.Pop();
            return this;
        }

        #endregion

        public BehaviorTree Build(float updatePeriod = 0.2f)
        {
            Assert.IsNotNull(_currentNode, "can't create a behavior tree without any nodes");
            return new BehaviorTree(_context, _currentNode!, updatePeriod);
        }
    }
}