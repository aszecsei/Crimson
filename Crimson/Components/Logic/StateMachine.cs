using System;
using System.Collections;

namespace Crimson
{
    public class StateMachine : Component
    {
        private readonly Action[] _begins;

        public bool ChangedStates;
        private readonly Func<IEnumerator>[] _coroutines;
        private readonly Coroutine? _currentCoroutine;
        private readonly Action[] _ends;
        public bool Locked;
        public bool Log;
        private int _state;
        private readonly Func<int>[] _updates;

        public StateMachine(int maxStates = 10)
            : base(true, false)
        {
            PreviousState = _state = -1;

            _begins = new Action[maxStates];
            _updates = new Func<int>[maxStates];
            _ends = new Action[maxStates];
            _coroutines = new Func<IEnumerator>[maxStates];

            _currentCoroutine = null;
        }

        public int PreviousState { get; private set; }

        public int State
        {
            get { return _state; }
            set
            {
#if DEBUG
                if (value >= _updates.Length || value < 0)
                    throw new Exception("StateMachine state out of range");
#endif

                if (!Locked && _state != value)
                {
                    if (Log)
                        Utils.Log("Enter State " + value + " (leaving " + _state + ")");

                    ChangedStates = true;
                    PreviousState = _state;
                    _state = value;

                    if (PreviousState != -1 && _ends[PreviousState] != null)
                    {
                        if (Log)
                            Utils.Log("Calling End " + PreviousState);
                        _ends[PreviousState]();
                    }

                    if (_begins[_state] != null)
                    {
                        if (Log)
                            Utils.Log("Calling Begin " + _state);
                        _begins[_state]();
                    }

                    if (_currentCoroutine != null)
                    {
                        StopCoroutine(_currentCoroutine);
                    }
                    if (_coroutines[_state] != null)
                    {
                        if (Log)
                            Utils.Log("Starting Coroutine " + _state);
                        StartCoroutine(_coroutines[_state]());
                    }
                }
            }
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);

            if (Entity.Scene != null && _state == -1)
                State = 0;
        }

        public override void EntityAdded(Scene scene)
        {
            base.EntityAdded(scene);

            if (_state == -1)
                State = 0;
        }

        public void ForceState(int toState)
        {
            if (_state != toState)
            {
                State = toState;
            }
            else
            {
                if (Log)
                    Utils.Log("Enter State " + toState + " (leaving " + _state + ")");

                ChangedStates = true;
                PreviousState = _state;
                _state = toState;

                if (PreviousState != -1 && _ends[PreviousState] != null)
                {
                    if (Log)
                        Utils.Log("Calling End " + _state);
                    _ends[PreviousState]();
                }

                if (_begins[_state] != null)
                {
                    if (Log)
                        Utils.Log("Calling Begin " + _state);
                    _begins[_state]();
                }

                if (_currentCoroutine != null)
                {
                    StopCoroutine(_currentCoroutine);
                }
                if (_coroutines[_state] != null)
                {
                    if (Log)
                        Utils.Log("Starting Coroutine " + _state);
                    StartCoroutine(_coroutines[_state]());
                }
            }
        }

        public void SetCallbacks(int state, Func<int> onUpdate, Func<IEnumerator> coroutine = null, Action begin = null,
            Action end = null)
        {
            _updates[state] = onUpdate;
            _begins[state] = begin;
            _ends[state] = end;
            _coroutines[state] = coroutine;
        }

        public void ReflectState(Entity from, int index, string name)
        {
            _updates[index] = (Func<int>) Utils.GetMethod<Func<int>>(from, name + "Update");
            _begins[index] = (Action) Utils.GetMethod<Action>(from, name + "Begin");
            _ends[index] = (Action) Utils.GetMethod<Action>(from, name + "End");
            _coroutines[index] = (Func<IEnumerator>) Utils.GetMethod<Func<IEnumerator>>(from, name + "Coroutine");
        }

        public override void Update()
        {
            base.Update();

            ChangedStates = false;

            if (_updates[_state] != null)
                State = _updates[_state]();
        }

        public static implicit operator int(StateMachine s)
        {
            return s._state;
        }

        public void LogAllStates()
        {
            for (var i = 0; i < _updates.Length; i++)
                LogState(i);
        }

        public void LogState(int index)
        {
            Utils.Log("State " + index + ": "
                      + (_updates[index] != null ? "U" : "")
                      + (_begins[index] != null ? "B" : "")
                      + (_ends[index] != null ? "E" : "")
                      + (_coroutines[index] != null ? "C" : ""));
        }
    }
}
