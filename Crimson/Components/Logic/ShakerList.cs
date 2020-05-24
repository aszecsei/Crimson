using System;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public class ShakerList : Component
    {
        public float Interval = .05f;

        private bool on;
        public Action<Vector2[]> OnShake;
        public bool RemoveOnFinish;
        public float Timer;
        public Vector2[] Values;

        public ShakerList(int length, bool on = true, Action<Vector2[]> onShake = null)
            : base(true, false)
        {
            Values = new Vector2[length];
            this.on = on;
            OnShake = onShake;
        }

        public ShakerList(int length, float time, bool removeOnFinish, Action<Vector2[]> onShake = null)
            : this(length, true, onShake)
        {
            Timer = time;
            RemoveOnFinish = removeOnFinish;
        }

        public bool On
        {
            get => on;

            set
            {
                on = value;
                if (!on)
                {
                    Timer = 0;
                    if (Values[0] != Vector2.Zero)
                    {
                        for (var i = 0; i < Values.Length; i++)
                            Values[i] = Vector2.Zero;
                        if (OnShake != null)
                            OnShake(Values);
                    }
                }
            }
        }

        public ShakerList ShakeFor(float seconds, bool removeOnFinish)
        {
            on = true;
            Timer = seconds;
            RemoveOnFinish = removeOnFinish;

            return this;
        }

        public override void Update()
        {
            if (on && Timer > 0)
            {
                Timer -= Time.DeltaTime;
                if (Timer <= 0)
                {
                    on = false;
                    for (var i = 0; i < Values.Length; i++)
                        Values[i] = Vector2.Zero;
                    if (OnShake != null)
                        OnShake(Values);
                    if (RemoveOnFinish)
                        RemoveSelf();
                    return;
                }
            }

            if (on && Scene.OnInterval(Interval))
            {
                for (var i = 0; i < Values.Length; i++)
                    Values[i] = Utils.Random.ShakeVector();
                if (OnShake != null)
                    OnShake(Values);
            }
        }
    }
}