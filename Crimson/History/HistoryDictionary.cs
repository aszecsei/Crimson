using System.Collections;
using System.Collections.Generic;

namespace Crimson.History
{
    /// <summary>
    /// A dictionary that can preserve its history.
    /// </summary>
    public class HistoryDictionary<TKey, TValue> : History, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> _hDict;

        public HistoryDictionary(Dictionary<TKey, TValue> dict, HistoryHandler? historyHandler) : base(historyHandler)
        {
            _hDict = dict;
        }

        public void Add(TKey key, TValue item)
        {
            _futureSetup.Add(() => _hDict.Add(key, item));
            _pastSetup.Add(() => _hDict.Remove(key));
            TryCommit();
        }

        public void Remove(TKey key)
        {
            TValue removedItem = _hDict[key];
            _futureSetup.Add(() => _hDict.Remove(key));
            _pastSetup.Add(() => _hDict.Add(key, removedItem));
            TryCommit();
        }

        public void Replace(TKey key, TValue item)
        {
            TValue removedItem = _hDict[key];
            _futureSetup.Add(() => _hDict[key] = item);
            _pastSetup.Add(() => _hDict[key] = removedItem);
            TryCommit();
        }

        public void Clear()
        {
            Dictionary<TKey, TValue> savedDict = new Dictionary<TKey, TValue>(_hDict);
            _futureSetup.Add(() => _hDict.Clear());
            _pastSetup.Add(() => _hDict = new Dictionary<TKey, TValue>(savedDict));
            TryCommit();
        }

        public bool ContainsKey(TKey key) => _hDict.ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => _hDict.TryGetValue(key, out value);
        public int Count() => _hDict.Count;

        public TValue this[TKey key]
        {
            get => _hDict[key];
            set => Replace(key, value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _hDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}