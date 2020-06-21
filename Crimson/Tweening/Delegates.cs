namespace Crimson.Tweening
{
    /// <summary>
    /// Used in place of <c>System.Func</c>, which is not available
    /// in mscorlib.
    /// </summary>
    public delegate T Getter<out T>();
    /// <summary>
    /// Used in place of <c>System.Action</c>.
    /// </summary>
    public delegate void Setter<in T>(T newValue);
    /// <summary>
    /// Used for tween callbacks.
    /// </summary>
    public delegate void TweenCallback();
    /// <summary>
    /// Used for custom ease functions.
    /// </summary>
    public delegate float Easer(float t);
}