using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace Crimson
{
    public static class EntityExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity SetPosition(this Entity self, Vector2 position)
        {
            self.Position = position;
            return self;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity SetPosition(this Entity self, float x, float y)
        {
            self.Position.X = x;
            self.Position.Y = y;
            return self;
        }
    }
}