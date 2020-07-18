namespace Crimson.AI.BehaviorTree
{
    public abstract class Behavior<T>
    {
        public TaskStatus Status = TaskStatus.Invalid;

        public abstract TaskStatus Update(T context);

        public virtual void Invalidate()
        {
            Status = TaskStatus.Invalid;
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnEnd()
        {
        }

        public virtual float Utility => 0f;

        internal TaskStatus Tick(T context)
        {
            if (Status == TaskStatus.Invalid)
                OnStart();

            Status = Update(context);
            
            if (Status != TaskStatus.Running)
                OnEnd();

            return Status;
        }
    }
}