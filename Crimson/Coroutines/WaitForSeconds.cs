namespace Crimson
{
    public class WaitForSeconds : YieldInstruction
    {
        internal float Duration;

        public WaitForSeconds(float duration)
        {
            Duration = duration;
        }
    }
}