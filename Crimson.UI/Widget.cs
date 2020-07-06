using Crimson.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Crimson.UI
{
    /// <summary>
    /// Abstract base class for Crimson.UI widgets
    /// </summary>
    /// <remarks>
    /// <para>
    /// DO NOT INHERIT DIRECTLY FROM WIDGET!
    /// </para>
    /// <para>
    /// Instead consider inheriting from LeafWidget or Panel, which represent
    /// intended use cases and provide a more succinct set of methods to override.
    /// </para>
    /// <para>
    /// Widget is the base class for all interactive Crimson.UI entities, and is
    /// thus fairly complex.
    /// </para>
    /// </remarks>
    public abstract class Widget
    {
        private Rect _geometry;
        private bool _enabled = true;
        private bool _focused = false;
        private bool _selected = false;
        private Visibility _visibility = Visibility.Visible;

        /// <summary>
        /// Whether or not this widget is enabled.
        /// </summary>
        public virtual bool Enabled
        {
            get => _enabled && (Parent == null || Parent.Enabled);
            set
            {
                if (_enabled == value) return;
                
                _enabled = value;
                if (_enabled && Parent != null && !Parent.Enabled)
                {
                    Parent.Enabled = true;
                }
                UIEvents.OnEnabledChange?.Invoke(this);
            }
        }

        /// <summary>
        /// Whether or not the widget is in focus.
        /// </summary>
        public virtual bool Focused
        {
            get => _focused;
            set
            {
                if (_focused == value) return;
                
                // If we can't support focus, fail early.
                if (!CanSupportFocus && value) return;
                
                _focused = value;
                if (_focused) UIEvents.OnGainFocus?.Invoke(this);
                else          UIEvents.OnLoseFocus?.Invoke(this);
            }
        }

        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value) return;
                
                _selected = value;
                if (_selected) UIEvents.OnSelectPress?.Invoke(this);
                else           UIEvents.OnSelectRelease?.Invoke(this);
            }
        }

        /// <summary>
        /// The visibility of the widget.
        /// </summary>
        public virtual Visibility Visibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                UIEvents.OnVisibilityChange?.Invoke(this);
            }
        }

        /// <summary>
        /// The rectangle containing the widget. Set by <see cref="Layout"/>.
        /// </summary>
        public Rect Geometry
        {
            get => _geometry;
            set
            {
                if (_geometry != value)
                {
                    _geometry = value;
                    InvalidateJustThis();
                }
            }
        }
        /// <summary>
        /// The width of the widget.
        /// </summary>
        public float Width => Geometry.Width;
        /// <summary>
        /// The height of the widget.
        /// </summary>
        public float Height => Geometry.Height;
        /// <summary>
        /// Can the widget ever support children? This will be false on
        /// LeafWidgets; rather than setting this directly, you should probably
        /// inherit from LeafWidget or Panel.
        /// </summary>
        public abstract bool CanHaveChildren { get; }
        /// <summary>
        /// Can the widget ever support keyboard focus?
        /// </summary>
        public abstract bool CanSupportFocus { get; }
        /// <summary>
        /// Is this widget hovered?
        /// </summary>
        public bool IsHovered { get; protected set; }
        /// <summary>
        /// Controls the clipping behavior of this widget.
        /// </summary>
        public WidgetClipping Clipping = WidgetClipping.Inherit;
        /// <summary>
        /// The cursor to show when the mouse is over the widget.
        /// </summary>
        public MouseCursor Cursor = MouseCursor.Arrow;
        /// <summary>
        /// Allows you to set a new flow direction
        /// </summary>
        public FlowDirectionPreference FlowDirectionPreference = FlowDirectionPreference.Inherit;
        /// <summary>
        /// Users can create custom navigation rules for this widget.
        /// </summary>
        public Navigation Navigation = new Navigation();
        /// <summary>
        /// UI events that may occur to the widget.
        /// </summary>
        public UIEvents UIEvents = new UIEvents();
        /// <summary>
        /// The opacity of the widget.
        /// </summary>
        public float RenderOpacity = 1f;
        /// <summary>
        /// The render transform of the widget allows for arbitrary 2D transforms
        /// to be applied to the widget.
        /// </summary>
        public WidgetTransform RenderTransform = new WidgetTransform();
        /// <summary>
        /// The render transform pivot controls the location about which transforms
        /// are applied.
        /// </summary>
        public Vector2 RenderTransformPivot;
        /// <summary>
        /// The parent of the widget.
        /// </summary>
        public Widget? Parent;
        /// <summary>
        /// The widget's minimum size.
        /// </summary>
        public virtual Size MinSize => Size.Zero;
        /// <summary>
        /// The widget's requested size.
        /// </summary>
        public abstract Size PrefSize { get; }
        /// <summary>
        /// The widget's maximum size.
        /// </summary>
        public virtual Size MaxSize => Size.Infinite;
        protected bool Dirty { get; private set; }
        /// <summary>
        /// Mark that the widget's layout has been invalidated; this might be due to size changes or other animations.
        ///
        /// Note that this also dirties all parent widgets.
        /// </summary>
        public virtual void Invalidate()
        {
            Widget element = this;
            while (element.Parent != null)
            {
                element.Dirty = true;
                element = element.Parent;
            }

            element.Dirty = true;
        }
        /// <summary>
        /// Invalidate the current widget, but no others.
        /// </summary>
        public void InvalidateJustThis() => Dirty = true;
        /// <summary>
        /// Check the widget; if it is dirty, run a layout.
        /// </summary>
        public virtual void Validate()
        {
            if (!Dirty)
                return;

            Dirty = false;
            Layout();
        }
        /// <summary>
        /// Layout the widget. The default behavior is to do nothing.
        /// </summary>
        public virtual void Layout() {}
        /// <summary>
        /// Update the widget.
        /// </summary>
        public virtual void Update()
        {
            if (!Enabled || Visibility != Visibility.Visible)
            {
                Selected = false;
                Focused = false;
            }
            
            Validate();

            // Determine widget state
            Vector2 mousePos = CInput.mouseData.RawPosition;
            if (Visibility == Visibility.Visible && CanSupportFocus)
            {
                if (Hit(mousePos) == this)
                {
                    Focused = true;
                    IsHovered = true;
                    Selected = CInput.mouseData.CheckLeftButton;
                }
            }
        }

        /// <summary>
        /// Render the widget on-screen. The default behavior is to do nothing.
        ///
        /// If this method is overriden, the super method or <see cref="Validate"/>
        /// should be called to ensure the widget is laid out.
        /// </summary>
        public virtual void Render(float parentAlpha)
        {
            Validate();
        }
        /// <summary>
        /// Perform a debug render of the widget. The default behavior is to draw a box around the widget.
        /// </summary>
        public virtual void DebugRender()
        {
            Draw.HollowRect(Geometry.X, Geometry.Y, Geometry.Width, Geometry.Height, Color.White);
        }

        public virtual Widget? Hit(Vector2 point)
        {
            if (!Enabled || Visibility != Visibility.Visible)
            {
                return null;
            }

            if (Geometry.Contains(point))
            {
                return this;
            }

            return null;
        }
    }
}