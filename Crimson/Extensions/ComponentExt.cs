using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Crimson
{
    public static class ComponentExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AddComponent<T>(this Component self, T component) where T : Component
        {
            self.Entity.AddComponent(component);
            return component;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T AddComponent<T>(this Component self) where T : Component, new()
        {
            return self.AddComponent(new T());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetComponent<T>(this Component self) where T : Component
        {
            return self.Entity.GetComponent<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetComponents<T>(this Component self, List<T> componentList) where T : Component
        {
            self.Entity.GetComponents(componentList);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> GetComponents<T>(this Component self) where T : Component
        {
            return self.Entity.GetComponents<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveComponent<T>(this Component self) where T : Component
        {
            return self.Entity.RemoveComponent<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveComponent<T>(this Component self, T component) where T : Component
        {
            return self.Entity.RemoveComponent(component);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveComponent(this Component self)
        {
            self.Entity.RemoveComponent(self);
        }
    }
}