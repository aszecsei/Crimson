using System;

namespace Crimson.AI.BehaviorTree
{
    /// <summary>
    /// Observer aborts will abort a subtree if the selected blackboard key has changed.
    /// </summary>
    [Flags]
    public enum AbortMode : byte
    {
        None = 0,
        /// <summary>
        /// This setting will cause lower-priority trees to abort when the decorator condition changes.
        /// </summary>
        LowerPriority = 1 << 0,
        /// <summary>
        /// This setting will allow a subtree to abort itself when the decorator condition changes.
        /// </summary>
        Self = 1 << 1,
        /// <summary>
        /// This setting will allow a subtree to abort itself and lower-priority trees when the decorator condition changes.
        /// </summary>
        Both = LowerPriority | Self
    }
}