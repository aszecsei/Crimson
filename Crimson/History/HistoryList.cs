﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Crimson.History
{
    /// <summary>
    /// A list that can preserve its history.
    /// </summary>
    public class HistoryList<T> : History, IEnumerable<T>
    {
        private List<T> _hList;
        
        public HistoryList(List<T> list, HistoryHandler? historyHandler) : base(historyHandler)
        {
            _hList = list;
        }

        public void Add(T item)
        {
            int index = _hList.Count;
            _futureSetup.Add(() => _hList.Add(item));
            _pastSetup.Add(() => _hList.RemoveAt(index));
            TryCommit();
        }

        public void Insert(int index, T item)
        {
            _futureSetup.Add(() => _hList.Insert(index, item));
            _pastSetup.Add(() => _hList.RemoveAt(index));
            TryCommit();
        }

        public void RemoveAt(int index)
        {
            T removedItem = _hList[index];
            _futureSetup.Add(() => _hList.RemoveAt(index));
            _pastSetup.Add(() => _hList.Insert(index, removedItem));
            TryCommit();
        }

        public void ReplaceAt(int index, T item)
        {
            T removedItem = _hList[index];
            _futureSetup.Add(() => _hList[index] = item);
            _pastSetup.Add(() => _hList[index] = removedItem);
            TryCommit();
        }

        public void Clear()
        {
            List<T> savedList = _hList.ToList();
            _futureSetup.Add(() => _hList.Clear());
            _pastSetup.Add(() => _hList = savedList.ToList());
            TryCommit();
        }

        public int Count()
        {
            return _hList.Count;
        }

        public T this[int index]
        {
            get => _hList[index];
            set
            {
                ReplaceAt(index, value);
                TryCommit();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _hList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}