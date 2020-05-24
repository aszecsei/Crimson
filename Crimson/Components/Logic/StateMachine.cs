using System;
using System.Collections;

namespace Crimson
{
    public class StateMachine : Component
    {
        private readonly Action[] begins;

        public bool ChangedStates;
        private readonly Func<IEnumerator>[] coroutines;
        private readonly Coroutine currentCoroutine;
        private readonly Action[] ends;
        public bool Locked;
        public bool Log;
        private int state;
        private readonly Func<int>[] updates;

        public StateMachine(int maxStates = 10)
            : base(true, false)
        {
            PreviousState = state = -1;

            begins = new Action[maxStates];
            updates = new Func<int>[maxStates];
            ends = new Action[maxStates];
            coroutines = new Func<IEnumerator>[maxStates];

            currentCoroutine = new Coroutine();
            currentCoroutine.RemoveOnComplete = false;
        }

        public int PreviousState { get; private set; }

        public int State
        {
            get { return state; }
            set
            {
#if DEBUG
                if (value >= updates.Length || value < 0)
                    throw new Exception("StateMachine state out of range");
#endif

                if (!Locked && state != value)
                {
                    if (Log)
                        Utils.Log("Enter State " + value + " (leaving " + state + ")");

                    ChangedStates = true;
                    PreviousState = state;
                    state = value;

                    if (PreviousState != -1 && ends[PreviousState] != null)
                    {
                        if (Log)
                            Utils.Log("Calling End " + PreviousState);
                        ends[PreviousState]();
                    }

                    if (begins[state] != null)
                    {
                        if (Log)
                            Utils.Log("Calling Begin " + state);
                        begins[state]();
                    }

                    if (coroutines[state] != null)
                    {
                        if (Log)
                            Utils.Log("Starting Coroutine " + state);
                        currentCoroutine.Replace(coroutines[state]());
                    }
                    else
                    {
                        currentCoroutine.Cancel();
                    }
                }
            }
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);

            if (Entity.Scene != null && state == -1)
                State = 0;
        }

        public override void EntityAdded(Scene scene)
        {
            base.EntityAdded(scene);

            if (state == -1)
                State = 0;
        }

        public void ForceState(int toState)
        {
            if (state != toState)
            {
                State = toState;
            }
            else
            {
                if (Log)
                    Utils.Log("Enter State " + toState + " (leaving " + state + ")");

                ChangedStates = true;
                PreviousState = state;
                state = toState;

                if (PreviousState != -1 && ends[PreviousState] != null)
                {
                    if (Log)
                        Utils.Log("Calling End " + state);
                    ends[PreviousState]();
                }

                if (begins[state] != null)
                {
                    if (Log)
                        Utils.Log("Calling Begin " + state);
                    begins[state]();
                }

                if (coroutines[state] != null)
                {
                    if (Log)
                        Utils.Log("Starting Coroutine " + state);
                    currentCoroutine.Replace(coroutines[state]());
                }
                else
                {
                    currentCoroutine.Cancel();
                }
            }
        }

        public void SetCallbacks(int state, Func<int> onUpdate, Func<IEnumerator> coroutine = null, Action begin = null,
            Action end = null)
        {
            updates[state] = onUpdate;
            begins[state] = begin;
            ends[state] = end;
            coroutines[state] = coroutine;
        }

        public void ReflectState(Entity from, int index, string name)
        {
            updates[index] = (Func<int>) Utils.GetMethod<Func<int>>(from, name + "Update");
            begins[index] = (Action) Utils.GetMethod<Action>(from, name + "Begin");
            ends[index] = (Action) Utils.GetMethod<Action>(from, name + "End");
            coroutines[index] = (Func<IEnumerator>) Utils.GetMethod<Func<IEnumerator>>(from, name + "Coroutine");
        }

        public override void Update()
        {
            ChangedStates = false;

            if (updates[state] != null)
                State = updates[state]();
            if (currentCoroutine.Active)
            {
                currentCoroutine.Update();
                if (!ChangedStates && Log && currentCoroutine.Finished)
                    Utils.Log("Finished Coroutine " + state);
            }
        }

        public static implicit operator int(StateMachine s)
        {
            return s.state;
        }

        public void LogAllStates()
        {
            for (var i = 0; i < updates.Length; i++)
                LogState(i);
        }

        public void LogState(int index)
        {
            Utils.Log("State " + index + ": "
                      + (updates[index] != null ? "U" : "")
                      + (begins[index] != null ? "B" : "")
                      + (ends[index] != null ? "E" : "")
                      + (coroutines[index] != null ? "C" : ""));
        }
    }
}