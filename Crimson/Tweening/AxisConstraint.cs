using System;

namespace Crimson.Tweening
{
    /// <summary>
    /// What axis to animate in case of Vector tweens
    /// </summary>
    [Flags]
    public enum AxisConstraint
    {
        All = X | Y | Z | W,
        X = 1,
        Y = 2,
        Z = 4,
        W = 8
    }
}