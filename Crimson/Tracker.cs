using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public class Tracker
    {
        public Tracker()
        {
            Entities = new Dictionary<Type, List<Entity>>(TrackedEntityTypes.Count);
            foreach (Type type in StoredEntityTypes)
                Entities.Add(type, new List<Entity>());

            Components = new Dictionary<Type, List<Component>>(TrackedComponentTypes.Count);
            foreach (Type type in StoredComponentTypes)
                Components.Add(type, new List<Component>());

            CollidableComponents = new Dictionary<Type, List<Collider2D>>(TrackedComponentTypes.Count);
            foreach (Type type in StoredCollidableComponentTypes)
                CollidableComponents.Add(type, new List<Collider2D>());
        }

        public Dictionary<Type, List<Entity>> Entities { get; }
        public Dictionary<Type, List<Component>> Components { get; }
        public Dictionary<Type, List<Collider2D>> CollidableComponents { get; }

        public bool IsEntityTracked<T>() where T : Entity
        {
            return Entities.ContainsKey(typeof(T));
        }

        public bool IsComponentTracked<T>() where T : Component
        {
            return Components.ContainsKey(typeof(T)) || CollidableComponents.ContainsKey(typeof(T));
        }

        public T? GetEntity<T>() where T : Entity
        {
#if DEBUG
            if (!IsEntityTracked<T>())
                throw new Exception("Entity type '" + typeof(T).Name + "' is not marked with the Tracked attribute!");
#endif

            List<Entity> list = Entities[typeof(T)];
            if (list.Count == 0)
                return null;
            return list[0] as T;
        }

        public T? GetNearestEntity<T>(Vector2 nearestTo) where T : Entity
        {
            List<Entity> list = GetEntities<T>();

            T? nearest = null;
            float nearestDistSq = 0;

            foreach (T entity in list)
            {
                var distSq = Vector2.DistanceSquared(nearestTo, entity.Position);

                if (nearest == null || distSq < nearestDistSq)
                {
                    nearest = entity;
                    nearestDistSq = distSq;
                }
            }

            return nearest;
        }

        public T? GetNearestEntityNot<T>(Vector2 nearestTo, T exclude) where T : Entity
        {
            List<Entity> list = GetEntities<T>();

            T? nearest = null;
            float nearestDistSq = 0;

            foreach (T entity in list)
            {
                if (entity == exclude)
                    continue;

                var distSq = Vector2.DistanceSquared(nearestTo, entity.Position);

                if (nearest == null || distSq < nearestDistSq)
                {
                    nearest = entity;
                    nearestDistSq = distSq;
                }
            }

            return nearest;
        }

        public List<Entity> GetEntities<T>() where T : Entity
        {
#if DEBUG
            if (!IsEntityTracked<T>())
                throw new Exception("Entity type '" + typeof(T).Name + "' is not marked with the Tracked attribute!");
#endif

            return Entities[typeof(T)];
        }

        public List<Entity> GetEntitiesCopy<T>() where T : Entity
        {
            return new List<Entity>(GetEntities<T>());
        }

        public IEnumerator<T> EnumerateEntities<T>() where T : Entity
        {
#if DEBUG
            if (!IsEntityTracked<T>())
                throw new Exception("Entity type '" + typeof(T).Name + "' is not marked with the Tracked attribute!");
#endif

            foreach (Entity e in Entities[typeof(T)])
                yield return e as T;
        }

        public int CountEntities<T>() where T : Entity
        {
#if DEBUG
            if (!IsEntityTracked<T>())
                throw new Exception("Entity type '" + typeof(T).Name + "' is not marked with the Tracked attribute!");
#endif

            return Entities[typeof(T)].Count;
        }

        public T? GetComponent<T>() where T : Component
        {
#if DEBUG
            if (!IsComponentTracked<T>())
                throw new Exception("Component type '" + typeof(T).Name +
                                    "' is not marked with the Tracked attribute!");
#endif

            List<Component> list = Components[typeof(T)];
            if (list.Count == 0)
                return null;
            return list[0] as T;
        }

        public T? GetNearestComponent<T>(Vector2 nearestTo) where T : Component
        {
            List<Component> list = GetComponents<T>();

            T? nearest = null;
            float nearestDistSq = 0;

            foreach (T component in list)
            {
                var distSq = Vector2.DistanceSquared(nearestTo, component.Entity.Position);

                if (nearest == null || distSq < nearestDistSq)
                {
                    nearest = component;
                    nearestDistSq = distSq;
                }
            }

            return nearest;
        }

        public List<Component> GetComponents<T>() where T : Component
        {
#if DEBUG
            if (!IsComponentTracked<T>())
                throw new Exception("Component type '" + typeof(T).Name +
                                    "' is not marked with the Tracked attribute!");
#endif

            return Components[typeof(T)];
        }

        public List<Component> GetComponentsCopy<T>() where T : Component
        {
            return new List<Component>(GetComponents<T>());
        }

        public IEnumerator<T> EnumerateComponents<T>() where T : Component
        {
#if DEBUG
            if (!IsComponentTracked<T>())
                throw new Exception("Component type '" + typeof(T).Name +
                                    "' is not marked with the Tracked attribute!");
#endif

            foreach (Component c in Components[typeof(T)])
                yield return c as T;
        }

        public int CountComponents<T>() where T : Component
        {
#if DEBUG
            if (!IsComponentTracked<T>())
                throw new Exception("Component type '" + typeof(T).Name +
                                    "' is not marked with the Tracked attribute!");
#endif

            return Components[typeof(T)].Count;
        }

        internal void EntityAdded(Entity entity)
        {
            Type type = entity.GetType();
            List<Type> trackAs;

            if (TrackedEntityTypes.TryGetValue(type, out trackAs))
                foreach (Type track in trackAs)
                    Entities[track].Add(entity);
        }

        internal void EntityRemoved(Entity entity)
        {
            Type type = entity.GetType();
            List<Type> trackAs;

            if (TrackedEntityTypes.TryGetValue(type, out trackAs))
                foreach (Type track in trackAs)
                    Entities[track].Remove(entity);
        }

        internal void ComponentAdded(Component component)
        {
            Type type = component.GetType();
            List<Type> trackAs;

            if (TrackedComponentTypes.TryGetValue(type, out trackAs))
                foreach (Type track in trackAs)
                    Components[track].Add(component);
            if (TrackedCollidableComponentTypes.TryGetValue(type, out trackAs))
                foreach (Type track in trackAs)
                    CollidableComponents[track].Add(component as Collider2D);
        }

        internal void ComponentRemoved(Component component)
        {
            Type type = component.GetType();
            List<Type> trackAs;

            if (TrackedComponentTypes.TryGetValue(type, out trackAs))
                foreach (Type track in trackAs)
                    Components[track].Remove(component);
            if (TrackedCollidableComponentTypes.TryGetValue(type, out trackAs))
                foreach (Type track in trackAs)
                    CollidableComponents[track].Remove(component as Collider2D);
        }

        public void LogEntities()
        {
            if (Entities.Count == 0)
                Engine.Commands.Log("n/a", Color.Red);
            else
                foreach (var kv in Entities)
                {
                    string output = kv.Key.Name + " : " + kv.Value.Count;
                    Engine.Commands.Log(output);
                }
        }

        public void LogComponents()
        {
            if (Components.Count == 0)
                Engine.Commands.Log("n/a", Color.Red);
            else
                foreach (var kv in Components)
                {
                    string output = kv.Key.Name + " : " + kv.Value.Count;
                    Engine.Commands.Log(output);
                }
        }

        public void LogCollidableComponents()
        {
            if (CollidableComponents.Count == 0)
                Engine.Commands.Log("n/a", Color.Red);
            else
                foreach (var kv in CollidableComponents)
                {
                    string output = kv.Key.Name + " : " + kv.Value.Count;
                    Engine.Commands.Log(output);
                }
        }

        #region Static

        public static Dictionary<Type, List<Type>> TrackedEntityTypes { get; private set; }
        public static Dictionary<Type, List<Type>> TrackedComponentTypes { get; private set; }
        public static Dictionary<Type, List<Type>> TrackedCollidableComponentTypes { get; private set; }
        public static HashSet<Type> StoredEntityTypes { get; private set; }
        public static HashSet<Type> StoredComponentTypes { get; private set; }
        public static HashSet<Type> StoredCollidableComponentTypes { get; private set; }

        public static void Initialize()
        {
            TrackedEntityTypes = new Dictionary<Type, List<Type>>();
            TrackedComponentTypes = new Dictionary<Type, List<Type>>();
            TrackedCollidableComponentTypes = new Dictionary<Type, List<Type>>();
            StoredEntityTypes = new HashSet<Type>();
            StoredComponentTypes = new HashSet<Type>();
            StoredCollidableComponentTypes = new HashSet<Type>();

            foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
            {
                object[] attrs = type.GetCustomAttributes(typeof(Tracked), false);
                if (attrs.Length > 0)
                {
                    var inherited = (attrs[0] as Tracked).Inherited;

                    if (typeof(Entity).IsAssignableFrom(type))
                    {
                        if (!type.IsAbstract)
                        {
                            if (!TrackedEntityTypes.ContainsKey(type))
                                TrackedEntityTypes.Add(type, new List<Type>());
                            TrackedEntityTypes[type].Add(type);
                        }

                        StoredEntityTypes.Add(type);

                        if (inherited)
                            foreach (Type subclass in GetSubclasses(type))
                                if (!subclass.IsAbstract)
                                {
                                    if (!TrackedEntityTypes.ContainsKey(subclass))
                                        TrackedEntityTypes.Add(subclass, new List<Type>());
                                    TrackedEntityTypes[subclass].Add(type);
                                }
                    }
                    else if (typeof(Component).IsAssignableFrom(type))
                    {
                        if (!type.IsAbstract)
                        {
                            if (!TrackedComponentTypes.ContainsKey(type))
                                TrackedComponentTypes.Add(type, new List<Type>());
                            TrackedComponentTypes[type].Add(type);
                        }

                        StoredComponentTypes.Add(type);

                        if (inherited)
                            foreach (Type subclass in GetSubclasses(type))
                                if (!subclass.IsAbstract)
                                {
                                    if (!TrackedComponentTypes.ContainsKey(subclass))
                                        TrackedComponentTypes.Add(subclass, new List<Type>());
                                    TrackedComponentTypes[subclass].Add(type);
                                }

                        if (typeof(Collider2D).IsAssignableFrom(type))
                        {
                            if (!type.IsAbstract)
                            {
                                if (!TrackedCollidableComponentTypes.ContainsKey(type))
                                    TrackedCollidableComponentTypes.Add(type, new List<Type>());
                                TrackedCollidableComponentTypes[type].Add(type);
                            }

                            StoredCollidableComponentTypes.Add(type);

                            if (inherited)
                                foreach (Type subclass in GetSubclasses(type))
                                    if (!subclass.IsAbstract)
                                    {
                                        if (!TrackedCollidableComponentTypes.ContainsKey(subclass))
                                            TrackedCollidableComponentTypes.Add(subclass, new List<Type>());
                                        TrackedCollidableComponentTypes[subclass].Add(type);
                                    }
                        }
                    }
                    else
                    {
                        throw new Exception("Type '" + type.Name +
                                            "' cannot be Tracked because it does not derive from Entity or Component");
                    }
                }
            }
        }

        private static List<Type> GetSubclasses(Type type)
        {
            var matches = new List<Type>();

            foreach (Type check in Assembly.GetEntryAssembly().GetTypes())
                if (type != check && type.IsAssignableFrom(check))
                    matches.Add(check);

            return matches;
        }

        #endregion
    }

    public class Tracked : Attribute
    {
        public bool Inherited;

        public Tracked(bool inherited = false)
        {
            Inherited = inherited;
        }
    }
}