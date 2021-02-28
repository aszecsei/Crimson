using System;
using System.Collections;
using System.Collections.Generic;
using Crimson.Physics;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public abstract class Scene : IEnumerable<Entity>, IEnumerable
    {
        private readonly Dictionary<int, double> _actualDepthLookup;

        public bool HasFocus = true;
        public bool Paused;
        public float RawTimeActive;
        public float TimeActive;

        public Scene()
        {
            Tracker      = new Tracker();
            Entities     = new EntityList(this);
            TagLists     = new TagLists();
            RendererList = new RendererList(this);
            Coroutines   = new CoroutineList();

            _actualDepthLookup = new Dictionary<int, double>();
        }

        public bool Focused { get; private set; }

        public EntityList    Entities     { get; }
        public TagLists      TagLists     { get; }
        public RendererList  RendererList { get; }
        public Tracker       Tracker      { get; }
        public CoroutineList Coroutines   { get; }

        public event Action? OnEndOfFrame;

        public virtual void Begin()
        {
            Focused = true;
            foreach (Entity entity in Entities) entity.SceneBegin(this);
        }

        public virtual void End()
        {
            Focused = false;
            foreach (Entity entity in Entities) entity.SceneEnd(this);
        }

        public virtual void BeforeUpdate()
        {
            if (!Paused) TimeActive += Time.DeltaTime;

            RawTimeActive += Time.RawDeltaTime;

            Entities.UpdateLists();
            TagLists.UpdateLists();
            RendererList.UpdateLists();
            RendererList.BeforeUpdate();
        }

        public virtual void Update()
        {
            if (!Paused)
            {
                Entities.Update();
                RendererList.Update();
            }

            Coroutines.HandleUpdate();
        }

        public virtual void AfterUpdate()
        {
            if (!Paused) Entities.LateUpdate();
            Coroutines.HandleEndOfFrame();

            if (OnEndOfFrame != null)
            {
                OnEndOfFrame.Invoke();
                OnEndOfFrame = null;
            }
        }

        public virtual void BeforeRender()
        {
            RendererList.BeforeRender();
        }

        public virtual void Render()
        {
            RendererList.Render();
        }

        public virtual void AfterRender()
        {
            RendererList.AfterRender();

            Entities.EndOfFrame();
        }

        public virtual void HandleGraphicsReset()
        {
            Entities.HandleGraphicsReset();
        }

        public virtual void HandleGraphicsCreate()
        {
            Entities.HandleGraphicsCreate();
        }

        public virtual void GainFocus()
        {
            HasFocus = true;
        }

        public virtual void LoseFocus()
        {
            HasFocus = false;
        }

        #region Utils

        internal void SetActualDepth(Entity entity)
        {
            const double theta = 0.000001f;

            if (_actualDepthLookup.TryGetValue(entity.Depth, out var add))
                _actualDepthLookup[entity.Depth] += theta;
            else
                _actualDepthLookup.Add(entity.Depth, theta);

            entity.ActualDepth = entity.Depth - add;

            // Mark lists unsorted
            Entities.MarkUnsorted();
            for (var i = 0; i < BitTag.TotalTags; i++)
                if (entity.TagCheck(1 << i))
                    TagLists.MarkUnsorted(i);
        }

        #endregion

        #region Interval

        /// <summary>
        ///     Returns whether the Scene timer has passed the given time interval since the last frame. Ex: given 2.0f, this will
        ///     return true once every 2 seconds.
        /// </summary>
        /// <param name="interval">The time interval to check for</param>
        /// <returns></returns>
        public bool OnInterval(float interval)
        {
            return (int) ((TimeActive - Time.DeltaTime) / interval) < (int) (TimeActive / interval);
        }

        /// <summary>
        ///     Returns whether the Scene timer has passed the given time interval since the last frame. Ex: given 2.0f, this will
        ///     return true once every 2 seconds.
        /// </summary>
        /// <param name="interval">The time interval to check for</param>
        /// <param name="offset">An offset time interval</param>
        /// <returns></returns>
        public bool OnInterval(float interval, float offset)
        {
            return Mathf.Floor((TimeActive - offset - Time.DeltaTime) / interval) <
                   Mathf.Floor((TimeActive - offset) / interval);
        }

        public bool BetweenInterval(float interval)
        {
            return Mathf.BetweenInterval(TimeActive, interval);
        }

        public bool OnRawInterval(float interval)
        {
            return (int) ((RawTimeActive - Time.RawDeltaTime) / interval) < (int) (RawTimeActive / interval);
        }

        public bool OnRawInterval(float interval, float offset)
        {
            return Mathf.Floor((RawTimeActive - offset - Time.RawDeltaTime) / interval) <
                   Mathf.Floor((RawTimeActive - offset) / interval);
        }

        public bool BetweenRawInterval(float interval)
        {
            return Mathf.BetweenInterval(RawTimeActive, interval);
        }

        #endregion

        #region Entity Shortcuts

        public List<Entity> this[BitTag tag] => TagLists[tag.ID];

        public void Add(Entity entity)
        {
            Entities.Add(entity);
        }

        public void Add(IEnumerable<Entity> entities)
        {
            Entities.Add(entities);
        }

        public void Add(params Entity[] entities)
        {
            Entities.Add(entities);
        }

        public void Remove(Entity entity)
        {
            Entities.Remove(entity);
        }

        public void Remove(IEnumerable<Entity> entities)
        {
            Entities.Remove(entities);
        }

        public void Remove(params Entity[] entities)
        {
            Entities.Remove(entities);
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Entity? FindEntityNamed(string name)
        {
            return Entities.FindEntityNamed(name);
        }

        public List<Entity> GetEntitiesByTagMask(int mask)
        {
            var list = new List<Entity>();
            foreach (Entity entity in Entities)
                if ((entity.Tag & mask) != 0)
                    list.Add(entity);

            return list;
        }

        public List<Entity> GetEntitiesExcludingTagMask(int mask)
        {
            var list = new List<Entity>();
            foreach (Entity entity in Entities)
                if ((entity.Tag & mask) == 0)
                    list.Add(entity);

            return list;
        }

        public void Add(Renderer renderer)
        {
            RendererList.Add(renderer);
        }

        public void Remove(Renderer renderer)
        {
            RendererList.Remove(renderer);
        }

        #endregion

        #region Physics Shortcuts

        public bool CollideCheck(Vector2 point, int tag)
        {
            List<Entity> list = TagLists[tag];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Collidable && list[i].CollidePoint(point))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CollideCheck(Vector2 from, Vector2 to, int tag)
        {
            List<Entity> list = TagLists[tag];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Collidable && list[i].CollideLine(from, to))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CollideCheck(Rectangle rect, int tag)
        {
            List<Entity> list = TagLists[tag];
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Collidable && list[i].CollideRect(rect))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CollideCheck(Rectangle rect, Entity entity)
        {
            if (entity.Collidable)
            {
                return entity.CollideRect(rect);
            }
            return false;
        }

        public Entity CollideFirst(Vector2 point, int tag)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollidePoint(point))
			    {
				    return list[i];
			    }
		    }
		    return null;
	    }

	    public Entity CollideFirst(Vector2 from, Vector2 to, int tag)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideLine(from, to))
			    {
				    return list[i];
			    }
		    }
		    return null;
	    }

	    public Entity CollideFirst(Rectangle rect, int tag)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideRect(rect))
			    {
				    return list[i];
			    }
		    }
		    return null;
	    }

	    public void CollideInto(Vector2 point, int tag, List<Entity> hits)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollidePoint(point))
			    {
				    hits.Add(list[i]);
			    }
		    }
	    }

	    public void CollideInto(Vector2 from, Vector2 to, int tag, List<Entity> hits)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideLine(from, to))
			    {
				    hits.Add(list[i]);
			    }
		    }
	    }

	    public void CollideInto(Rectangle rect, int tag, List<Entity> hits)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideRect(rect))
			    {
				    list.Add(list[i]);
			    }
		    }
	    }

	    public List<Entity> CollideAll(Vector2 point, int tag)
	    {
		    List<Entity> list = new List<Entity>();
		    CollideInto(point, tag, list);
		    return list;
	    }

	    public List<Entity> CollideAll(Vector2 from, Vector2 to, int tag)
	    {
		    List<Entity> list = new List<Entity>();
		    CollideInto(from, to, tag, list);
		    return list;
	    }

	    public List<Entity> CollideAll(Rectangle rect, int tag)
	    {
		    List<Entity> list = new List<Entity>();
		    CollideInto(rect, tag, list);
		    return list;
	    }

	    public void CollideDo(Vector2 point, int tag, Action<Entity> action)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollidePoint(point))
			    {
				    action(list[i]);
			    }
		    }
	    }

	    public void CollideDo(Vector2 from, Vector2 to, int tag, Action<Entity> action)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideLine(from, to))
			    {
				    action(list[i]);
			    }
		    }
	    }

	    public void CollideDo(Rectangle rect, int tag, Action<Entity> action)
	    {
		    List<Entity> list = TagLists[tag];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideRect(rect))
			    {
				    action(list[i]);
			    }
		    }
	    }

	    public Vector2 LineWalkCheck(Vector2 from, Vector2 to, int tag, float precision)
	    {
		    Vector2 add = to - from;
		    add.Normalize();
		    add *= precision;
		    int amount = (int)Math.Floor((from - to).Length() / precision);
		    Vector2 prev = from;
		    Vector2 at = from + add;
		    for (int i = 0; i <= amount; i++)
		    {
			    if (CollideCheck(at, tag))
			    {
				    return prev;
			    }
			    prev = at;
			    at += add;
		    }
		    return to;
	    }

	    public bool CollideCheck<T>(Vector2 point) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollidePoint(point))
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public bool CollideCheck<T>(Vector2 from, Vector2 to) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideLine(from, to))
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public bool CollideCheck<T>(Rectangle rect) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideRect(rect))
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public T CollideFirst<T>(Vector2 point) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollidePoint(point))
			    {
				    return list[i] as T;
			    }
		    }
		    return null;
	    }

	    public T CollideFirst<T>(Vector2 from, Vector2 to) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideLine(from, to))
			    {
				    return list[i] as T;
			    }
		    }
		    return null;
	    }

	    public T CollideFirst<T>(Rectangle rect) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideRect(rect))
			    {
				    return list[i] as T;
			    }
		    }
		    return null;
	    }

	    public void CollideInto<T>(Vector2 point, List<Entity> hits) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollidePoint(point))
			    {
				    hits.Add(list[i]);
			    }
		    }
	    }

	    public void CollideInto<T>(Vector2 from, Vector2 to, List<Entity> hits) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideLine(from, to))
			    {
				    hits.Add(list[i]);
			    }
		    }
	    }

	    public void CollideInto<T>(Rectangle rect, List<Entity> hits) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideRect(rect))
			    {
				    list.Add(list[i]);
			    }
		    }
	    }

	    public void CollideInto<T>(Vector2 point, List<T> hits) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollidePoint(point))
			    {
				    hits.Add(list[i] as T);
			    }
		    }
	    }

	    public void CollideInto<T>(Vector2 from, Vector2 to, List<T> hits) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideLine(from, to))
			    {
				    hits.Add(list[i] as T);
			    }
		    }
	    }

	    public void CollideInto<T>(Rectangle rect, List<T> hits) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideRect(rect))
			    {
				    hits.Add(list[i] as T);
			    }
		    }
	    }

	    public List<T> CollideAll<T>(Vector2 point) where T : Entity
	    {
		    List<T> list = new List<T>();
		    CollideInto(point, list);
		    return list;
	    }

	    public List<T> CollideAll<T>(Vector2 from, Vector2 to) where T : Entity
	    {
		    List<T> list = new List<T>();
		    CollideInto(from, to, list);
		    return list;
	    }

	    public List<T> CollideAll<T>(Rectangle rect) where T : Entity
	    {
		    List<T> list = new List<T>();
		    CollideInto(rect, list);
		    return list;
	    }

	    public void CollideDo<T>(Vector2 point, Action<T> action) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollidePoint(point))
			    {
				    action(list[i] as T);
			    }
		    }
	    }

	    public void CollideDo<T>(Vector2 from, Vector2 to, Action<T> action) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideLine(from, to))
			    {
				    action(list[i] as T);
			    }
		    }
	    }

	    public void CollideDo<T>(Rectangle rect, Action<T> action) where T : Entity
	    {
		    List<Entity> list = Tracker.Entities[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Collidable && list[i].CollideRect(rect))
			    {
				    action(list[i] as T);
			    }
		    }
	    }

	    public Vector2 LineWalkCheck<T>(Vector2 from, Vector2 to, float precision) where T : Entity
	    {
		    Vector2 add = to - from;
		    add.Normalize();
		    add *= precision;
		    int amount = (int)Math.Floor((from - to).Length() / precision);
		    Vector2 prev = from;
		    Vector2 at = from + add;
		    for (int i = 0; i <= amount; i++)
		    {
			    if (CollideCheck<T>(at))
			    {
				    return prev;
			    }
			    prev = at;
			    at += add;
		    }
		    return to;
	    }

	    public bool CollideCheckByComponent<T>(Vector2 point) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public bool CollideCheckByComponent<T>(Vector2 from, Vector2 to) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public bool CollideCheckByComponent<T>(Rectangle rect) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
			    {
				    return true;
			    }
		    }
		    return false;
	    }

	    public T CollideFirstByComponent<T>(Vector2 point) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
			    {
				    return list[i] as T;
			    }
		    }
		    return null;
	    }

	    public T CollideFirstByComponent<T>(Vector2 from, Vector2 to) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
			    {
				    return list[i] as T;
			    }
		    }
		    return null;
	    }

	    public T CollideFirstByComponent<T>(Rectangle rect) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
			    {
				    return list[i] as T;
			    }
		    }
		    return null;
	    }

	    public void CollideIntoByComponent<T>(Vector2 point, List<Component> hits) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
			    {
				    hits.Add(list[i]);
			    }
		    }
	    }

	    public void CollideIntoByComponent<T>(Vector2 from, Vector2 to, List<Component> hits) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
			    {
				    hits.Add(list[i]);
			    }
		    }
	    }

	    public void CollideIntoByComponent<T>(Rectangle rect, List<Component> hits) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
			    {
				    list.Add(list[i]);
			    }
		    }
	    }

	    public void CollideIntoByComponent<T>(Vector2 point, List<T> hits) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
			    {
				    hits.Add(list[i] as T);
			    }
		    }
	    }

	    public void CollideIntoByComponent<T>(Vector2 from, Vector2 to, List<T> hits) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
			    {
				    hits.Add(list[i] as T);
			    }
		    }
	    }

	    public void CollideIntoByComponent<T>(Rectangle rect, List<T> hits) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
			    {
				    list.Add(list[i] as T);
			    }
		    }
	    }

	    public List<T> CollideAllByComponent<T>(Vector2 point) where T : Component
	    {
		    List<T> list = new List<T>();
		    CollideIntoByComponent(point, list);
		    return list;
	    }

	    public List<T> CollideAllByComponent<T>(Vector2 from, Vector2 to) where T : Component
	    {
		    List<T> list = new List<T>();
		    CollideIntoByComponent(from, to, list);
		    return list;
	    }

	    public List<T> CollideAllByComponent<T>(Rectangle rect) where T : Component
	    {
		    List<T> list = new List<T>();
		    CollideIntoByComponent(rect, list);
		    return list;
	    }

	    public void CollideDoByComponent<T>(Vector2 point, Action<T> action) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollidePoint(point))
			    {
				    action(list[i] as T);
			    }
		    }
	    }

	    public void CollideDoByComponent<T>(Vector2 from, Vector2 to, Action<T> action) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideLine(from, to))
			    {
				    action(list[i] as T);
			    }
		    }
	    }

	    public void CollideDoByComponent<T>(Rectangle rect, Action<T> action) where T : Component
	    {
		    List<Component> list = Tracker.Components[typeof(T)];
		    for (int i = 0; i < list.Count; i++)
		    {
			    if (list[i].Entity.Collidable && list[i].Entity.CollideRect(rect))
			    {
				    action(list[i] as T);
			    }
		    }
	    }

	    public Vector2 LineWalkCheckByComponent<T>(Vector2 from, Vector2 to, float precision) where T : Component
	    {
		    Vector2 add = to - from;
		    add.Normalize();
		    add *= precision;
		    int amount = (int)Math.Floor((from - to).Length() / precision);
		    Vector2 prev = from;
		    Vector2 at = from + add;
		    for (int i = 0; i <= amount; i++)
		    {
			    if (CollideCheckByComponent<T>(at))
			    {
				    return prev;
			    }
			    prev = at;
			    at += add;
		    }
		    return to;
	    }
    #endregion

        #region Coroutines

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return Coroutines.StartCoroutine(routine);
        }

        public void StopAllCoroutines()
        {
            Coroutines.StopAllCoroutines();
        }

        public void StopCoroutine(IEnumerator routine)
        {
            Coroutines.StopCoroutine(routine);
        }

        public void StopCoroutine(Coroutine routine)
        {
            Coroutines.StopCoroutine(routine);
        }

        #endregion
    }
}
