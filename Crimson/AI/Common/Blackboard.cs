using System;
using System.Collections;
using System.Collections.Generic;

namespace Crimson.AI
{
    /// <summary>
    /// A Blackboard database, consisting of string keys and dynamic values. Shared blackboards can be retrieved using the
    /// static <see cref="GetSharedBlackboard"/> method.
    /// </summary>
    public class Blackboard : ICloneable
    {
        private static readonly Dictionary<string, Blackboard> SharedBlackboards = new Dictionary<string, Blackboard>();

        public Action? OnBlackboardUpdate;
        
        public static Blackboard GetSharedBlackboard(string key)
        {
            if (!SharedBlackboards.ContainsKey(key))
            {
                SharedBlackboards[key] = new Blackboard();
            }

            return SharedBlackboards[key];
        }

        private Dictionary<string, BlackboardValue> _internal = new Dictionary<string, BlackboardValue>();

        private struct BlackboardValue : IComparable<BlackboardValue>
        {
            public Type Type;
            public dynamic Value;
            
            public int CompareTo(BlackboardValue other)
            {
                return (Type == other.Type) && (Value == other.Value);
            }
        }
        
        public void Set<T>(string key, T value)
        {
            // Check if there's no change necessary
            if (_internal.ContainsKey(key))
                if (_internal[key].Type == typeof(T) && _internal[key].Value == value)
                    return;
            
            _internal[key] = new BlackboardValue
            {
                Type = typeof(T),
                Value = value,
            };
            OnBlackboardUpdate?.Invoke();
        }

        public T Get<T>(string key)
        {
            if (!_internal.ContainsKey(key))
                return default(T);
            
            var v = _internal[key];
            if (typeof(T) != v.Type)
            {
                throw new ArgumentException($"value of type {v.Type} ({v.Value}) does not match type {typeof(T).FullName}", nameof(T));
            }

            return v.Value;
        }

        /// <summary>
        /// Returns the combination of this Blackboard with a child. Shared values are overriden by the child.
        /// </summary>
        public Blackboard Combine(Blackboard child)
        {
            Blackboard res = (Blackboard) Clone();
            foreach (var kvp in child._internal)
                res._internal[kvp.Key] = kvp.Value;
            return res;
        }

        public bool Contains(string key)
        {
            return _internal.ContainsKey(key);
        }

        public object Clone()
        {
            var b = (Blackboard)MemberwiseClone();
            b._internal = new Dictionary<string, BlackboardValue>(_internal);

            return b;
        }

        /// <summary>
        /// Returns the distance between this blackboard and another, defined as the number of different key/value pairs
        /// the two have.
        /// </summary>
        public int GetDistance(Blackboard other)
        {
            int dist = 0;
            
            foreach (var kvp in _internal)
            {
                if (!other.Contains(kvp.Key))
                    dist++;
                else if (!other._internal[kvp.Key].Equals(kvp.Value))
                    dist++;
            }

            foreach (var kvp in other._internal)
            {
                if (!Contains(kvp.Key))
                    dist++;
            }

            return dist;
        }
    }
}