using System.Collections;

namespace Crimson
{
    public class Component
    {
        public bool Active;
        public bool Visible;
        private CoroutineList _coroutineList;

        public Component(bool active, bool visible)
        {
            Active = active;
            Visible = visible;
            _coroutineList = new CoroutineList();
        }

        public Entity Entity { get; private set; }

        public Scene Scene => Entity?.Scene;

        public virtual void Added(Entity entity)
        {
            Entity = entity;
            if (Scene != null) Scene.Tracker.ComponentAdded(this);
        }

        public virtual void Removed(Entity entity)
        {
            if (Scene != null) Scene.Tracker.ComponentRemoved(this);

            Entity = null;
        }

        public virtual void EntityAdded(Scene scene)
        {
            scene.Tracker.ComponentAdded(this);
        }

        public virtual void EntityRemoved(Scene scene)
        {
            scene.Tracker.ComponentRemoved(this);
        }

        public virtual void SceneEnd(Scene scene)
        {
        }

        public virtual void EntityAwake()
        {
        }

        public virtual void Update()
        {
            _coroutineList.HandleUpdate();
        }

        public virtual void LateUpdate()
        {
        }

        public virtual void Render()
        {
        }

        public virtual void EndOfFrame()
        {
            _coroutineList.HandleEndOfFrame();
        }

        public virtual void DebugRender(Camera camera)
        {
        }

        public virtual void HandleGraphicsReset()
        {
        }

        public virtual void HandleGraphicsCreate()
        {
        }

        public void RemoveSelf()
        {
            if (Entity != null) Entity.RemoveComponent(this);
        }

        public T SceneAs<T>() where T : Scene
        {
            return Scene as T;
        }

        public T EntityAs<T>() where T : Entity
        {
            return Entity as T;
        }
        
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _coroutineList.StartCoroutine(routine);
        }

        public void StopAllCoroutines()
        {
            _coroutineList.StopAllCoroutines();
        }

        public void StopCoroutine(IEnumerator routine)
        {
            _coroutineList.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            _coroutineList.StopCoroutine(routine);
        }
    }
}