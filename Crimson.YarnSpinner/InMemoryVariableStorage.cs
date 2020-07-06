using System.Collections;
using System.Collections.Generic;
using Yarn;

namespace Crimson.YarnSpinner
{
    public class InMemoryVariableStorage : VariableStorageComponent, IEnumerable<KeyValuePair<string, Yarn.Value>>
    {
        /// Where we actually keep our variables
        private Dictionary<string, Yarn.Value> _variables = new Dictionary<string, Yarn.Value> ();

        /// <summary>
        /// A default value to apply when the object wakes up, or when
        /// ResetToDefaults is called.
        /// </summary>
        public class DefaultVariable
        {
            /// <summary>
            /// The name of the variable.
            /// </summary>
            public string Name;
            /// <summary>
            /// The value of the variable, as a string.
            /// </summary>
            public string Value;
            /// <summary>
            /// The type of the variable.
            /// </summary>
            public Yarn.Value.Type Type;
        }

        /// <summary>
        /// The list of default variables that should be present in the
        /// InMemoryVariableStorage when the scene loads.
        /// </summary>
        public DefaultVariable[] DefaultVariables = new DefaultVariable[0];

        public override void EntityAwake()
        {
            base.EntityAwake();
            ResetToDefaults();
        }

        public override void ResetToDefaults()
        {
            Clear();

            foreach (var variable in DefaultVariables)
            {
                object value;
                switch (variable.Type)
                {
                    case Value.Type.Number:
                        float f = 0f;
                        float.TryParse(variable.Value, out f);
                        value = f;
                        break;
                    
                    case Value.Type.String:
                        value = variable.Value;
                        break;
                    
                    case Value.Type.Bool:
                        bool b = false;
                        bool.TryParse(variable.Value, out b);
                        value = b;
                        break;
                    
                    case Value.Type.Variable:
                        Utils.LogError($"Can't set variable {variable.Name} to {variable.Value}: You can't set a default variable to be another variable, because it may not have been initialized yet.");
                        continue;
                    
                    case Value.Type.Null:
                        value = null;
                        break;
                    
                    default:
                        throw new System.ArgumentOutOfRangeException();
                        
                }
                
                var v = new Yarn.Value(value);
                SetValue("$" + variable.Name, v);
            }
        }

        public override void SetValue(string variableName, Value value)
        {
            _variables[variableName] = new Value(value);
        }

        public override Value GetValue(string variableName)
        {
            if (_variables.ContainsKey(variableName) == false)
                return Yarn.Value.NULL;
            return _variables[variableName];
        }

        public override void Clear()
        {
            _variables.Clear();
        }

        IEnumerator<KeyValuePair<string, Value>> IEnumerable<KeyValuePair<string, Value>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Value>>) _variables).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Value>>) _variables).GetEnumerator();
        }
    }
}