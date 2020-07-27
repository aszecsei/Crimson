using System.Collections.Generic;

namespace Crimson.AI.BehaviorTree
{
    public abstract class Composite : Node
    {
        protected readonly List<Node> Children = new List<Node>();
        protected TaskInstance[] ChildrenInstances = new TaskInstance[0];
        protected bool HasLowerPriorityConditionalAbort;
        protected bool HasSelfConditionalAbort;
        /// <summary>
        /// Index of currently active child node.
        /// </summary>
        protected int CurrentChildIndex = 0;

        public override int Cost
        {
            get
            {
                // average children cost
                int totCost = 0;
                for (int i = 0; i < Children.Count; ++i)
                    totCost += Children[i].Utility;
                return totCost / Children.Count;
            }
        }
        
        public override int Utility
        {
            get
            {
                // average children utility
                int totUtility = 0;
                for (int i = 0; i < Children.Count; ++i)
                    totUtility += Children[i].Utility;
                return totUtility / Children.Count;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            
            HasLowerPriorityConditionalAbort = HasLowerPriorityConditionalAbortInChildren();
            HasSelfConditionalAbort = HasSelfConditionalAbortInChildren();
            CurrentChildIndex = 0;
            
            ChildrenInstances = new TaskInstance[Children.Count];
            for (var i = 0; i < Children.Count; ++i)
                ChildrenInstances[i] = Children[i].Instance();
        }

        public override void OnEnd()
        {
            base.OnEnd();
            
            for (var i = 0; i < ChildrenInstances.Length; ++i)
            {
                ChildrenInstances[i].Invalidate();
            }
            ChildrenInstances = new TaskInstance[0];
        }

        public void AddChild(Node child)
        {
            Children.Add(child);
        }
        
        private bool HasLowerPriorityConditionalAbortInChildren()
        {
            for (var i = 0; i < Children.Count; i++)
            {
                // check for a Composite with an abortType set
                for (var j = 0; j < Children[i].Decorators.Count; ++j)
                {
                    if (Children[i].Decorators[j].AbortMode.HasFlag(AbortMode.LowerPriority))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool HasSelfConditionalAbortInChildren()
        {
            for (var i = 0; i < Children.Count; i++)
            {
                // check for a Composite with an abortType set
                for (var j = 0; j < Children[i].Decorators.Count; ++j)
                {
                    if (Children[i].Decorators[j].AbortMode.HasFlag(AbortMode.Self))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        /// <summary>
        /// Checks if any children with a "lower priority abort" decorator has its status changed
        /// </summary>
        protected void UpdateLowerPriorityAbortConditional(Blackboard context, TaskStatus statusCheck)
        {
            // check any higher priority tasks to see if they changed status
            for (var i = 0; i < CurrentChildIndex; i++)
            {
                bool hasAbort = false;
                for (var j = 0; j < Children[i].Decorators.Count; ++j)
                {
                    if (Children[i].Decorators[j].AbortMode.HasFlag(AbortMode.LowerPriority))
                    {
                        var child = Children[i];
                        var status = UpdateConditionals(context, child);
                        if (status != statusCheck)
                        {
                            hasAbort = true;
                        }
                    }
                }

                if (hasAbort)
                {
                    CurrentChildIndex = i;
                    // we have an abort so we invalidate the children so they get reevaluated
                    for (var j = i; j < ChildrenInstances.Length; j++)
                        ChildrenInstances[j].Invalidate();
                    break;
                }
            }
        }
        
        protected void UpdateSelfAbortConditional(Blackboard context, TaskStatus statusCheck)
        {
            var child = Children[CurrentChildIndex];
            var status = UpdateConditionals(context, child);
            if (status != statusCheck)
            {
                CurrentChildIndex--;

                // we have an abort so we invalidate the children so they get reevaluated
                for (var j = CurrentChildIndex; j < ChildrenInstances.Length; j++)
                    ChildrenInstances[j].Invalidate();
            }
        }
        
        private TaskStatus UpdateConditionals(Blackboard context, Node node)
        {
            for (var i = 0; i < node.Decorators.Count; ++i)
            {
                if (node.Decorators[i] is ConditionalDecorator conditionalDecorator)
                {
                    var status = conditionalDecorator.ExecuteConditional(context, true);
                    if (status != TaskStatus.Success)
                        return status;
                }
            }

            return TaskStatus.Success;
        }
    }
}