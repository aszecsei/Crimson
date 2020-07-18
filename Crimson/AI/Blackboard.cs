using System;
using System.Collections.Generic;

namespace Crimson.AI
{
    /// <summary>
    /// A Blackboard database, consisting of string keys and dynamic values. Shared blackboards can be retrieved using the
    /// static <see cref="GetSharedBlackboard"/> method.
    /// </summary>
    public class Blackboard : ICloneable
    {
        private static Dictionary<string, Blackboard> s_sharedBlackboards = new Dictionary<string, Blackboard>();

        public static Blackboard GetSharedBlackboard(string key)
        {
            if (!s_sharedBlackboards.ContainsKey(key))
            {
                s_sharedBlackboards[key] = new Blackboard();
            }

            return s_sharedBlackboards[key];
        }
        
        private struct BlackboardValue
        {
            public Type Type;
            public dynamic Value;
        }
        
        private Dictionary<string, BlackboardValue> _internal = new Dictionary<string, BlackboardValue>();

        public void Set<T>(string key, T value)
        {
            _internal[key] = new BlackboardValue
            {
                Type = typeof(T),
                Value = value,
            };
        }

        public T Get<T>(string key)
        {
            if (!_internal.ContainsKey(key))
                return default(T);
            
            var v = _internal[key];
            if (typeof(T) != v.Type)
            {
                throw new ArgumentException("value does not match type", nameof(T));
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

        public object Clone()
        {
            var b = (Blackboard)MemberwiseClone();
            b._internal = new Dictionary<string, BlackboardValue>(_internal);

            return b;
        }
    }
}