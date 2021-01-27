using Microsoft.Xna.Framework;

namespace Crimson.Physics
{
    public class CircleCollider : Collider
    {
        public float Radius;

        public CircleCollider(float radius, float x = 0, float y = 0)
        {
            Radius = radius;
            Position.X = x;
            Position.Y = y;
        }

        public override float Width
        {
            get { return Radius * 2; }
            set { Radius = value / 2; }
        }

        public override float Height
        {
            get { return Radius * 2; }
            set { Radius = value / 2; }
        }

        public override float Left
        {
            get { return Position.X - Radius; }
            set { Position.X = value + Radius; }
        }

        public override float Top
        {
            get { return Position.Y - Radius; }
            set { Position.Y = value + Radius; }
        }

        public override float Right
        {
            get { return Position.X + Radius; }
            set { Position.X = value - Radius; }
        }

        public override float Bottom
        {
            get { return Position.Y + Radius; }
            set { Position.Y = value - Radius; }
        }

        public override Collider Clone()
        {
            return new CircleCollider(Radius, Position.X, Position.Y);
        }

        public override void Render(Camera camera, Color color)
        {
            Draw.Circle(AbsolutePosition, Radius, color, 4);
        }

        /*
         *  Checking against other colliders
         */

        public override bool Collide(Vector2 point)
        {
            return Physics.Collide.CircleToPoint(AbsolutePosition, Radius, point);
        }

        public override bool Collide(Rectangle rect)
        {
            return Physics.Collide.RectToCircle(rect, AbsolutePosition, Radius);
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            return Physics.Collide.CircleToLine(AbsolutePosition, Radius, from, to);
        }

        public override bool Collide(CircleCollider circle)
        {
            return Vector2.DistanceSquared(AbsolutePosition, circle.AbsolutePosition) <
                   (Radius + circle.Radius) * (Radius + circle.Radius);
        }

        public override bool Collide(BoxCollider hitbox)
        {
            return hitbox.Collide(this);
        }

        public override bool Collide(GridCollider grid)
        {
            return grid.Collide(this);
        }

        public override bool Collide(ColliderList list)
        {
            return list.Collide(this);
        }
    }
}
