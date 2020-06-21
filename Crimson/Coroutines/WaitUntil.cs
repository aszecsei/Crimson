using System;

namespace Crimson
{
    public class WaitUntil : CustomYieldInstruction
    {
        private readonly Func<bool> _predicate;

        public WaitUntil(Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public override bool KeepWaiting => !_predicate();
    }
}