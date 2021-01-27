using Microsoft.Xna.Framework;

namespace Crimson.Physics
{
    public delegate void Collision(CollisionData data);

    public ref struct CollisionData
    {
        public Solid Pusher;
        public Solid Hit;
        public Vector2 Direction;
        public Vector2 TargetPosition;
    }

    [Tracked]
    public class Actor : PhysicsEntity
    {
        private float xRemainder;
        private float yRemainder;

        public Actor() { }

        public Actor(Vector2 position)
            : base(position) { }

        public void MoveX(float amount, Collision onCollide = null)
        {
            xRemainder += amount;
            int move = Mathf.RoundToInt(xRemainder);

            if ( move != 0 )
            {
                xRemainder -= move;
                int sign = Mathf.Sign(move);

                while ( move != 0 )
                {
                    var first = CollideFirst<Solid>(Position + new Vector2(sign, 0));
                    if ( first == null )
                    {
                        // No solid immediately beside us
                        Position.X += sign;
                        move -= sign;
                    }
                    else
                    {
                        // Hit a solid!
                        onCollide?.Invoke(new CollisionData
                        {
                            Hit = first,
                            Direction = new Vector2(sign, 0),
                            TargetPosition = Position + new Vector2(sign, 0)
                        });
                        break;
                    }
                }
            }
        }

        public void MoveY(float amount, Collision onCollide = null)
        {
            yRemainder += amount;
            int move = Mathf.RoundToInt(yRemainder);

            if ( move != 0 )
            {
                yRemainder -= move;
                int sign = Mathf.Sign(move);

                while ( move != 0 )
                {
                    var first = CollideFirst<Solid>(Position + new Vector2(0, sign));
                    if ( first == null )
                    {
                        // No solid immediately beside us
                        Position.Y += sign;
                        move -= sign;
                    }
                    else
                    {
                        // Hit a solid!
                        onCollide?.Invoke(new CollisionData
                        {
                            Hit = first,
                            Direction = new Vector2(0, sign),
                            TargetPosition = Position + new Vector2(0, sign)
                        });
                        break;
                    }
                }
            }
        }

        public virtual bool IsRiding(Solid solid)
        {
            if ( solid.CollideCheck(this, Position + Vector2.UnitY) )
            {
                return true;
            }

            return false;
        }

        public bool TrySquishWiggle(CollisionData data) =>

            // TODO: Implement squish wiggle
            false;

        public virtual void Squish(CollisionData data) => Destroy();
    }
}
