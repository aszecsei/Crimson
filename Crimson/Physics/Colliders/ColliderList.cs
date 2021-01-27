using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Crimson.Physics
{
    public class ColliderList : Collider
    {
        public ColliderList(params Collider[] colliders)
        {
#if DEBUG
            foreach ( Collider c in colliders )
                if ( c == null )
                    throw new Exception("Cannot add a null Collider to a ColliderList.");
#endif
            this.colliders = colliders;
        }

        public Collider[] colliders { get; private set; }

        public override float Width
        {
            get { return Right - Left; }

            set { throw new NotImplementedException(); }
        }

        public override float Height
        {
            get { return Bottom - Top; }
            set { throw new NotImplementedException(); }
        }

        public override float Left
        {
            get
            {
                float left = colliders[0].Left;
                for ( var i = 1; i < colliders.Length; i++ )
                    if ( colliders[i].Left < left )
                        left = colliders[i].Left;
                return left;
            }

            set
            {
                float changeX = value - Left;
                foreach ( Collider c in colliders )
                    Position.X += changeX;
            }
        }

        public override float Right
        {
            get
            {
                float right = colliders[0].Right;
                for ( var i = 1; i < colliders.Length; i++ )
                    if ( colliders[i].Right > right )
                        right = colliders[i].Right;
                return right;
            }

            set
            {
                float changeX = value - Right;
                foreach ( Collider c in colliders )
                    Position.X += changeX;
            }
        }

        public override float Top
        {
            get
            {
                float top = colliders[0].Top;
                for ( var i = 1; i < colliders.Length; i++ )
                    if ( colliders[i].Top < top )
                        top = colliders[i].Top;
                return top;
            }

            set
            {
                float changeY = value - Top;
                foreach ( Collider c in colliders )
                    Position.Y += changeY;
            }
        }

        public override float Bottom
        {
            get
            {
                float bottom = colliders[0].Bottom;
                for ( var i = 1; i < colliders.Length; i++ )
                    if ( colliders[i].Bottom > bottom )
                        bottom = colliders[i].Bottom;
                return bottom;
            }

            set
            {
                float changeY = value - Bottom;
                foreach ( Collider c in colliders )
                    Position.Y += changeY;
            }
        }

        public void Add(params Collider[] toAdd)
        {
#if DEBUG
            foreach ( Collider c in toAdd )
            {
                if ( colliders.Contains(c) )
                    throw new Exception("Adding a Collider to a ColliderList that already contains it!");
                if ( c == null )
                    throw new Exception("Cannot add a null Collider to a ColliderList.");
            }
#endif

            var newColliders = new Collider[colliders.Length + toAdd.Length];
            for ( var i = 0; i < colliders.Length; i++ )
                newColliders[i] = colliders[i];
            for ( var i = 0; i < toAdd.Length; i++ )
            {
                newColliders[i + colliders.Length] = toAdd[i];
                toAdd[i].Added(Entity);
            }

            colliders = newColliders;
        }

        public void Remove(params Collider[] toRemove)
        {
#if DEBUG
            foreach ( Collider c in toRemove )
            {
                if ( !colliders.Contains(c) )
                    throw new Exception("Removing a Collider from a ColliderList that does not contain it!");
                if ( c == null )
                    throw new Exception("Cannot remove a null Collider from a ColliderList.");
            }
#endif

            var newColliders = new Collider[colliders.Length - toRemove.Length];
            var at = 0;
            foreach ( Collider c in colliders )
            {
                if ( !toRemove.Contains(c) )
                {
                    newColliders[at] = c;
                    at++;
                }
            }

            colliders = newColliders;
        }

        internal override void Added(Entity entity)
        {
            base.Added(entity);
            foreach ( Collider c in colliders )
                c.Added(entity);
        }

        internal override void Removed()
        {
            base.Removed();
            foreach ( Collider c in colliders )
                c.Removed();
        }

        public override Collider Clone()
        {
            var clones = new Collider[colliders.Length];
            for ( var i = 0; i < colliders.Length; i++ )
                clones[i] = colliders[i].Clone();

            return new ColliderList(clones);
        }

        public override void Render(Camera camera, Color color)
        {
            foreach ( Collider c in colliders )
                c.Render(camera, color);
        }

        /*
         *  Checking against other colliders
         */

        public override bool Collide(Vector2 point)
        {
            foreach ( Collider c in colliders )
                if ( c.Collide(point) )
                    return true;

            return false;
        }

        public override bool Collide(Rectangle rect)
        {
            foreach ( Collider c in colliders )
                if ( c.Collide(rect) )
                    return true;

            return false;
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            foreach ( Collider c in colliders )
                if ( c.Collide(from, to) )
                    return true;

            return false;
        }

        public override bool Collide(BoxCollider hitbox)
        {
            foreach ( Collider c in colliders )
                if ( c.Collide(hitbox) )
                    return true;

            return false;
        }

        public override bool Collide(GridCollider grid)
        {
            foreach ( Collider c in colliders )
                if ( c.Collide(grid) )
                    return true;

            return false;
        }

        public override bool Collide(CircleCollider circle)
        {
            foreach ( Collider c in colliders )
                if ( c.Collide(circle) )
                    return true;

            return false;
        }

        public override bool Collide(ColliderList list)
        {
            foreach ( Collider c in colliders )
                if ( c.Collide(list) )
                    return true;

            return false;
        }
    }
}
