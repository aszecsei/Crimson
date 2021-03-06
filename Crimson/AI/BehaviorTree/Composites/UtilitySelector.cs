﻿namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// Same as Selector, except it selects the highest-utility children first
    /// </summary>
    [AITag("UtilitySelector")]
    public class UtilitySelector : Selector
    {
        public override void OnStart()
        {
            // Sort from high utility to low utility
            Children.Sort((Node c1, Node c2) => c2.Utility.CompareTo(c1.Utility));
        }
    }
}