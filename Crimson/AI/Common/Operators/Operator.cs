using System;

namespace Crimson.AI
{
    /// <summary>
    /// A base class representing operators: actions that an AI may perform. These operators can be used by multiple AI
    /// types, including behavior trees, GOAP planners, utility-based AIs, and HTNs.
    /// </summary>
    public abstract class Operator : ICloneable
    {
        public TaskStatus Status = TaskStatus.Invalid;
        
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
        /// Invalidate the Operator.
        /// </summary>
        public virtual void Invalidate()
        {
            Status = TaskStatus.Invalid;
        }
        
        /// <summary>
        /// Called whenever the Operator begins running.
        /// </summary>
        public virtual void OnStart() {}
        
        /// <summary>
        /// Called whenever the Operator stops running.
        /// </summary>
        public virtual void OnEnd() {}
        
        internal TaskStatus Tick(Blackboard context)
        {
            if (Status == TaskStatus.Invalid)
                OnStart();

            Status = Update(context);
            
            if (Status != TaskStatus.Running)
                OnEnd();

            return Status;
        }

        /// <inheritdoc cref="ICloneable.Clone()"/>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
        
        public virtual float Utility => 0f;
    }
}