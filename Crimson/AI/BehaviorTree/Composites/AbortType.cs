using System;

namespace Crimson.AI.BehaviorTree
{
    [Flags]
    public enum AbortType
    {
        None,
        LowerPriority,
        Self,
        Both = Self | LowerPriority
    }
}