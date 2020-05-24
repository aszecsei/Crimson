namespace Crimson.Input
{
    public abstract class VirtualInput
    {
        public enum OverlapBehaviors
        {
            CancelOut,
            TakeOlder,
            TakeNewer
        }

        public enum ThresholdModes
        {
            LargerThan,
            LessThan,
            EqualTo
        }

        public VirtualInput()
        {
            CInput.VirtualInputs.Add(this);
        }

        public void Deregister()
        {
            CInput.VirtualInputs.Remove(this);
        }

        public abstract void Update();
    }

    public abstract class VirtualInputNode
    {
        public virtual void Update()
        {
        }
    }
}