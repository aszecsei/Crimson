namespace Crimson
{
    public abstract class Subsystem
    {
        public string Name;

        protected internal readonly int Priority;
        
        public int MainThreadId { get; internal set; }

        public bool IsRegistered;

        public bool IsStarted;
        
        protected Subsystem(int priority = 10000)
        {
            Name = GetType().Name;
            Priority = priority;
        }

        /// <summary>
        /// Called when the application begins, after the primary window is created.
        /// If the application has already started when the subsystem is registered, this will be called immediately.
        /// </summary>
        protected internal virtual void Startup() { }

        /// <summary>
        /// Called when the application shuts down, or the subsystem is removed
        /// </summary>
        protected internal virtual void Shutdown() { }

        /// <summary>
        /// Called after the shutdown method when the subsystem should be fully disposed
        /// </summary>
        protected internal virtual void Disposed() { }
        
        /// <summary>
        /// Called after a scene ends, before a new scene begins
        /// </summary>
        protected internal virtual void OnSceneTransition(Scene? from, Scene? to) { }

        /// <summary>
        /// Called every frame before the Update method
        /// </summary>
        protected internal virtual void BeforeUpdate() { }

        /// <summary>
        /// Called every frame
        /// </summary>
        protected internal virtual void Update() { }

        /// <summary>
        /// Called every frame after the Update method
        /// </summary>
        protected internal virtual void AfterUpdate() { }

        /// <summary>
        /// Called before any rendering
        /// </summary>
        protected internal virtual void BeforeRender() { }

        /// <summary>
        /// Called after all rendering
        /// </summary>
        protected internal virtual void AfterRender() { }

        /// <summary>
        /// Called at the very beginning of the frame
        /// </summary>
        protected internal virtual void Tick() { }
        
        protected internal virtual void ViewUpdated() { }
    }
}