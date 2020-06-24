namespace Crimson.Tweening
{
    public enum LoopType
    {
        /// <summary>
        /// When a loop ends it will restart from the beginning.
        /// </summary>
        Restart,
        /// <summary>
        /// When a loop ends it will play backwards until it completes another
        /// loop, then forward again, then backwards again, and so on.
        /// </summary>
        Yoyo,
    }
}