using System.Collections.Generic;

namespace Crimson.AI.BehaviorTree
{
    public abstract class Composite : Behavior
    {
        public AbortType AbortType = AbortType.None;
        
        protected readonly List<Behavior> Children = new List<Behavior>();
        protected TaskInstance[] ChildrenInstances = new TaskInstance[0];
        protected bool HasLowerPriorityConditionalAbort;
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
            HasLowerPriorityConditionalAbort = HasLowerPriorityConditionalAbortInChildren();
            CurrentChildIndex = 0;
            
            ChildrenInstances = new TaskInstance[Children.Count];
            for (var i = 0; i < Children.Count; ++i)
                ChildrenInstances[i] = Children[i].Instance();
        }

        public override void OnEnd()
        {
            for (var i = 0; i < ChildrenInstances.Length; ++i)
                ChildrenInstances[i].Invalidate();
        }

        public void AddChild(Behavior child)
        {
            Children.Add(child);
        }

        public bool IsFirstChildConditional()
        {
            return Children[0] is IConditional;
        }
        
        private bool HasLowerPriorityConditionalAbortInChildren()
        {
            for (var i = 0; i < Children.Count; i++)
            {
                // check for a Composite with an abortType set
                if (Children[i] is Composite composite && composite.AbortType.HasFlag(AbortType.LowerPriority))
                {
                    // now make sure the first child is a Conditional
                    if (composite.IsFirstChildConditional())
                        return true;
                }
            }

            return false;
        }
        
        protected void UpdateLowerPriorityAbortConditional(Blackboard context, TaskStatus statusCheck)
        {
            // check any lower priority tasks to see if they changed status
            for (var i = 0; i < CurrentChildIndex; i++)
            {
                if (Children[i] is Composite composite && composite.AbortType.HasFlag(AbortType.LowerPriority))
                {
                    // now we get the status of only the Conditional (update instead of tick) to see if it changed taking care with ConditionalDecorators
                    var child = composite.Children[0];
                    var status = UpdateConditionalNode(context, child);
                    if (status != statusCheck)
                    {
                        CurrentChildIndex = i;

                        // we have an abort so we invalidate the children so they get reevaluated
                        for (var j = i; j < ChildrenInstances.Length; j++)
                            ChildrenInstances[j].Invalidate();
                        break;
                    }
                }
            }
        }
        
        protected void UpdateSelfAbortConditional(Blackboard context, TaskStatus statusCheck)
        {
            // check any IConditional child tasks to see if they changed status
            for (var i = 0; i < CurrentChildIndex; i++)
            {
                var child = Children[i];
                if (!(child is IConditional))
                    continue;

                var status = UpdateConditionalNode(context, child);
                if (status != statusCheck)
                {
                    CurrentChildIndex = i;

                    // we have an abort so we invalidate the children so they get reevaluated
                    for (var j = i; j < ChildrenInstances.Length; j++)
                        ChildrenInstances[j].Invalidate();
                    break;
                }
            }
        }
        
        private TaskStatus UpdateConditionalNode(Blackboard context, Behavior node)
        {
            if (node is ConditionalDecorator decorator)
                return decorator.ExecuteConditional(context, true);
            else
                return node.Update(context);
        }
    }
}