using System;
using System.Collections;
using System.Collections.Generic;

namespace Crimson
{
    public class Coroutine : YieldInstruction
    {
        internal readonly CoroutineList Parent;
        internal readonly IEnumerator FunctionCall;
        internal object? Waiting;
        internal bool Active;

        private float _timer = 0;

        internal Coroutine(CoroutineList parent, IEnumerator functionCall)
        {
            FunctionCall = functionCall;
            Active = true;
            Parent = parent;
            Waiting = null;

            MoveNext();
        }

        private bool MoveNext()
        {
            bool canMoveNext = FunctionCall.MoveNext();
            if (!canMoveNext)
            {
                Active = false;
            }
            else
            {
                Waiting = FunctionCall.Current;
                if (Waiting is WaitForSeconds ws)
                {
                    _timer = ws.Duration;
                }
            }
            
            return canMoveNext;
        }

        internal void HandleUpdate()
        {
            if (_timer > 0)
            {
                _timer -= Time.DeltaTime;
                return;
            }
            
            // If the last call was a WFS, we've finished the timer - and thus the WFS is complete :)
            if (Waiting is WaitForSeconds)
            {
                MoveNext();
                return;
            }

            if (!Active)
                return;

            // Figure out the next thing we're waiting on
            if (Waiting is CustomYieldInstruction cy)
            {
                // We have a custom yield instruction...check if we're done?
                if (!cy.KeepWaiting)
                {
                    MoveNext();
                }
                // Either way, we're done for this cycle
                return;
            }
            if (Waiting is WaitForEndOfFrame)
            {
                // Still waiting
                return;
            }
            if (Waiting is IEnumerator ie)
            {
                // We've got a new coroutine!
                Waiting = Parent.StartCoroutine(ie);
                return;
            }
            if (Waiting is Coroutine c)
            {
                // We're waiting on a coroutine
                if (!c.Active)
                {
                    MoveNext();
                }

                return;
            }
            if (Waiting == null)
            {
                MoveNext();
                return;
            }
            
            // We've got some weird return value here...
            Engine.Commands.Log($"Unexpected coroutine result: {Waiting.GetType().ToString()}");
        }

        internal void HandleEndOfFrame()
        {
            if (Waiting is WaitForEndOfFrame)
            {
                MoveNext();
            }
        }
    }
}