using System;
using System.Collections.Generic;
using Crimson.Tweening.Plugins;
using Crimson.Tweening.Plugins.Options;
using Microsoft.Xna.Framework;

namespace Crimson.Tweening
{
    public class TweenSubsystem : Subsystem
    {
        /// <summary>
        /// If <c>UseSmoothDeltaTime</c> is <c>true</c>, indicates the
        /// max time that will be considered as elapsed in case of
        /// time-independent animations.
        /// </summary>
        public static float MaxSmoothUnscaledTime = 0.15f;
        /// <summary>
        /// Global time scale applied to all animations, both regular and independent.
        /// </summary>
        public static float TimeScale = 1f;
        /// <summary>
        /// If true, will use Time.SmoothDeltaTime instead of Time.DeltaTime for
        /// UpdateType.Normal and UpdateType.Late animations (unless they're set as
        /// time-scale independent, in which case a value between the last timestep
        /// and MaxSmoothUnscaledTime will be used instead). Setting this to true will
        /// lead to smoother animations.
        /// </summary>
        public static bool UseSmoothDeltaTime = true;
        /// <summary>
        /// Default auto-kill behavior applied to all newly created animations.
        /// </summary>
        public static bool DefaultAutoKill = true;
        /// <summary>
        /// Default ease function applied to all newly created animations.
        /// </summary>
        public static Easer DefaultEaser = Ease.OutQuad.GetEaser();
        /// <summary>
        /// Default loop type for all newly created animations.
        /// </summary>
        public static LoopType DefaultLoopType = LoopType.Restart;
        /// <summary>
        /// Default update type for all newly created animations.
        /// </summary>
        public static UpdateType DefaultUpdateType = UpdateType.Normal;
        /// <summary>
        /// Whether or not newly created animations can be re-used by default.
        /// </summary>
        public static bool DefaultRecyclable = false;
        /// <summary>
        /// Whether or not newly created animations are time-scale independent by default.
        /// </summary>
        public static bool DefaultTimeScaleIndependent = false;
        /// <summary>
        /// Default auto-play behavior for newly created animations.
        /// </summary>
        public static AutoPlay DefaultAutoPlay = AutoPlay.All;

        private static List<Animation> s_pausedAnimations = new List<Animation>();

        /// <summary>
        /// In order to be faster, Tweening limits the max amount of active tweens you can
        /// have. If you go beyond that limit don't worry: it is automatically increased.
        /// Still, if you already know you'll need more (or less) than the default max
        /// Tweeners/Sequences (which is 200 Tweeners and 50 Sequences) you can set the
        /// capacity at startup and avoid hiccups when it's raised automatically.
        /// </summary>
        public static void SetAnimationsCapacity(int maxTweens, int maxSequences)
        {
            AnimManager.SetCapacities(maxTweens, maxSequences);
        }

        /// <summary>
        /// Kills all tweens, and clears all cached tween pools.
        /// </summary>
        public static void Clear(bool isApplicationQuitting = false)
        {
            AnimManager.PurgeAll(isApplicationQuitting);
        }

        public static void Pause()
        {
            foreach ( var anim in AnimManager.ActiveAnimations )
            {
                anim?.Pause();
                s_pausedAnimations.Add(anim);
            }
        }

        public static void Resume()
        {
            foreach ( var anim in s_pausedAnimations )
            {
                anim?.Play();
            }
            s_pausedAnimations.Clear();
        }

        /// <summary>
        /// Clears all cached tween pools.
        /// </summary>
        public static void ClearCachedTweens()
        {
            AnimManager.PurgePools();
        }

        /// <summary>
        /// Checks all active animations to find and eventually remove invalid ones (usually because
        /// their targets became null).
        /// </summary>
        /// <returns>the total number of invalid animations found and removed.</returns>
        public static int Validate()
        {
            return AnimManager.Validate();
        }

        #region Tween TO

