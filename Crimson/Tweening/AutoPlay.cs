using System;

namespace Crimson.Tweening
{
    [Flags]
    public enum AutoPlay
    {
        None = 0,
        Tweens = 1,
        Sequences = 2,
        All = Tweens | Sequences,
    }
}