﻿using System;

namespace Crimson.History
{
    public class HistoryHandler : History
    {
        public HistoryHandler(HistoryHandler? historyHandler) : base(historyHandler) {}

        public void Add(HistorySet hs)
        {
            _pastSetup.Add(hs.Undo);
            _futureSetup.Add(hs.Redo);

            TryCommit();
        }

        public void Add(Action undo, Action redo)
        {
            _pastSetup.Add(undo);
            _futureSetup.Add(redo);

            TryCommit();
        }
    }
}