namespace Crimson.Tweening
{
    public enum UpdateType
    {
        /// <summary>
        /// Updates every frame during Update calls.
        /// </summary>
        Normal,
        /// <summary>
        /// Updates every frame during AfterUpdate calls.
        /// </summary>
        Late,
    }
}