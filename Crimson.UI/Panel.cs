using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    public abstract class Panel : Widget, IEnumerable<Widget>
    {
        public override bool CanHaveChildren => true;
        
        private readonly List<Widget> _children;

        protected Panel(params Widget[] children) => _children = new List<Widget>(children);

        public IReadOnlyList<Widget> Children => _children;

        public override Visibility Visibility
        {
            get => base.Visibility;
            set
            {
                base.Visibility = value;
                foreach (Widget c in _children)
                {
                    c.Visibility = value;
                }
            }
        }

        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                foreach (Widget c in _children)
                {
                    c.Enabled = value;
                }
            }
        }

        public Widget this[int index]
        {
            get => _children[index];
            set
            {
                _children[index].Parent = null;
                _children[index] = value;
                _children[index].Parent = this;
                Invalidate();
            }
        }

        public int Count => _children.Count;

        public bool Empty => _children.Count == 0;

        public override Widget? Hit(Vector2 point)
        {
            if (!Enabled) return null;
            if (!Geometry.Contains(point)) return null;
            if (Visibility != Visibility.Visible && Visibility != Visibility.SelfHitTestInvisible)
            {
                return null;
            }

            foreach (Widget child in _children)
            {
                Widget? hit = child.Hit(point);
                if (hit != null)
                {
                    return hit;
                }
            }

            return base.Hit(point);
        }

        public T Add<T>(T child) where T : Widget
        {
            _children.Add(child);
            child.Parent = this;
            Invalidate();
            return child;
        }

        public bool Remove<T>(T child) where T : Widget
        {
            if (!_children.Contains(child))
            {
                return false;
            }

            _children.Remove(child);
            child.Parent = null;
            Invalidate();
            return true;
        }

        public void Clear()
        {
            foreach (Widget c in _children)
            {
                c.Parent = null;
            }
            _children.Clear();
            Invalidate();
        }

        public override void Update()
        {
            base.Update();

            foreach (Widget c in _children)
            {
                c.Update();
            }
        }

        public override void Render(float parentAlpha)
        {
            base.Render(parentAlpha);
            
            // Sometimes we might need another layout
            Validate();
            
            foreach (Widget c in _children)
            {
                c.Render(parentAlpha * RenderOpacity);
            }
        }

        public override void DebugRender()
        {
            base.DebugRender();

            foreach (Widget c in _children)
            {
                c.DebugRender();
            }
        }

        /// <summary>
        /// Returns a size that satisfies all size constraints on <c>widget</c> and
        /// that is as close as possible to <c>size</c>.
        /// </summary>
        public static Size ClosestAvailableSize(Widget widget, Size size)
        {
            if (widget.Geometry == null) return size;

            Size result = size.BoundedTo(widget.MaxSize);
            result = result.ExpandedTo(widget.MinSize);

            return result;
        }

        public IEnumerator<Widget> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _children.GetEnumerator();
        }
    }
}