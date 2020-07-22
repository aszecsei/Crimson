using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crimson.Collections;

namespace Crimson
{
    public class ComponentList : IEnumerable<Component>, IEnumerable
    {
        public enum LockModes
        {
            Open,
            Locked,
            Error
        }

        private readonly HashSet<Component> adding;

        private readonly List<Component> components;

        private readonly HashSet<Component> current;
        private readonly HashSet<Component> removing;
        private readonly List<Component> toAdd;
        private readonly List<Component> toRemove;

        private LockModes lockMode;

        internal ComponentList(Entity entity)
        {
            Entity = entity;

            components = new List<Component>();
            toAdd = new List<Component>();
            toRemove = new List<Component>();
            current = new HashSet<Component>();
            adding = new HashSet<Component>();
            removing = new HashSet<Component>();
        }

        public Entity Entity { get; internal set; }

        internal LockModes LockMode
        {
            get => lockMode;
            set
            {
                lockMode = value;

                if (toAdd.Count > 0)
                {
                    foreach (Component component in toAdd)
                        if (!current.Contains(component))
                        {
                            current.Add(component);
                            components.Add(component);
                            component.Added(Entity);
                        }

                    adding.Clear();
                    toAdd.Clear();
                }

                if (toRemove.Count > 0)
                {
                    foreach (Component component in toRemove)
                        if (current.Contains(component))
                        {
                            current.Remove(component);
                            components.Remove(component);
                            component.Removed(Entity);
                        }

                    removing.Clear();
                    toRemove.Clear();
                }
            }
        }

        public int Count => components.Count;

        public Component this[int index]
        {
            get
            {
                if (index < 0 || index >= components.Count) throw new IndexOutOfRangeException();

                return components[index];
            }
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Component component)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (!current.Contains(component))
                    {
                        current.Add(component);
                        components.Add(component);
                        component.Added(Entity);
                    }

                    break;
                case LockModes.Locked:
                    if (!current.Contains(component) && !adding.Contains(component))
                    {
                        adding.Add(component);
                        toAdd.Add(component);
                    }

                    break;
                case LockModes.Error:
                    throw new Exception("Cannot add or remove entities at this time!");
            }
        }

        public bool Remove(Component component)
        {
            switch (lockMode)
            {
                case LockModes.Open:
                    if (current.Contains(component))
                    {
                        current.Remove(component);
                        components.Remove(component);
                        component.Removed(Entity);
                        return true;
                    }

                    return false;
                case LockModes.Locked:
                    if (current.Contains(component) && !removing.Contains(component))
                    {
                        removing.Add(component);
                        toRemove.Add(component);
                        return true;
                    }

                    return false;
                case LockModes.Error:
                    throw new Exception("Cannot add or remove entities at this time!");
            }

            return false;
        }

        public void Add(IEnumerable<Component> components)
        {
            foreach (Component component in components) Add(component);
        }

        public void Remove(IEnumerable<Component> components)
        {
            foreach (Component component in components) Remove(component);
        }

        public void RemoveAll<T>() where T : Component
        {
            Remove(GetAll<T>());
        }

        public void Add(params Component[] components)
        {
            foreach (Component component in components) Add(component);
        }

        public void Remove(params Component[] components)
        {
            foreach (Component component in components) Remove(component);
        }

        public Component[] ToArray()
        {
            return components.ToArray<Component>();
        }

        internal void Update()
        {
            LockMode = LockModes.Locked;
            foreach (Component component in components)
                if (component.Active)
                    component.Update();

            LockMode = LockModes.Open;
        }

        internal void LateUpdate()
        {
            LockMode = LockModes.Locked;
            foreach (Component component in components)
                if (component.Active)
                    component.LateUpdate();

            LockMode = LockModes.Open;
        }

        internal void Render()
        {
            LockMode = LockModes.Error;
            foreach (Component component in components)
                if (component.Visible)
                    component.Render();

            LockMode = LockModes.Open;
        }

        internal void EndOfFrame()
        {
            LockMode = LockModes.Locked;
            foreach (Component component in components)
                if (component.Active)
                    component.EndOfFrame();
            LockMode = LockModes.Open;
        }

        internal void DebugRender(Camera camera)
        {
            LockMode = LockModes.Error;
            foreach (Component component in components) component.DebugRender(camera);

            LockMode = LockModes.Open;
        }

        internal void HandleGraphicsReset()
        {
            LockMode = LockModes.Error;
            foreach (Component component in components) component.HandleGraphicsReset();

            LockMode = LockModes.Open;
        }

        internal void HandleGraphicsCreate()
        {
            LockMode = LockModes.Error;
            foreach (Component component in components) component.HandleGraphicsCreate();

            LockMode = LockModes.Open;
        }

        public T? Get<T>() where T : Component
        {
            foreach (Component component in components)
                if (component is T)
                    return component as T;

            return null;
        }

        public List<T> GetAll<T>() where T : Component
        {
            List<T> results = ListPool<T>.Obtain();
            foreach (Component component in components)
                if (component is T componentT)
                    results.Add(componentT);
            return results;
        }

        public void GetAll<T>(List<T> list) where T : Component
        {
            list.Clear();
            foreach (Component component in components)
                if (component is T componentT)
                    list.Add(componentT);
        }
    }
}