#region Using Statements

using System;
using Microsoft.Xna.Framework;

#endregion

namespace Crimson
{
    public static class Time
    {
        /// <summary>
        /// The coefficient used for SmoothDeltaTime smoothing. This is, in essence, the cutoff frequency coefficient of
        /// a simple "one-pole" low pass filter.
        /// </summary>
        public const float SMOOTH_DELTA_TIME_COEFFICIENT = 0.2f;
        
        /// <summary>
        /// The scale at which time passes. This can be used for slow motion effects.
        /// </summary>
        /// <remarks>
        /// When TimeRate is 1.0 time passes as fast as realtime. When TimeRate is 0.5 time passes 2x slower than realtime.
        /// When TimeRate is set to zero the game is basically paused if all your functions are frame rate independent.
        /// </remarks>
        public static float TimeRate = 1f;
        /// <summary>
        /// The length of time remaining (in seconds) for a "freeze"; if this value is > 0, no update functions will
        /// be called until it ticks down to zero. This is unaffected by any TimeScale effects.
        /// </summary>
        /// See also <seealso cref="Freeze"/>
        public static float FreezeTimer;
        /// <summary>
        /// The current FPS of the game.
        /// </summary>
        public static int FPS { get; private set; }
        /// <summary>
        /// Callback that runs every frame with the latest FPS value.
        /// </summary>
        public static Action<int> onFPSUpdate;
        /// <summary>
        /// The completion time, in seconds, since the last frame.
        /// </summary>
        public static float DeltaTime { get; private set; }
        /// <summary>
        /// The TimeRate-independent interval, in seconds, from the last frame to the current one.
        /// </summary>
        public static float RawDeltaTime { get; private set; }
        /// <summary>
        /// The smoothed completion time, in seconds, since the last frame.
        /// </summary>
        /// <remarks>
        /// The smoothing is applied before any TimeScale effects, thus changes to the TimeScale will not negatively
        /// impact the values here.
        /// </remarks>
        public static float SmoothDeltaTime { get; private set; }
        /// <summary>
        /// The TimeRate-independent smoothed interval, in seconds, from the last frame to the current one.
        /// </summary>
        public static float RawSmoothDeltaTime { get; private set; }

        /// <summary>
        /// The total elapsed time, in seconds.
        /// </summary>
        public static float TotalTime { get; private set; } = 0f;

        /// <summary>
        /// The TimeRate-independent total elapsed time, in seconds.
        /// </summary>
        public static float RawTotalTime { get; private set; } = 0f;
        
        private static TimeSpan counterElapsed = TimeSpan.Zero;
        private static int fpsCounter;

        public static void Update(GameTime gameTime)
        {
            RawDeltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            RawTotalTime += RawDeltaTime;
            DeltaTime = RawDeltaTime * TimeRate;
            TotalTime += DeltaTime;
            RawSmoothDeltaTime = SMOOTH_DELTA_TIME_COEFFICIENT * RawDeltaTime +
                                 (1 - SMOOTH_DELTA_TIME_COEFFICIENT) * RawSmoothDeltaTime;
            SmoothDeltaTime = RawSmoothDeltaTime * TimeRate;
        }

        public static void OnDraw(GameTime gameTime)
        {
            fpsCounter++;
            counterElapsed += gameTime.ElapsedGameTime;
            if (counterElapsed >= TimeSpan.FromSeconds(1))
            {
                FPS = fpsCounter;
                fpsCounter = 0;
                counterElapsed -= TimeSpan.FromSeconds(1);

                onFPSUpdate.Invoke(FPS);
            }
        }

        public static void Freeze(float time)
        {
            FreezeTimer = time;
        }
    }
}