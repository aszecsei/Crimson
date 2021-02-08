using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Crimson.Physics
{
    public class PhysicsEntity : Entity
    {
        public bool Collidable = true;
        private Collider? _collider;

        public Vector2 LiftSpeed = Vector2.Zero;

        public PhysicsEntity(Vector2 position) : base(position)
        {
        }

        public PhysicsEntity() : base()
        {
        }

        public override void DebugRender(Camera camera)
        {
            base.DebugRender(camera);

            if ( Collider != null )
                Collider.Render(camera, Collidable ? Color.Red : Color.DarkRed);
        }

        #region Collider

        public Collider? Collider
        {
            get { return _collider; }
            set
            {
                if ( value == _collider )
                    return;

#if DEBUG
                if ( value?.Entity != null )
                    throw new Exception("Setting an Entity's Collider to a Collider already in use by another object");
#endif
                if ( _collider != null )
                    _collider.Removed();
                _collider = value;
                if ( _collider != null )
                    _collider.Added(this);
            }
        }

        public float Width
        {
            get
            {
                if ( _collider == null )
                    return 0;
                return _collider.Width;
            }
        }

        public float Height
        {
            get
            {
                if ( _collider == null )
                    return 0;
                return _collider.Height;
            }
        }

        public float Left
        {
            get
            {
                if ( _collider == null )
                    return X;
                return Position.X + _collider.Left;
            }

            set
            {
                if ( _collider == null )
                    Position.X = value;
                else
                    Position.X = value - _collider.Left;
            }
        }

        public float Right
        {
            get
            {
                if ( _collider == null )
                    return Position.X;
                return Position.X + _collider.Right;
            }

            set
            {
                if ( _collider == null )
                    Position.X = value;
                else
                    Position.X = value - _collider.Right;
            }
        }

        public float Top
        {
            get
            {
                if ( _collider == null )
                    return Position.Y;
                return Position.Y + _collider.Top;
            }

            set
            {
                if ( _collider == null )
                    Position.Y = value;
                else
                    Position.Y = value - _collider.Top;
            }
        }

        public float Bottom
        {
            get
            {
                if ( _collider == null )
                    return Position.Y;
                return Position.Y + _collider.Bottom;
            }

            set
            {
                if ( _collider == null )
                    Position.Y = value;
                else
                    Position.Y = value - _collider.Bottom;
            }
        }

        public float CenterX
        {
            get
            {
                if ( _collider == null )
                    return Position.X;
                return Position.X + _collider.CenterX;
            }

            set
            {
                if ( _collider == null )
                    Position.X = value;
                else
                    Position.X = value - _collider.CenterX;
            }
        }

        public float CenterY
        {
            get
            {
                if ( _collider == null )
                    return Position.Y;
                return Position.Y + _collider.CenterY;
            }

            set
            {
                if ( _collider == null )
                    Position.Y = value;
                else
                    Position.Y = value - _collider.CenterY;
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

        #endregion

        #region Collision Shortcuts

        #region Collide Check

        public bool CollideCheck(PhysicsEntity other)
        {
            return Collide.Check(this, other);
        }

        public bool CollideCheck(PhysicsEntity other, Vector2 at)
        {
            return Collide.Check(this, other, at);
        }

        public bool CollideCheck(CollidableComponent other)
        {
            return Collide.Check(this, other);
        }

        public bool CollideCheck(CollidableComponent other, Vector2 at)
        {
            return Collide.Check(this, other, at);
        }

        public bool CollideCheck(BitTag tag)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            List<PhysicsEntity> entities = new List<PhysicsEntity>();
            foreach (Entity e in Scene[tag])
            {
                if (e is PhysicsEntity pe)
                {
                    entities.Add(pe);
                }
            }
            return Collide.Check(this, entities);
        }

        public bool CollideCheck(BitTag tag, Vector2 at)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            List<PhysicsEntity> entities = new List<PhysicsEntity>();
            foreach ( Entity e in Scene[tag] )
            {
                if ( e is PhysicsEntity pe )
                {
                    entities.Add(pe);
                }
            }
            return Collide.Check(this, entities, at);
        }

        public bool CollideCheck<T>() where T : Entity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            return Collide.Check(this, Scene.Tracker.Entities[typeof(T)].PhysicsEntities());
        }

        public bool CollideCheck<T>(Vector2 at) where T : Entity
        {
            return Collide.Check(this, Scene.Tracker.Entities[typeof(T)].PhysicsEntities(), at);
        }

        public bool CollideCheck<T, Exclude>() where T : Entity where Exclude : Entity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked objects when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(Exclude)) )
                throw new Exception("Excluded type is an untracked Entity type!");
