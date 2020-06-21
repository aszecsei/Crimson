namespace Crimson
{
    public class WaitForSecondsRealtime : CustomYieldInstruction
    {
        private float _duration;

        public WaitForSecondsRealtime(float duration)
        {
            _duration = duration;
        }

        public override bool KeepWaiting
        {
            get
            {
                _duration -= Time.RawDeltaTime;
                return _duration > 0;
            }
        }
    }
}