#region Using Statements

using System;
using Microsoft.Xna.Framework;

#endregion

namespace Crimson
{
    public static class Time
    {
        public delegate void OnFPSUpdate(int newFPS);

        public static float TimeRate = 1f;
        public static float FreezeTimer;
        public static int FPS;
        public static OnFPSUpdate onFPSUpdate;

        private static TimeSpan counterElapsed = TimeSpan.Zero;
        private static int fpsCounter;
        public static float DeltaTime { get; private set; }
        public static float RawDeltaTime { get; private set; }

        public static void Update(GameTime gameTime)
        {
            RawDeltaTime = (float) gameTime.ElapsedGameTime.TotalSeconds;
            DeltaTime = RawDeltaTime * TimeRate;
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