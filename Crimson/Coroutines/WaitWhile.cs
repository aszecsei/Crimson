using System;

namespace Crimson
{
    public class WaitWhile : CustomYieldInstruction
    {
        private readonly Func<bool> _predicate;

        public WaitWhile(Func<bool> predicate)
        {
            _predicate = predicate;
        }

        public override bool KeepWaiting => !_predicate();
    }
}