namespace Crimson.UI
{
    /// <summary>
    /// A callback function you can register on UI events.
    /// </summary>
    /// <param name="target">The UI element that was the recipient of the event.</param>
    public delegate void EventCallback(object target);
    
    /// <summary>
    /// A collection of event callbacks.
    /// </summary>
    public struct UIEvents
    {
        /// <summary>
        /// The UI element was focused and a "select" mouse button, key,
        /// or gamepad button was pressed.
        /// </summary>
        public EventCallback OnSelectPress;
        /// <summary>
        /// The UI element was focused and a "select" mouse button, key,
        /// or gamepad button was released.
        /// </summary>
        public EventCallback OnSelectRelease;
        /// <summary>
        /// The UI element was hovered over with the mouse or selected
        /// via arrow keys or gamepad directional controls.
        /// </summary>
        public EventCallback OnGainFocus;
        /// <summary>
        /// The UI element stopped being hovered over with the mouse or deselected
        /// via arrow keys or gamepad directional controls.
        /// </summary>
        public EventCallback OnLoseFocus;
        /// <summary>
        /// The UI element's value was changed (used for widgets like text fields).
        /// </summary>
        public EventCallback OnValueChange;
        /// <summary>
        /// The UI element's visibility changed.
        /// </summary>
        public EventCallback OnVisibilityChange;
        /// <summary>
        /// The UI element was either enabled or disabled.
        /// </summary>
        public EventCallback OnEnabledChange;
    }
}