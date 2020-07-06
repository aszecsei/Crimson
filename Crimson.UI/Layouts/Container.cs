using System;
using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    public class Container : Panel
    {
        public Align Align = Align.Center;
        public float FillX, FillY;

        public Value MaxHeight = Value.Infinity;
        public Value MaxWidth = Value.Infinity;
        public Value MinHeight = Value.MinHeight;
        public Value MinWidth = Value.MinWidth;

        public Value PadBottom = Value.Zero;
        public Value PadLeft = Value.Zero;
        public Value PadTop = Value.Zero;
        public Value PadRight = Value.Zero;

        public Value PrefHeight = Value.PrefHeight;
        public Value PrefWidth = Value.PrefWidth;
        
        public Container() {}
        public Container(Widget widget) => Widget = widget;

        public Widget? Widget
        {
            get => Count == 0 ? null : this[0];
            set
            {
                if (value == this)
                {
                    throw new Exception("Widget cannot be its own container!");
                }

                // If we were empty, just add the widget
                if (this.Empty && value != null)
                {
                    Add(value);
                    return;
                }
                
                // If we had the widget before, exit early
                if (value == this[0]) return;

                // Remove the old widget before adding the new one
                Remove(this[0]);
                
                if (value != null)
                    Add(value);
            }
        }

        public override Size MinSize => new Size(
            MinWidth.Get(Widget) + PadLeft.Get(this) + PadRight.Get(this),
            MinHeight.Get(Widget) + PadTop.Get(this) + PadBottom.Get(this));

        public override Size PrefSize => new Size(
            Mathf.Max(MinSize.Width, PrefWidth.Get(Widget) + PadLeft.Get(this) + PadRight.Get(this)),
            Mathf.Max(MinSize.Height, PrefHeight.Get(Widget) + PadTop.Get(this) + PadBottom.Get(this)));

        public override Size MaxSize
        {
            get
            {
                float w = MaxWidth.Get(Widget);
                if (w < Mathf.INFINITY)
                {
                    w += PadLeft.Get(this) + PadRight.Get(this);
                }

                float h = MaxHeight.Get(Widget);
                if (h < Mathf.INFINITY)
                {
                    h += PadTop.Get(this) + PadBottom.Get(this);
                }

                return new Size(w, h);
            }
        }

        public T? GetWidget<T>() where T : Widget => Widget as T;

        public override void Layout()
        {
            if (Widget == null) return;

            float padLeft = PadLeft.Get(this), padBottom = PadBottom.Get(this);
            float containerWidth = Width - padLeft - PadRight.Get(this);
            float containerHeight = Height - padBottom - PadTop.Get(this);
            float minWidth = MinWidth.Get(Widget), minHeight = MinHeight.Get(Widget);
            float prefWidth = PrefWidth.Get(Widget), prefHeight = PrefHeight.Get(Widget);
            float maxWidth = MaxWidth.Get(Widget), maxHeight = MaxHeight.Get(Widget);

            float width;
            if (FillX > 0) width = containerWidth * FillX;
            else width = Mathf.Min(prefWidth, containerWidth);
            width = Mathf.Clamp(width, minWidth, maxWidth);

            float height;
            if (FillY > 0) height = containerHeight * FillY;
            else height = Mathf.Min(prefHeight, containerHeight);
            height = Mathf.Clamp(height, minHeight, maxHeight);

            Size closestSize = ClosestAvailableSize(Widget, new Size(width, height));
            width = closestSize.Width;
            height = closestSize.Height;

            float x = padLeft;
            if (Align.HasFlag(Align.Right)) x += containerWidth - width;
            else if (Align.HasFlag(Align.CenterX)) x += (containerWidth - width) / 2;

            float y = padBottom;
            if (Align.HasFlag(Align.Top)) y += containerHeight - height;
            else if (Align.HasFlag(Align.CenterY)) y += (containerHeight - height) / 2;
            
            Widget.Geometry = new Rect(Geometry.X + x, Geometry.Y + y, width, height);
        }

        public override bool CanSupportFocus => false;

        public override void DebugRender()
        {
            Draw.HollowRect(Geometry.Position, Geometry.Size.Width, Geometry.Size.Height, Color.Blue);
            Widget?.DebugRender();
        }
    }
}