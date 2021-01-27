using System;
using Microsoft.Xna.Framework;

namespace Crimson.Physics
{
    public class CollidableComponent : Component
    {
        public bool Collidable;

        private Collider? collider;

        public PhysicsEntity? PhysicsEntity => Entity as PhysicsEntity;

        public CollidableComponent(bool active, bool visible, bool colllidable)
            : base(active, visible)
        {
            Collidable = colllidable;
        }

        public Collider? Collider
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity?.Collider;
                return collider;
            }

            set
            {
                if ( value == collider )
                    return;
#if DEBUG
                if ( value?.Entity != null )
                    throw new Exception("Setting an Entity's Collider to a Collider already in use by another object");
#endif
                if ( collider != null )
                    collider.Removed();
                collider = value;
                if ( collider != null )
                    collider.Added(this);
            }
        }

        public float Width
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity.Width;
                return collider.Width;
            }
        }

        public float Height
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity.Height;
                return collider.Height;
            }
        }

        public float Left
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity.Left;
                return Entity.X + collider.Left;
            }

            set
            {
                if ( collider == null )
                    PhysicsEntity.Left = value;
                else
                    Entity.X = value - collider.Left;
            }
        }

        public float Right
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity.Right;
                return Entity.X + collider.Right;
            }

            set
            {
                if ( collider == null )
                    PhysicsEntity.Right = value;
                else
                    PhysicsEntity.X = value - collider.Right;
            }
        }

        public float Top
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity.Top;
                return Entity.Y + collider.Top;
            }

            set
            {
                if ( collider == null )
                    PhysicsEntity.Top = value;
                else
                    PhysicsEntity.Y = value - collider.Top;
            }
        }

        public float Bottom
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity.Bottom;
                return Entity.Y + collider.Bottom;
            }

            set
            {
                if ( collider == null )
                    PhysicsEntity.Bottom = value;
                else
                    Entity.Y = value - collider.Bottom;
            }
        }

        public float CenterX
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity.CenterX;
                return Entity.X + collider.CenterX;
            }

            set
            {
                if ( collider == null )
                    PhysicsEntity.CenterX = value;
                else
                    Entity.X = value - collider.CenterX;
            }
        }

        public float CenterY
        {
            get
            {
                if ( collider == null )
                    return PhysicsEntity.CenterY;
                return Entity.Y + collider.CenterY;
            }

            set
            {
                if ( collider == null )
                    PhysicsEntity.CenterY = value;
                else
                    Entity.Y = value - collider.CenterY;
            }
        }

        public Vector2 TopLeft
        {
            get { return new Vector2(Left, Top); }

            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Vector2 TopRight
        {
            get { return new Vector2(Right, Top); }

            set
            {
                Right = value.X;
                Top = value.Y;
            }
        }

        public Vector2 BottomLeft
        {
            get { return new Vector2(Left, Bottom); }

            set
            {
                Left = value.X;
                Bottom = value.Y;
            }
        }

        public Vector2 BottomRight
        {
            get { return new Vector2(Right, Bottom); }

            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }

        public Vector2 Center
        {
            get { return new Vector2(CenterX, CenterY); }

            set
            {
                CenterX = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 CenterLeft
        {
            get { return new Vector2(Left, CenterY); }

            set
            {
                Left = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 CenterRight
        {
            get { return new Vector2(Right, CenterY); }

            set
            {
                Right = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 TopCenter
        {
            get { return new Vector2(CenterX, Top); }

            set
            {
                CenterX = value.X;
                Top = value.Y;
            }
        }

        public Vector2 BottomCenter
        {
            get { return new Vector2(CenterX, Bottom); }

            set
            {
                CenterX = value.X;
                Bottom = value.Y;
            }
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            if ( collider != null )
                collider.Entity = entity;
        }

        public override void Removed(Entity entity)
        {
            if ( collider != null )
                collider.Entity = null;
            base.Removed(entity);
        }
    }
}
