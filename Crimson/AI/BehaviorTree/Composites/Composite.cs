using System.Collections.Generic;

namespace Crimson.AI.BehaviorTree
{
    public abstract class Composite : Behavior
    {
        public AbortType AbortType = AbortType.None;
        
        protected readonly List<Behavior> Children = new List<Behavior>();
        protected bool HasLowerPriorityConditionalAbort;
        protected int CurrentChildIndex = 0;

        public override float Utility
        {
            get
            {
                // average children utility
                float maxUtility = 0;
                for (int i = 0; i < Children.Count; ++i)
                    maxUtility += Children[i].Utility;
                return maxUtility / Children.Count;
            }
        }

        public override void Invalidate()
        {
            base.Invalidate();
            
            for (var i = 0; i < Children.Count; ++i)
                Children[i].Invalidate();
        }

        public override void OnStart()
        {
            HasLowerPriorityConditionalAbort = HasLowerPriorityConditionalAbortInChildren();
            CurrentChildIndex = 0;
        }

        public override void OnEnd()
        {
            for (var i = 0; i < Children.Count; ++i)
                Children[i].Invalidate();
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
                        for (var j = i; j < Children.Count; j++)
                            Children[j].Invalidate();
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
                    for (var j = i; j < Children.Count; j++)
                        Children[j].Invalidate();
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