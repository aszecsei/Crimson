﻿namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// Same as Selector except it shuffles the children when started
    /// </summary>
    public class RandomSelector<T> : Selector<T>
    {
        public override void OnStart()
        {
            Children.Shuffle();
        }
    }
}