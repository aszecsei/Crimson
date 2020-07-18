namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// Same as Selector, except it selects the highest-utility children first
    /// </summary>
    public class UtilitySelector<T> : Selector<T>
    {
        public override void OnStart()
        {
            // Sort from high utility to low utility
            Children.Sort((Behavior<T> c1, Behavior<T> c2) => c2.Utility.CompareTo(c1.Utility));
        }
    }
}