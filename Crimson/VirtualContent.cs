using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class VirtualContent
    {
        private static List<VirtualAsset> s_assets = new List<VirtualAsset>();
        private static bool               s_reloading;

        public static int Count => s_assets.Count;

        public static VirtualTexture CreateTexture(string path)
        {
            VirtualTexture asset = new VirtualTexture(path);
            s_assets.Add(asset);
            return asset;
        }

        public static VirtualTexture CreateTexture(string name, int width, int height, Color color)
        {
            VirtualTexture asset = new VirtualTexture(name, width, height, color);
            s_assets.Add(asset);
            return asset;
        }

        public static VirtualRenderTarget CreateRenderTarget(
            string name,
            int    width,
            int    height,
            bool   depth            = false,
            bool   preserve         = true,
            int    multiSampleCount = 0
        )
        {
            VirtualRenderTarget asset = new VirtualRenderTarget(name, width, height, multiSampleCount, depth, preserve);
            s_assets.Add(asset);
            return asset;
        }

        public static void BySize()
        {
            Dictionary<int, Dictionary<int, int>> list = new Dictionary<int, Dictionary<int, int>>();
            foreach ( VirtualAsset asset in s_assets )
            {
                if ( !list.ContainsKey(asset.Width) )
                {
                    list.Add(asset.Width, new Dictionary<int, int>());
                }

                if ( !list[asset.Width].ContainsKey(asset.Height) )
                {
                    list[asset.Width].Add(asset.Height, 0);
                }

                list[asset.Width][asset.Height]++;
            }

            foreach ( var a in list )
            {
                foreach ( var b in a.Value )
                {
                    Console.WriteLine(a.Key + "x" + b.Key + ": " + b.Value);
                }
            }
        }

        public static void ByName()
        {
            foreach ( VirtualAsset asset in s_assets )
            {
                Console.WriteLine(asset.Name + "[" + asset.Width + "x" + asset.Height + "]");
            }
        }

        internal static void Remove(VirtualAsset asset)
        {
            s_assets.Remove(asset);
        }

        internal static void Reload()
        {
            if ( s_reloading )
            {
                foreach ( VirtualAsset asset in s_assets )
                {
                    asset.Reload();
                }
            }

            s_reloading = false;
        }

        internal static void Unload()
        {
            foreach ( VirtualAsset asset in s_assets )
            {
                asset.Unload();
            }

            s_reloading = true;
        }
    }
}
