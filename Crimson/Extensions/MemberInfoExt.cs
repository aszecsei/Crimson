using System;
using System.Reflection;

namespace Crimson
{
    public static class MemberInfoExt
    {
        public static T? GetCustomAttribute<T>(this MemberInfo self) where T : Attribute
        {
            var attributes = self.GetCustomAttributes(typeof(T));
            foreach (var attribute in attributes)
            {
                if (attribute.GetType() == typeof(T))
                    return (T) attribute;
            }

            return null;
        }
    }
}