#endif

            List<Entity> exclude = Scene.Tracker.Entities[typeof(Exclude)];
            foreach ( PhysicsEntity e in Scene.Tracker.Entities[typeof(T)].PhysicsEntities() )
                if ( !exclude.Contains(e) )
                    if ( Collide.Check(this, e) )
                        return true;
            return false;
        }

        public bool CollideCheck<T, Exclude>(Vector2 at) where T : Entity where Exclude : Entity
        {
            Vector2 was = Position;
            Position = at;
            bool ret = CollideCheck<T, Exclude>();
            Position = was;
            return ret;
        }

        public bool CollideCheckByComponent<T>() where T : CollidableComponent
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            if ( !Scene.Tracker.Components.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach ( CollidableComponent c in Scene.Tracker.CollidableComponents[typeof(T)] )
                if ( Collide.Check(this, c) )
                    return true;
            return false;
        }

        public bool CollideCheckByComponent<T>(Vector2 at) where T : CollidableComponent
        {
            Vector2 old = Position;
            Position = at;
            bool ret = CollideCheckByComponent<T>();
            Position = old;
            return ret;
        }

        #endregion

        #region Collide CheckOutside

        public bool CollideCheckOutside(PhysicsEntity other, Vector2 at)
        {
            return !Collide.Check(this, other) && Collide.Check(this, other, at);
        }

        public bool CollideCheckOutside(BitTag tag, Vector2 at)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            foreach ( PhysicsEntity entity in Scene[tag].PhysicsEntities() )
                if ( !Collide.Check(this, entity) && Collide.Check(this, entity, at) )
                    return true;

            return false;
        }

        public bool CollideCheckOutside<T>(Vector2 at) where T : Entity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            foreach ( PhysicsEntity entity in Scene.Tracker.Entities[typeof(T)].PhysicsEntities() )
                if ( !Collide.Check(this, entity) && Collide.Check(this, entity, at) )
                    return true;
            return false;
        }

        public bool CollideCheckOutsideByComponent<T>(Vector2 at) where T : CollidableComponent
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            if ( !Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach ( CollidableComponent component in Scene.Tracker.CollidableComponents[typeof(T)] )
                if ( !Collide.Check(this, component) && Collide.Check(this, component, at) )
                    return true;
            return false;
        }

        #endregion

        #region Collide First

        public Entity CollideFirst(BitTag tag)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.First(this, Scene[tag].PhysicsEntities());
        }

        public Entity CollideFirst(BitTag tag, Vector2 at)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.First(this, Scene[tag].PhysicsEntities(), at);
        }

        public T CollideFirst<T>() where T : Entity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif
            return Collide.First(this, Scene.Tracker.Entities[typeof(T)].PhysicsEntities()) as T;
        }

        public T CollideFirst<T>(Vector2 at) where T : Entity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif
            return Collide.First(this, Scene.Tracker.Entities[typeof(T)].PhysicsEntities(), at) as T;
        }

        public T CollideFirstByComponent<T>() where T : CollidableComponent
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            if ( !Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach ( CollidableComponent component in Scene.Tracker.CollidableComponents[typeof(T)] )
                if ( Collide.Check(this, component) )
                    return component as T;
            return null;
        }

        public T CollideFirstByComponent<T>(Vector2 at) where T : CollidableComponent
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            if ( !Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach ( CollidableComponent component in Scene.Tracker.CollidableComponents[typeof(T)] )
                if ( Collide.Check(this, component, at) )
                    return component as T;
            return null;
        }

        #endregion

        #region Collide FirstOutside

        public Entity CollideFirstOutside(BitTag tag, Vector2 at)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            foreach ( PhysicsEntity entity in Scene[tag].PhysicsEntities() )
                if ( !Collide.Check(this, entity) && Collide.Check(this, entity, at) )
                    return entity;
            return null;
        }

        public T CollideFirstOutside<T>(Vector2 at) where T : Entity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            foreach ( PhysicsEntity entity in Scene.Tracker.Entities[typeof(T)].PhysicsEntities() )
                if ( !Collide.Check(this, entity) && Collide.Check(this, entity, at) )
                    return entity as T;
            return null;
        }

        public T CollideFirstOutsideByComponent<T>(Vector2 at) where T : CollidableComponent
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            if ( !Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            foreach ( CollidableComponent component in Scene.Tracker.CollidableComponents[typeof(T)] )
                if ( !Collide.Check(this, component) && Collide.Check(this, component, at) )
                    return component as T;
            return null;
        }

        #endregion

        #region Collide All

        public List<PhysicsEntity> CollideAll(BitTag tag)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.All(this, Scene[tag].PhysicsEntities());
        }

        public List<PhysicsEntity> CollideAll(BitTag tag, Vector2 at)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif
            return Collide.All(this, Scene[tag].PhysicsEntities(), at);
        }

        public List<PhysicsEntity> CollideAll<T>() where T : PhysicsEntity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            return Collide.All(this, Scene.Tracker.Entities[typeof(T)].PhysicsEntities());
        }

        public List<PhysicsEntity> CollideAll<T>(Vector2 at) where T : Entity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            return Collide.All(this, Scene.Tracker.Entities[typeof(T)].PhysicsEntities(), at);
        }

        public List<PhysicsEntity> CollideAll<T>(Vector2 at, List<PhysicsEntity> into) where T : Entity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            into.Clear();
            return Collide.All(this, Scene.Tracker.Entities[typeof(T)].PhysicsEntities(), into, at);
        }

        public List<T> CollideAllByComponent<T>() where T : CollidableComponent
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            if ( !Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            var list = new List<T>();
            foreach ( CollidableComponent component in Scene.Tracker.CollidableComponents[typeof(T)] )
                if ( Collide.Check(this, component) )
                    list.Add(component as T);
            return list;
        }

        public List<T> CollideAllByComponent<T>(Vector2 at) where T : CollidableComponent
        {
            Vector2 old = Position;
            Position = at;
            List<T> ret = CollideAllByComponent<T>();
            Position = old;
            return ret;
        }

        #endregion

        #region Collide Do

        public bool CollideDo(BitTag tag, Action<PhysicsEntity> action)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            var hit = false;
            foreach ( PhysicsEntity other in Scene[tag].PhysicsEntities() )
            {
                if ( CollideCheck(other) )
                {
                    action(other);
                    hit = true;
                }
            }

            return hit;
        }

        public bool CollideDo(BitTag tag, Action<PhysicsEntity> action, Vector2 at)
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against a tag list when it is not a member of a Scene");
#endif

            var hit = false;
            Vector2 was = Position;
            Position = at;

            foreach ( PhysicsEntity other in Scene[tag].PhysicsEntities() )
            {
                if ( CollideCheck(other) )
                {
                    action(other);
                    hit = true;
                }
            }

            Position = was;
            return hit;
        }

        public bool CollideDo<T>(Action<T> action) where T : PhysicsEntity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            var hit = false;
            foreach ( PhysicsEntity other in Scene.Tracker.Entities[typeof(T)].PhysicsEntities() )
            {
                if ( CollideCheck(other) )
                {
                    action(other as T);
                    hit = true;
                }
            }

            return hit;
        }

        public bool CollideDo<T>(Action<T> action, Vector2 at) where T : PhysicsEntity
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked Entities when it is not a member of a Scene");
            if ( !Scene.Tracker.Entities.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked Entity type");
#endif

            var hit = false;
            Vector2 was = Position;
            Position = at;

            foreach ( PhysicsEntity other in Scene.Tracker.Entities[typeof(T)].PhysicsEntities() )
            {
                if ( CollideCheck(other) )
                {
                    action(other as T);
                    hit = true;
                }
            }

            Position = was;
            return hit;
        }

        public bool CollideDoByComponent<T>(Action<T> action) where T : CollidableComponent
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            if ( !Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            var hit = false;
            foreach ( CollidableComponent component in Scene.Tracker.CollidableComponents[typeof(T)] )
            {
                if ( CollideCheck(component) )
                {
                    action(component as T);
                    hit = true;
                }
            }

            return hit;
        }

        public bool CollideDoByComponent<T>(Action<T> action, Vector2 at) where T : CollidableComponent
        {
#if DEBUG
            if ( Scene == null )
                throw new Exception(
                    "Can't collide check an Entity against tracked CollidableComponents when it is not a member of a Scene");
            if ( !Scene.Tracker.CollidableComponents.ContainsKey(typeof(T)) )
                throw new Exception("Can't collide check an Entity against an untracked CollidableComponent type");
#endif

            var hit = false;
            Vector2 was = Position;
            Position = at;

            foreach ( CollidableComponent component in Scene.Tracker.CollidableComponents[typeof(T)] )
            {
                if ( CollideCheck(component) )
                {
                    action(component as T);
                    hit = true;
                }
            }

            Position = was;
            return hit;
        }

        #endregion

        #region Collide Geometry

        public bool CollidePoint(Vector2 point)
        {
            return Collide.CheckPoint(this, point);
        }

        public bool CollidePoint(Vector2 point, Vector2 at)
        {
            return Collide.CheckPoint(this, point, at);
        }

        public bool CollideLine(Vector2 from, Vector2 to)
        {
            return Collide.CheckLine(this, from, to);
        }

        public bool CollideLine(Vector2 from, Vector2 to, Vector2 at)
        {
            return Collide.CheckLine(this, from, to, at);
        }

        public bool CollideRect(Rectangle rect)
        {
            return Collide.CheckRect(this, rect);
        }

        public bool CollideRect(Rectangle rect, Vector2 at)
        {
            return Collide.CheckRect(this, rect, at);
        }

        #endregion

        #endregion
    }
}
