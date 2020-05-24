using System;

namespace Crimson.UI
{
    /// <summary>
    /// Describes the thickness of a frame around a rectangle.
    /// </summary>
    public struct Thickness : IEquatable<Thickness>
    {
        /// <summary>
        /// The width, in pixels, of the lower side of the bounding rectangle.
        /// </summary>
        public float Bottom;
        /// <summary>
        /// The width, in pixels, of the left side of the bounding rectangle.
        /// </summary>
        public float Left;
        /// <summary>
        /// The width, in pixels, of the right side of the bounding rectangle.
        /// </summary>
        public float Right;
        /// <summary>
        /// The width, in pixels, of the upper side of the bounding rectangle.
        /// </summary>
        public float Top;

        public bool Equals(Thickness other)
        {
            return Mathf.Approximately(Bottom, other.Bottom) &&
                   Mathf.Approximately(Left, other.Left) &&
                   Mathf.Approximately(Right, other.Right) &&
                   Mathf.Approximately(Top, other.Top);
        }
    }
}