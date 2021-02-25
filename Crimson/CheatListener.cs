using System;
using System.Collections.Generic;

namespace Crimson
{
    public class CheatListener : Entity
    {
        public string CurrentInput;
        public bool   Logging;

        private List<Tuple<char, Func<bool>>> _inputs;
        private List<Tuple<string, Action?>>  _cheats;
        private int                           _maxInput;

        public CheatListener()
        {
            Visible      = false;
            CurrentInput = "";
            _inputs      = new List<Tuple<char, Func<bool>>>();
            _cheats      = new List<Tuple<string, Action?>>();
        }

        public override void Update()
        {
            bool changed = false;
            foreach ( var input in _inputs )
            {
                if ( input.Item2() )
                {
                    CurrentInput += input.Item1;
                    changed      =  true;
                }
            }

            if ( !changed ) return;

            if ( CurrentInput.Length > _maxInput )
            {
                CurrentInput = CurrentInput.Substring(CurrentInput.Length - _maxInput);
            }

            if ( Logging )
            {
                Utils.Log(CurrentInput);
            }

            foreach ( var cheat in _cheats )
            {
                if ( CurrentInput.Contains(cheat.Item1) )
                {
                    CurrentInput = "";
                    cheat.Item2?.Invoke();
                    _cheats.Remove(cheat);
                    if ( Logging )
                    {
                        Utils.Log("Cheat Activated: " + cheat.Item1);
                    }

                    break;
                }
            }
        }

        public void AddCheat(string code, Action? onEntered = null)
        {
            _cheats.Add(new Tuple<string, Action?>(code, onEntered));
            _maxInput = Mathf.Max(_maxInput, code.Length);
        }

        public void AddInput(char id, Func<bool> checker)
        {
            _inputs.Add(new Tuple<char, Func<bool>>(id, checker));
        }
    }
}
