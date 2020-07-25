using System;
using System.Collections.Generic;

namespace Crimson.History
{
    public class HistorySet
    {
        private List<Action> _undos;
        private List<Action> _redos;
        
        public HistorySet(List<Action> undos, List<Action> redos)
        {
            _undos = undos;
            _redos = redos;
        }

        public void Undo()
        {
            foreach (Action a in _undos)
                a();
        }

        public void Redo()
        {
            foreach (Action a in _redos)
                a();
        }
    }
}