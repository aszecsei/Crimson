using System;

namespace Crimson.AI
{
    /// <summary>
    /// A base class representing operators: actions that an AI may perform. These operators can be used by multiple AI
    /// types, including behavior trees, GOAP planners, utility-based AIs, and HTNs.
    /// </summary>
    public abstract class Operator : ICloneable
    {
        public string? Name;

        public virtual string OperatorType => "Operator";

        protected Operator()
        {
            Name = GetType().GetCustomAttribute<AITag>()?.Tag;
        }
        
        /// <summary>
        /// Returns true if the operator can currently be performed, false otherwise.
        /// </summary>
        /// <param name="context">The current world state</param>
        public virtual bool IsSatisfied(Blackboard context) => true;
        
        /// <summary>
        /// Represents post-conditions of the behavior. Modifies the context to simulate the end result of the operator.
        /// </summary>
        public virtual void Execute(Blackboard context) {}
        
        /// <summary>
        /// Runs the operator.
        /// </summary>
        /// <param name="context">The current world state.</param>
        /// <returns>A value describing whether the operator is currently running, has finished successfully or unsuccessfully, or has entered an invalid state.</returns>
        public abstract TaskStatus Update(Blackboard context);

        /// <summary>
        /// Called whenever the Operator begins running.
        /// </summary>
        public virtual void OnStart() {}
        
        /// <summary>
        /// Called whenever the Operator stops running.
        /// </summary>
        public virtual void OnEnd() {}

        /// <inheritdoc cref="ICloneable.Clone()"/>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public virtual int Cost => 0;
        
        public virtual int Utility => 0;
        
        public TaskInstance Instance() => new TaskInstance(this);

        public override string ToString()
        {
            return $"[{OperatorType}] {Name}, Cost {Cost}, Utility {Utility}";
        }
    }
}