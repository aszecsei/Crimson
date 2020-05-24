namespace Crimson.UI
{
    public struct Navigation
    {
        /// <summary>
        /// Happens when the user presses the down arrow, joystick, or d-pad.
        /// </summary>
        public EventCallback Down;
        /// <summary>
        /// Happens when the user presses the left arrow, joystick, or d-pad.
        /// </summary>
        public EventCallback Left;
        /// <summary>
        /// Happens when the user presses Tab.
        /// </summary>
        public EventCallback Next;
        /// <summary>
        /// Happens when the user presses Shift+Tab.
        /// </summary>
        public EventCallback Previous;
        /// <summary>
        /// Happens when the user presses right arrow, joystick, or d-pad.
        /// </summary>
        public EventCallback Right;
        /// <summary>
        /// Happens when the user presses up arrow, joystick, or d-pad.
        /// </summary>
        public EventCallback Up;
    }
}