        public static TweenCore<float, FloatOptions>? To(Getter<float> getter, Setter<float> setter, float endValue, float duration) =>
            ApplyTo<float, FloatOptions>(getter, setter, endValue, duration);
        public static TweenCore<double, NoOptions>? To(Getter<double> getter, Setter<double> setter, double endValue, float duration) =>
            ApplyTo<double, NoOptions>(getter, setter, endValue, duration);
        public static TweenCore<int, NoOptions>? To(Getter<int> getter, Setter<int> setter, int endValue, float duration) =>
            ApplyTo<int, NoOptions>(getter, setter, endValue, duration);
        public static TweenCore<uint, NoOptions>? To(Getter<uint> getter, Setter<uint> setter, uint endValue, float duration) =>
            ApplyTo<uint, NoOptions>(getter, setter, endValue, duration);
        public static TweenCore<long, NoOptions>? To(Getter<long> getter, Setter<long> setter, long endValue, float duration) =>
            ApplyTo<long, NoOptions>(getter, setter, endValue, duration);
        public static TweenCore<ulong, NoOptions>? To(Getter<ulong> getter, Setter<ulong> setter, ulong endValue, float duration) =>
            ApplyTo<ulong, NoOptions>(getter, setter, endValue, duration);
        public static TweenCore<string, StringOptions>? To(Getter<string> getter, Setter<string> setter, string endValue, float duration) =>
            ApplyTo<string, StringOptions>(getter, setter, endValue, duration);
        public static TweenCore<Vector2, VectorOptions>? To(Getter<Vector2> getter, Setter<Vector2> setter, Vector2 endValue, float duration) =>
            ApplyTo<Vector2, VectorOptions>(getter, setter, endValue, duration);
        public static TweenCore<Vector3, VectorOptions>? To(Getter<Vector3> getter, Setter<Vector3> setter, Vector3 endValue, float duration) =>
            ApplyTo<Vector3, VectorOptions>(getter, setter, endValue, duration);
        public static TweenCore<Vector4, VectorOptions>? To(Getter<Vector4> getter, Setter<Vector4> setter, Vector4 endValue, float duration) =>
            ApplyTo<Vector4, VectorOptions>(getter, setter, endValue, duration);
        public static TweenCore<Quaternion, QuaternionOptions>? To(Getter<Quaternion> getter, Setter<Quaternion> setter, Quaternion endValue, float duration) =>
            ApplyTo<Quaternion, QuaternionOptions>(getter, setter, endValue, duration);
        public static TweenCore<Color, ColorOptions>? To(Getter<Color> getter, Setter<Color> setter, Color endValue, float duration) =>
            ApplyTo<Color, ColorOptions>(getter, setter, endValue, duration);
        public static TweenCore<Rectangle, NoOptions>? To(Getter<Rectangle> getter, Setter<Rectangle> setter, Rectangle endValue, float duration) =>
            ApplyTo<Rectangle, NoOptions>(getter, setter, endValue, duration);
        public static TweenCore<FRect, FRectOptions>? To(Getter<FRect> getter, Setter<FRect> setter, FRect endValue, float duration) =>
            ApplyTo<FRect, FRectOptions>(getter, setter, endValue, duration);

        public static TweenCore<T, TPlugOptions> To<T, TPlugOptions>(ITweenPlugin<T, TPlugOptions> plugin,
            Getter<T> getter, Setter<T> setter, T endValue, float duration)
            where TPlugOptions : struct, IPlugOptions
        {
            return ApplyTo(getter, setter, endValue, duration, plugin);
        }

        #endregion

        #region Sequence

        public static Sequence Sequence()
        {
            return AnimManager.GetSequence();
        }

        #endregion

        #region Play Operations



        #endregion

        #region Private Methods

        private static TweenCore<T, TPlugOptions>? ApplyTo<T, TPlugOptions>(Getter<T> getter, Setter<T> setter, T endValue, float duration, ITweenPlugin<T, TPlugOptions>? plugin = null)
        where TPlugOptions : struct, IPlugOptions
        {
            TweenCore<T, TPlugOptions> tween = AnimManager.GetTween<T, TPlugOptions>();

            bool setupSuccessful = Tween.Setup(tween, getter, setter, endValue, duration, plugin);
            if (!setupSuccessful)
            {
                AnimManager.Despawn(tween);
                return null;
            }

            return tween;
        }

        #endregion

        #region Subsystem Methods

        protected internal override void Update()
        {
            base.Update();
            if (UseSmoothDeltaTime)
            {
                AnimManager.Update(UpdateType.Normal, Time.SmoothDeltaTime, Time.RawSmoothDeltaTime);
            }
            else
            {
                AnimManager.Update(UpdateType.Normal, Time.DeltaTime, Time.RawDeltaTime);
            }
        }

        protected internal override void AfterUpdate()
        {
            base.AfterUpdate();
            if (UseSmoothDeltaTime)
            {
                AnimManager.Update(UpdateType.Late, Time.SmoothDeltaTime, Time.RawSmoothDeltaTime);
            }
            else
            {
                AnimManager.Update(UpdateType.Late, Time.DeltaTime, Time.RawDeltaTime);
            }
        }

        #endregion
    }
}
