using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;

namespace Crimson
{
    public static class ContentManagerExt
    {
        public static Task<T> LoadAsync<T>(this ContentManager ctx, string assetName)
        {
            return Task.Run(() => ctx.Load<T>(assetName));
        }

        public static Task<T[]> LoadAsync<T>(this ContentManager ctx, string[] assetNames)
        {
            return Task.Run(() =>
            {
                T[] results = new T[assetNames.Length];
                
                for (var i = 0; i < assetNames.Length; ++i)
                    results[i] = ctx.Load<T>(assetNames[i]);

                return results;
            });
        }
    }
}