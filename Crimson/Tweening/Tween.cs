using System;

namespace Crimson.Tweening
{
    /// <summary>
    /// Animates a single value.
    /// </summary>
    public abstract class Tweener : Tween
    {
        /// <summary>
        /// Changes the start value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences
        /// </summary>
        /// <param name="newStartValue">The new start value</param>
        /// <param name="newDuration">If bigger than 0, applies it as the new tween duration</param>
        public abstract Tweener ChangeStartValue(object newStartValue, float newDuration = -1);
        
        /// <summary>
        /// Changes the end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences
        /// </summary>
        /// <param name="newEndValue">The new end value</param>
        /// <param name="newDuration">If bigger than 0, applies it as the new tween duration</param>
        /// <param name="snapStartValue">If true the start value will become the current target's value,
        /// otherwise it will stay the same</param>
        public abstract Tweener ChangeEndValue(object newEndValue, float newDuration = -1, bool snapStartValue = false);
        
        /// <summary>
        /// Changes the end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences
        /// </summary>
        /// <param name="newEndValue">The new end value</param>
        /// <param name="snapStartValue">If true the start value will become the current target's value,
        /// otherwise it will stay the same</param>
        public Tweener ChangeEndValue(object newEndValue, bool snapStartValue)
        {
            return ChangeEndValue(newEndValue, -1, snapStartValue);
        }
        
        /// <summary>
        /// Changes the start and end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences
        /// </summary>
        /// <param name="newStartValue">The new start value</param>
        /// <param name="newEndValue">The new end value</param>
        /// <param name="newDuration">If bigger than 0, applies it as the new tween duration</param>
        public abstract Tweener ChangeValues(object newStartValue, object newEndValue, float newDuration = -1);
    }
}