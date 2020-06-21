namespace Crimson.Tweening
{
    public class TweenSystem : Subsystem
    {
        /// <summary>
        /// If <c>UseSmoothDeltaTime</c> is <c>true</c>, indicates the
        /// max time that will be considered as elapsed in case of
        /// time-independent tweens.
        /// </summary>
        public static float MaxSmoothUnscaledTime = 0.15f;
        /// <summary>
        /// Global time scale applied to all tweens, both regular and independent.
        /// </summary>
        public static float TimeScale = 1f;
        /// <summary>
        /// If true, will use Time.smoothDeltaTime instead of Time.DeltaTime for
        /// UpdateType.Normal and UpdateType.Late tweens (unless they're set as
        /// time-scale independent, in which case a value between the last timestep
        /// and maxSmoothUnscaledTime will be used instead). Setting this to true will
        /// lead to smoother animations.
        /// </summary>
        public static bool UseSmoothDeltaTime = false;
        /// <summary>
        /// In order to be faster, Tweening limits the max amount of active tweens you can
        /// have. If you go beyond that limit don't worry: it is automatically increased.
        /// Still, if you already know you'll need more (or less) than the default max
        /// Tweeners/Sequences (which is 200 Tweeners and 50 Sequences) you can set the
        /// capacity at startup and avoid hiccups when it's raised automatically.
        /// </summary>
        public static void SetTweensCapacity(int maxTweeners, int maxSequences)
        {
            
        }
        /// <summary>
        /// Default auto-kill behavior applied to all newly created tweens.
        /// </summary>
        public static bool DefaultAutoKill = true;
    }
}