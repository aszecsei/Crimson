using System;
using System.Collections.Generic;
using Crimson.Tweening.Plugins.Options;
using Microsoft.Xna.Framework;

namespace Crimson.Tweening.Plugins
{
    internal static class PluginsManager
    {
        private static Dictionary<Type, IBaseTweenPlugin> s_defaultPlugins = new Dictionary<Type, IBaseTweenPlugin>
        {
            { typeof(float), new FloatPlugin() },
            { typeof(double), new DoublePlugin() },
            { typeof(int), new IntPlugin() },
            { typeof(uint), new UintPlugin() },
            { typeof(long), new LongPlugin() },
            { typeof(ulong), new UlongPlugin() },
            { typeof(Vector2), new Vector2Plugin() },
            { typeof(Vector3), new Vector3Plugin() },
            { typeof(Vector4), new Vector4Plugin() },
            { typeof(Quaternion), new QuaternionPlugin() },
            { typeof(Color), new ColorPlugin() },
            { typeof(Rectangle), new RectPlugin() },
            { typeof(FRect), new FRectPlugin() },
            { typeof(string), new StringPlugin() },
        };

        private const int MAX_CUSTOM_PLUGINS = 20;
        private static Dictionary<Type, IBaseTweenPlugin> s_customPlugins = new Dictionary<Type, IBaseTweenPlugin>(MAX_CUSTOM_PLUGINS);

        internal static ITweenPlugin<T, TPlugOptions>? GetDefaultPlugin<T, TPlugOptions>()
            where TPlugOptions : struct, IPlugOptions
        {
            Type t = typeof(T);

            if (s_defaultPlugins.TryGetValue(t, out IBaseTweenPlugin plugin))
            {
                return plugin as ITweenPlugin<T, TPlugOptions>;
            }

            return null;
        }
        
        internal static ITweenPlugin<T, TPlugOptions>? GetCustomPlugin<TPlugin, T, TPlugOptions>()
            where TPlugin : IBaseTweenPlugin, new()
            where TPlugOptions : struct, IPlugOptions
        {
            Type t = typeof(T);

            IBaseTweenPlugin plugin;
            if (s_customPlugins.TryGetValue(t, out plugin))
            {
                return plugin as ITweenPlugin<T, TPlugOptions>;
            }

            plugin = new TPlugin();
            s_customPlugins.Add(t, plugin);
            return plugin as ITweenPlugin<T, TPlugOptions>;
        }
    }
}