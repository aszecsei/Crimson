namespace Crimson
{
    public class CustomYieldInstruction : YieldInstruction
    {
        public virtual bool KeepWaiting => false;
    }
}