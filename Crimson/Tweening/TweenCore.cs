using System;
using Crimson.Tweening.Plugins;
using Crimson.Tweening.Plugins.Options;

namespace Crimson.Tweening
{
    public sealed class TweenCore<T, TPlugOptions> : Tween where TPlugOptions : struct, IPlugOptions
    {
        public T StartValue;
        public T EndValue;
        public TPlugOptions PlugOptions;
        public Getter<T>? Getter;
        public Setter<T>? Setter;
        internal ITweenPlugin<T, TPlugOptions> TweenPlugin;

        internal TweenCore()
        {
            Type = typeof(T);
            OptionsType = typeof(TPlugOptions);
            AnimationType = AnimationType.Tween;
            Reset();
        }
        
        public override Tween ChangeStartValue(object newStartValue, float newDuration = -1)
        {
            if (IsSequenced)
            {
                Utils.Log("You cannot change the values of a tween contained inside a Sequence");
                return this;
            }

            Type valT = newStartValue.GetType();
            if (valT != Type)
            {
                Utils.Log("ChangeStartValue: incorrect newStartValue type (is " + valT + ", should be " + Type + ")");
                return this;
            }

            return DoChangeStartValue(this, (T)newStartValue, newDuration);
        }

        public override Tween ChangeEndValue(object newEndValue, float newDuration = -1, bool snapStartValue = false)
        {
            if (IsSequenced)
            {
                Utils.Log("You cannot change the values of a tween contained inside a Sequence");
                return this;
            }

            Type valT = newEndValue.GetType();
            if (valT != Type)
            {
                Utils.Log("ChangeEndValue: incorrect newEndValue type (is " + valT + ", should be " + Type + ")");
                return this;
            }

            return DoChangeEndValue(this, (T)newEndValue, newDuration, snapStartValue);
        }

        public override Tween ChangeValues(object newStartValue, object newEndValue, float newDuration = -1)
        {
            if (IsSequenced)
            {
                Utils.Log("You cannot change the values of a tween contained inside a Sequence");
                return this;
            }
            
            Type valT0 = newStartValue.GetType();
            if (valT0 != Type)
            {
                Utils.Log("ChangeValues: incorrect newStartValue type (is " + valT0 + ", should be " + Type + ")");
                return this;
            }

            Type valT1 = newEndValue.GetType();
            if (valT1 != Type)
            {
                Utils.Log("ChangeValues: incorrect newEndValue type (is " + valT1 + ", should be " + Type + ")");
                return this;
            }

            return DoChangeValues(this, (T)newStartValue, (T)newEndValue, newDuration);
        }

        internal override void Reset()
        {
            base.Reset();
            
            TweenPlugin?.Reset(this);
            PlugOptions.Reset();

            Getter = null;
            Setter = null;
            HasManuallySetStartValue = false;
        }

        internal override bool Validate()
        {
            try
            {
                Getter();
            }
            catch
            {
                return false;
            }

            return true;
        }

        internal override float UpdateDelay(float elapsed)
        {
            return DoUpdateDelay(this, elapsed);
        }

        internal override bool Startup()
        {
            return DoStartup(this);
        }

        internal override bool ApplyAnimation(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition)
        {
            float updatePosition = useInversePosition ? BaseDuration - Position : Position;
            try
            {
                TweenPlugin.EvaluateAndApply(PlugOptions, this, Getter, Setter, updatePosition, BaseDuration,
                    StartValue, EndValue);
            }
            catch
            {
                return true;
            }

            return false;
        }
    }
}