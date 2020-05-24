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
        public abstract bool IsHovered { get; protected set; }
        /// <summary>
        /// Controls the clipping behavior of this widget.
        /// </summary>
        public WidgetClipping Clipping = WidgetClipping.Inherit;
        /// <summary>
        /// The cursor to show when the mouse is over the widget.
        /// </summary>
        public MouseCursor Cursor = MouseCursor.Arrow;
        /// <summary>
        /// Whether or not this widget is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Allows you to set a new flow direction
        /// </summary>
        public FlowDirectionPreference FlowDirectionPreference = FlowDirectionPreference.Inherit;
        /// <summary>
        /// Users can create custom navigation rules for this widget.
        /// </summary>
        public Navigation? Navigation = null;
        /// <summary>
        /// The opacity of the widget.
        /// </summary>
        public float RenderOpacity = 1f;
        /// <summary>
        /// The render transform of the widget allows for arbitrary 2D transforms
        /// to be applied to the widget.
        /// </summary>
        public WidgetTransform RenderTransform;
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
        /// The visibility of the widget.
        /// </summary>
        public Visibility Visibility = Visibility.Visible;
        
        
        public abstract Size MinSize { get; }
        
        public abstract Size PrefSize { get; }
        
        public abstract Size MaxSize { get; }
    }
}