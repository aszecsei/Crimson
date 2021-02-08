using System;
using Microsoft.Xna.Framework;

namespace Crimson.Physics
{
    [Tracked(true)]
    public class Platform : PhysicsEntity
    {
        public virtual bool Safe => true;
        public Action<Vector2>? OnCollide;

        public Platform(Vector2 position)
            : base(position) { }

        public Platform() { }
    }
}
