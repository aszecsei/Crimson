using System;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public class Shaker : Component
    {
        public float Interval = .05f;

        private bool _on;
        public Action<Vector2>? OnShake;
        public bool RemoveOnFinish;
        public float Timer;
        public Vector2 Value;

        public Shaker(bool on = true, Action<Vector2>? onShake = null)
            : base(true, false)
        {
            this._on = on;
            OnShake = onShake;
        }

        public Shaker(float time, bool removeOnFinish, Action<Vector2> onShake = null)
            : this(true, onShake)
        {
            Timer = time;
            RemoveOnFinish = removeOnFinish;
        }

        public bool On
        {
            get => _on;

            set
            {
                _on = value;
                if (!_on)
                {
                    Timer = 0;
                    if (Value != Vector2.Zero)
                    {
                        Value = Vector2.Zero;
                        OnShake?.Invoke(Vector2.Zero);
                    }
                }
            }
        }

        public Shaker ShakeFor(float seconds, bool removeOnFinish)
        {
            _on = true;
            Timer = seconds;
            RemoveOnFinish = removeOnFinish;

            return this;
        }

        public override void Update()
        {
            if (_on && Timer > 0)
            {
                Timer -= Time.DeltaTime;
                if (Timer <= 0)
                {
                    _on = false;
                    Value = Vector2.Zero;
                    OnShake?.Invoke(Vector2.Zero);
                    if (RemoveOnFinish)
                        RemoveSelf();
                    return;
                }
            }

            if (_on && Scene.OnInterval(Interval))
            {
                Value = Utils.Random.ShakeVector();
                OnShake?.Invoke(Value);
            }
        }
    }
}