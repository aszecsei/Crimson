using System;

namespace Crimson.UI
{
    /// <summary>
    /// Controls clipping of widgets.
    /// </summary>
    public enum WidgetClipping
    {
        /// <summary>
        /// This widget does not clip children, it and all children inherit
        /// the clipping area of the last widget that clipped.
        /// </summary>
        Inherit,
        /// <summary>
        /// This widget clips content to the bounds of the widget.
        /// </summary>
        ClipToBounds,
        /// <summary>
        /// This widget clips to its bounds when it's Desired Size is larger than the allocated geometry the widget is given.
        /// </summary>
        OnDemand,
    }
    
    public enum FlowDirectionPreference
    {
        /// <summary>
        /// Inherits the flow direction set by the parent widget.
        /// </summary>
        Inherit,
        /// <summary>
        /// Begins laying out widgets using the current culture's layout direction preference, flipping the directionality of flows.
        /// </summary>
        Culture,
        /// <summary>
        /// Forces a Left to Right layout flow.
        /// </summary>
        LeftToRight,
        /// <summary>
        /// Forces a Right to Left layout flow.
        /// </summary>
        RightToLeft,
    }

    public enum Visibility
    {
        /// <summary>
        /// Visible and hit-testable (can interact with cursor). Default value.
        /// </summary>
        Visible,
        /// <summary>
        /// Not visible and takes up no space in the layout (obviously not hit-testable).
        /// </summary>
        Collapsed,
        /// <summary>
        /// Not visible but occupies layout space (obviously not hit-testable).
        /// </summary>
        Hidden,
        /// <summary>
        /// Visible but not hit-testable (cannot interact with cursor) and children in the hierarchy (if any) are also not hit-testable.
        /// </summary>
        HitTestInvisible,
        /// <summary>
        /// Visible but not hit-testable (cannot interact with cursor) and doesn't affect hit-testing on children (if any).
        /// </summary>
        SelfHitTestInvisible
    }
}