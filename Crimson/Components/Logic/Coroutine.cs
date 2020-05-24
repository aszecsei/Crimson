using System.Collections;
using System.Collections.Generic;

namespace Crimson
{
    public class Coroutine : Component
    {
        private readonly Stack<IEnumerator> enumerators;
        private bool ended;
        public bool RemoveOnComplete = true;
        public bool UseRawDeltaTime = false;
        private float waitTimer;

        public Coroutine(IEnumerator functionCall, bool removeOnComplete = true)
            : base(true, false)
        {
            enumerators = new Stack<IEnumerator>();
            enumerators.Push(functionCall);
            RemoveOnComplete = removeOnComplete;
        }

        public Coroutine(bool removeOnComplete = true)
            : base(false, false)
        {
            RemoveOnComplete = removeOnComplete;
            enumerators = new Stack<IEnumerator>();
        }

        public bool Finished { get; private set; }

        public override void Update()
        {
            ended = false;

            if (waitTimer > 0)
            {
                waitTimer -= UseRawDeltaTime ? Time.RawDeltaTime : Time.DeltaTime;
            }
            else if (enumerators.Count > 0)
            {
                IEnumerator now = enumerators.Peek();
                if (now.MoveNext() && !ended)
                {
                    if (now.Current is int)
                        waitTimer = (int) now.Current;
                    if (now.Current is float)
                        waitTimer = (float) now.Current;
                    else if (now.Current is IEnumerator)
                        enumerators.Push(now.Current as IEnumerator);
                }
                else if (!ended)
                {
                    enumerators.Pop();
                    if (enumerators.Count == 0)
                    {
                        Finished = true;
                        Active = false;
                        if (RemoveOnComplete)
                            RemoveSelf();
                    }
                }
            }
        }

        public void Cancel()
        {
            Active = false;
            Finished = true;
            waitTimer = 0;
            enumerators.Clear();

            ended = true;
        }

        public void Replace(IEnumerator functionCall)
        {
            Active = true;
            Finished = false;
            waitTimer = 0;
            enumerators.Clear();
            enumerators.Push(functionCall);

            ended = true;
        }
    }
}