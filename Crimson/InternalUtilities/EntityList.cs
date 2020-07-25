using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Crimson
{
    public class EntityList : IEnumerable<Entity>, IEnumerable
    {
        public static Comparison<Entity> CompareDepth = (a, b) => Math.Sign(b.ActualDepth - a.ActualDepth);

        private readonly HashSet<Entity> adding;

        private readonly HashSet<Entity> current;

        private readonly List<Entity> entities;
        private readonly HashSet<Entity> removing;
        private readonly List<Entity> toAdd;
        private readonly List<Entity> toAwake;
        private readonly List<Entity> toRemove;

        private bool unsorted;

        internal EntityList(Scene scene)
        {
            Scene = scene;

            entities = new List<Entity>();
            toAdd = new List<Entity>();
            toAwake = new List<Entity>();
            toRemove = new List<Entity>();

            current = new HashSet<Entity>();
            adding = new HashSet<Entity>();
            removing = new HashSet<Entity>();
        }

        public Scene Scene { get; }

        public int Count => entities.Count;

        public Entity this[int index]
        {
            get
            {
                if (index < 0 || index >= entities.Count) throw new IndexOutOfRangeException();

                return entities[index];
            }
        }

        public IEnumerator<Entity> GetEnumerator()
        {
            return entities.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void MarkUnsorted()
        {
            unsorted = true;
        }

        public void Sort()
        {
            unsorted = false;
            entities.Sort(CompareDepth);
        }

        public void UpdateLists()
        {
            if (toAdd.Count > 0)
            {
                for (var i = 0; i < toAdd.Count; i++)
                {
                    Entity entity = toAdd[i];
                    if (!current.Contains(entity))
                    {
                        current.Add(entity);
                        entities.Add(entity);

                        if (Scene != null)
                        {
                            Scene.TagLists.EntityAdded(entity);
                            Scene.Tracker.EntityAdded(entity);
                            entity.Added(Scene);
                        }
                    }
                }

                unsorted = true;
            }

            if (toRemove.Count > 0)
            {
                for (var i = 0; i < toRemove.Count; i++)
                {
                    Entity entity = toRemove[i];
                    if (current.Contains(entity))
                    {
                        current.Remove(entity);
                        entities.Remove(entity);

                        if (Scene != null)
                        {
                            Scene.TagLists.EntityRemoved(entity);
                            Scene.Tracker.EntityRemoved(entity);
                            entity.Removed(Scene);
                        }
                    }
                }

                toRemove.Clear();
                removing.Clear();
            }

            if (unsorted)
            {
                Sort();
            }

            if (toAdd.Count > 0)
            {
                toAwake.AddRange(toAdd);
                toAdd.Clear();
                adding.Clear();

                foreach (Entity entity in toAwake)
                    if (entity.Scene == Scene)
                        entity.Awake(Scene);

                toAwake.Clear();
            }
        }

        public void Add(Entity entity)
        {
            if (!adding.Contains(entity) && !current.Contains(entity))
            {
                adding.Add(entity);
                toAdd.Add(entity);
            }
        }

        public void Remove(Entity entity)
        {
            if (!removing.Contains(entity) && current.Contains(entity))
            {
                removing.Add(entity);
                toRemove.Add(entity);
            }
        }

        public void Add(IEnumerable<Entity> entities)
        {
            foreach (Entity entity in entities) Add(entity);
        }

        public void Remove(IEnumerable<Entity> entities)
        {
            foreach (Entity entity in entities) Remove(entity);
        }

        public void Add(params Entity[] entities)
        {
            foreach (Entity entity in entities) Add(entity);
        }

        public void Remove(params Entity[] entities)
        {
            foreach (Entity entity in entities) Remove(entity);
        }

        public int AmountOf<T>() where T : Entity
        {
            var count = 0;
            foreach (Entity e in entities)
                if (e is T)
                    count++;

            return count;
        }

        public Entity? FindEntityNamed(string name)
        {
            foreach (Entity e in entities)
                if (e.Name == name)
                    return e;
            return null;
        }

        public T? FindFirst<T>() where T : Entity
        {
            foreach (Entity e in entities)
                if (e is T)
                    return e as T;

            return null;
        }

        public List<T> FindAll<T>() where T : Entity
        {
            var list = new List<T>();
            foreach (Entity e in entities)
                if (e is T)
                    list.Add(e as T);

            return list;
        }

        public void With<T>(Action<T> action) where T : Entity
        {
            foreach (Entity e in entities)
                if (e is T)
                    action(e as T);
        }

        public Entity[] ToArray()
        {
            return entities.ToArray<Entity>();
        }

        public bool HasVisibleEntities(int matchTags)
        {
            foreach (Entity entity in entities)
                if (entity.Visible && entity.TagCheck(matchTags))
                    return true;

            return false;
        }

        internal void Update()
        {
            foreach (Entity entity in entities)
                if (entity.Active)
                    entity.Update();
        }

        internal void LateUpdate()
        {
            foreach (Entity entity in entities)
                if (entity.Active)
                    entity.LateUpdate();
        }

        public void Render()
        {
            for (var i = 0; i < entities.Count; i++)
            {
                Entity entity = entities[i];
                if (entity.Visible) entity.Render();
            }
        }

        public void RenderOnly(int matchTags)
        {
            foreach (Entity entity in entities)
                if (entity.Visible && entity.TagCheck(matchTags))
                    entity.Render();
        }

        public void RenderOnlyFullMatch(int matchTags)
        {
            foreach (Entity entity in entities)
                if (entity.Visible && entity.TagFullCheck(matchTags))
                    entity.Render();
        }

        public void RenderExcept(int matchTags)
        {
            foreach (Entity entity in entities)
                if (entity.Visible && !entity.TagCheck(matchTags))
                    entity.Render();
        }

        public void EndOfFrame()
        {
            foreach (Entity entity in entities)
                if (entity.Active)
                    entity.EndOfFrame();
        }

        public void DebugRender(Camera camera)
        {
            foreach (Entity entity in entities) entity.DebugRender(camera);
        }

        public void HandleGraphicsReset()
        {
            foreach (Entity entity in entities) entity.HandleGraphicsReset();
        }

        public void HandleGraphicsCreate()
        {
            foreach (Entity entity in entities) entity.HandleGraphicsCreate();
        }
    }
}