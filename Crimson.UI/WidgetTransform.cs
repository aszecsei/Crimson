using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    public class WidgetTransform
    {
        /// <summary>
        /// The angle in radians to rotate.
        /// </summary>
        public float Angle = 0f;
        /// <summary>
        /// The scale to apply to the widget.
        /// </summary>
        public Vector2 Scale = Vector2.One;
        /// <summary>
        /// The amount to shear the widget.
        /// </summary>
        public Vector2 Shear = Vector2.Zero;
        /// <summary>
        /// The amount to translate the widget.
        /// </summary>
        public Vector2 Translation = Vector2.Zero;
    }
}