using System;
using Crimson.Tweening.Plugins;
using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening
{
    /// <summary>
    /// Animates a single value.
    /// </summary>
    public abstract class Tween : Animation
    {
        internal bool HasManuallySetStartValue;

        internal Tween() {}
        
        /// <summary>
        /// Changes the start value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences
        /// </summary>
        /// <param name="newStartValue">The new start value</param>
        /// <param name="newDuration">If bigger than 0, applies it as the new tween duration</param>
        public abstract Tween ChangeStartValue(object newStartValue, float newDuration = -1);
        
        /// <summary>
        /// Changes the end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences
        /// </summary>
        /// <param name="newEndValue">The new end value</param>
        /// <param name="newDuration">If bigger than 0, applies it as the new tween duration</param>
        /// <param name="snapStartValue">If true the start value will become the current target's value,
        /// otherwise it will stay the same</param>
        public abstract Tween ChangeEndValue(object newEndValue, float newDuration = -1, bool snapStartValue = false);
        
        /// <summary>
        /// Changes the end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences
        /// </summary>
        /// <param name="newEndValue">The new end value</param>
        /// <param name="snapStartValue">If true the start value will become the current target's value,
        /// otherwise it will stay the same</param>
        public Tween ChangeEndValue(object newEndValue, bool snapStartValue)
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
        public abstract Tween ChangeValues(object newStartValue, object newEndValue, float newDuration = -1);

        #region Internal Methods

        /// <summary>
        /// Called when spawning/crating a new Tween. Returns true if the setup is successful.
        /// </summary>
        internal static bool Setup<T, TPlugOptions>(TweenCore<T, TPlugOptions> tween, Getter<T> getter, Setter<T> setter, T endValue, float duration, ITweenPlugin<T, TPlugOptions>? plugin)
            where TPlugOptions : struct, IPlugOptions
        {
            if (plugin != null) tween.TweenPlugin = plugin;
            else
            {
                var plg = PluginsManager.GetDefaultPlugin<T, TPlugOptions>();
                if (plg == null)
                {
                    // No suitable plugin found. Fail out.
                    Utils.Log("No suitable plugin found for type {0}", typeof(T));
                    return false;
                }
                tween.TweenPlugin = plg;
            }

            tween.Getter = getter;
            tween.Setter = setter;
            tween.EndValue = endValue;
            tween.BaseDuration = duration;
            
            // Defaults
            tween.AutoKill = TweenSubsystem.DefaultAutoKill;
            tween.Recyclable = TweenSubsystem.DefaultRecyclable;
            tween.Easer = TweenSubsystem.DefaultEaser;
            tween.LoopType = TweenSubsystem.DefaultLoopType;
            tween.IsPlaying = TweenSubsystem.DefaultAutoPlay.HasFlag(AutoPlay.Tweens);
            tween.UpdateType = TweenSubsystem.DefaultUpdateType;
            tween.IsTimeScaleIndependent = TweenSubsystem.DefaultTimeScaleIndependent;

            return true;
        }

        internal static float DoUpdateDelay<T, TPlugOptions>(TweenCore<T, TPlugOptions> t, float elapsed)
            where TPlugOptions : struct, IPlugOptions
        {
            float tweenDelay = t.Delay;
            if (elapsed > tweenDelay)
            {
                // Delay complete
                t.ElapsedDelay = tweenDelay;
                return elapsed - tweenDelay;
            }

            t.ElapsedDelay = elapsed;
            return 0;
        }

        internal static bool DoStartup<T, TPlugOptions>(TweenCore<T, TPlugOptions> t)
            where TPlugOptions : struct, IPlugOptions
        {
            t.StartupDone = true;

            if (!t.HasManuallySetStartValue)
            {
                try
                {
                    t.StartValue = t.Getter!();
                }
                catch
                {
                    return false;
                }
            }
            
            return true;
        }

        internal static TweenCore<T, TPlugOptions> DoChangeStartValue<T, TPlugOptions>(
            TweenCore<T, TPlugOptions> t, T newStartValue, float newDuration)
            where TPlugOptions : struct, IPlugOptions
        {
            t.HasManuallySetStartValue = true;
            t.StartValue = newStartValue;

            if (newDuration > 0)
            {
                t.BaseDuration = newDuration;
            }
            
            // Force rewind
            DoGoto(t, 0, 0, false);

            return t;
        }
        
        internal static TweenCore<T, TPlugOptions> DoChangeEndValue<T, TPlugOptions>(
            TweenCore<T, TPlugOptions> t, T newEndValue, float newDuration, bool snapStartValue)
            where TPlugOptions : struct, IPlugOptions
        {
            t.EndValue = newEndValue;

            if (t.StartupDone && snapStartValue)
            {
                t.StartValue = t.Getter!();
            }

            if (newDuration > 0)
            {
                t.BaseDuration = newDuration;
            }
            
            // Force rewind
            DoGoto(t, 0, 0, false);

            return t;
        }
        
        internal static TweenCore<T, TPlugOptions> DoChangeValues<T, TPlugOptions>(
            TweenCore<T, TPlugOptions> t, T newStartValue, T newEndValue, float newDuration)
            where TPlugOptions : struct, IPlugOptions
        {
            t.HasManuallySetStartValue = true;
            t.StartValue = newStartValue;
            t.EndValue = newEndValue;

            if (newDuration > 0)
            {
                t.BaseDuration = newDuration;
            }
            
            // Force rewind
            DoGoto(t, 0, 0, false);

            return t;
        }
        
        #endregion
    }
}