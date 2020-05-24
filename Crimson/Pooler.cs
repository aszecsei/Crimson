using System;
using System.Collections.Generic;
using System.Reflection;

namespace Crimson
{
    public class Pooler
    {
        public Pooler()
        {
            Pools = new Dictionary<Type, Queue<Entity>>();

            foreach (Type type in Assembly.GetEntryAssembly().GetTypes())
                if (type.GetCustomAttributes(typeof(Pooled), false).Length > 0)
                {
                    if (!typeof(Entity).IsAssignableFrom(type))
                        throw new Exception("Type '" + type.Name +
                                            "' cannot be Pooled because it doesn't derive from Entity");

                    if (type.GetConstructor(Type.EmptyTypes) == null)
                        throw new Exception("Type '" + type.Name +
                                            "' cannot be Pooled because it doesn't have a parameterless constructor");

                    Pools.Add(type, new Queue<Entity>());
                }
        }

        internal Dictionary<Type, Queue<Entity>> Pools { get; }

        public T Create<T>() where T : Entity, new()
        {
            if (!Pools.ContainsKey(typeof(T))) return new T();

            Queue<Entity> queue = Pools[typeof(T)];
            if (queue.Count == 0) return new T();

            return queue.Dequeue() as T;
        }

        internal void EntityRemoved(Entity entity)
        {
            Type type = entity.GetType();
            if (Pools.ContainsKey(type)) Pools[type].Enqueue(entity);
        }

        public void Log()
        {
            if (Pools.Count == 0) Engine.Commands.Log("No Entity types are marked as Pooled!");

            foreach (var kv in Pools)
            {
                string output = kv.Key.Name + " : " + kv.Value.Count;
                Engine.Commands.Log(output);
            }
        }
    }

    public class Pooled : Attribute
    {
    }
}