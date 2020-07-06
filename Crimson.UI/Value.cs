namespace Crimson.UI
{
    public abstract class Value
    {
        public static readonly Value Zero = new FixedValue(0);
        public static readonly Value Infinity = new FixedValue(Mathf.INFINITY);

        public static readonly Value MinWidth = new MinWidthValue();
        public static readonly Value MinHeight = new MinHeightValue();
        public static readonly Value PrefWidth = new PrefWidthValue();
        public static readonly Value PrefHeight = new PrefHeightValue();
        public static readonly Value MaxWidth = new MaxWidthValue();
        public static readonly Value MaxHeight = new MaxHeightValue();
        
        public static Value Fixed(float val) => new FixedValue(val);
        public static Value PercentWidth(float percent) => new PercentWidthValue() { Percent = percent };
        public static Value PercentHeight(float percent) => new PercentHeightValue() { Percent = percent };

        public abstract float Get(Widget? context);

        private class FixedValue : Value
        {
            private readonly float _value;
            public FixedValue(float value) => _value = value;
            public override float Get(Widget? context) => _value;
        }

        private class MinWidthValue : Value
        {
            public override float Get(Widget? context) => context?.MinSize.Width ?? 0;
        }

        private class MinHeightValue : Value
        {
            public override float Get(Widget? context) => context?.MinSize.Height ?? 0;
        }
        
        private class PrefWidthValue : Value
        {
            public override float Get(Widget? context) => context?.PrefSize.Width ?? 0;
        }

        private class PrefHeightValue : Value
        {
            public override float Get(Widget? context) => context?.PrefSize.Height ?? 0;
        }
        
        private class MaxWidthValue : Value
        {
            public override float Get(Widget? context) => context?.MaxSize.Width ?? 0;
        }

        private class MaxHeightValue : Value
        {
            public override float Get(Widget? context) => context?.MaxSize.Height ?? 0;
        }

        private class PercentWidthValue : Value
        {
            public float Percent;
            public override float Get(Widget? context) => (context?.Geometry.Width ?? 0) * Percent;
        }
        
        private class PercentHeightValue : Value
        {
            public float Percent;
            public override float Get(Widget? context) => (context?.Geometry.Height ?? 0) * Percent;
        }
    }
}