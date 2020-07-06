using Yarn;

namespace Crimson.YarnSpinner
{
    public abstract class VariableStorageComponent : Component, Yarn.VariableStorage
    {
        protected VariableStorageComponent() : base(true, false)
        {
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract Value GetValue(string variableName);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void SetValue(string variableName, float floatValue) => SetValue(variableName, new Yarn.Value(floatValue));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void SetValue(string variableName, bool boolValue) =>
            SetValue(variableName, new Yarn.Value(boolValue));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void SetValue(string variableName, string stringValue) =>
            SetValue(variableName, new Yarn.Value(stringValue));

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public abstract void SetValue(string variableName, Value value);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>
        /// The implementation in this abstract class throws a <see
        /// cref="System.NotImplementedException"/> when called. Subclasses of
        /// this class must provide their own implementation.
        /// </remarks>
        public virtual void Clear() => throw new System.NotImplementedException();

        /// <summary>
        /// Resets the VariableStorageBehaviour to its initial state.
        /// </summary>
        /// <remarks>
        /// This is similar to <see cref="Clear"/>, but additionally allows
        /// subclasses to restore any default values that should be
        /// present.
        /// </remarks>
        public abstract void ResetToDefaults();
    }
}