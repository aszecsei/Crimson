using System;
using Crimson.AI.BehaviorTree;

namespace Crimson.AI
{
    /// <summary>
    /// Wraps an <see cref="Operator"/>, containing information about the current running status of an instance of that
    /// operator. This is useful for constructing a single AI behavior and creating multiple instances of it across
    /// different entities.
    /// </summary>
    public class TaskInstance : ICloneable
    {
        public TaskStatus Status = TaskStatus.Invalid;

        public Operator Operator;

        internal TaskInstance(Operator @operator)
        {
            Operator = (Operator)@operator.Clone();
        }

        #region Operator Shortcuts

        /// <summary>
        /// Returns true if the task can currently be performed, false otherwise.
        /// </summary>
        /// <param name="context">The current world state</param>
        public bool IsSatisfied(Blackboard context) => Operator.IsSatisfied(context);

        /// <summary>
        /// Represents post-conditions of the task. Modifies the context to simulate the end result of the operator.
        /// </summary>
        public void Execute(Blackboard context) => Operator.Execute(context);

        /// <summary>
        /// Runs the behavior.
        /// </summary>
        /// <param name="context">The current world state.</param>
        /// <returns>A value describing whether the operator is currently running, has finished successfully or unsuccessfully, or has entered an invalid state.</returns>
        public TaskStatus Update(Blackboard context)
        {
            var status = Operator.Update(context);
            
            if (Operator is Decorator decorator && decorator.IsInversed)
            {
                if (status == TaskStatus.Success) return TaskStatus.Failure;
                if (status == TaskStatus.Failure) return TaskStatus.Success;
            }

            return status;
        }
        
        public void OnStart() => Operator.OnStart();
        
        public void OnEnd() => Operator.OnEnd();
        
        public int Cost => Operator.Cost;
        
        public int Utility => Operator.Utility;

        #endregion
        
        public virtual void Invalidate()
        {
            Status = TaskStatus.Invalid;
        }
        
        internal TaskStatus Tick(Blackboard context)
        {
            if (Status != TaskStatus.Running)
                Operator.OnStart();

            Status = Update(context);

            if (Status != TaskStatus.Running)
                Operator.OnEnd();

            return Status;
        }
        
        /// <inheritdoc cref="ICloneable.Clone()"/>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{Operator} ({Status})";
        }
    }
}