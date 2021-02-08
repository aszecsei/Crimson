using Microsoft.Xna.Framework;

namespace Crimson.Physics
{
    public delegate void Collision(CollisionData data);

    public ref struct CollisionData
    {
        public Platform Pusher;
        public Platform Hit;
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
                    Platform first = CollideFirst<Solid>(Position + new Vector2(sign, 0));
                    if ( first != null )
                    {
                        // Hit a solid!
                        onCollide?.Invoke(new CollisionData
                        {
                            Hit            = first,
                            Direction      = new Vector2(sign, 0),
                            TargetPosition = Position + new Vector2(sign, 0)
                        });
                        break;
                    }

                    first = CollideFirst<JumpThru>(Position + new Vector2(sign, 0));
                    if ( first != null )
                    {
                        // Check the jumpthru's orientation to see if we can ignore it
                        JumpThru jt = (JumpThru)first;
                        if ( (jt.Rotation == 1 && sign < 0) || (jt.Rotation == 3 && sign > 0) )
                        {
                            onCollide?.Invoke(new CollisionData
                            {
                                Hit            = first,
                                Direction      = new Vector2(sign, 0),
                                TargetPosition = Position + new Vector2(sign, 0)
                            });
                            break;
                        }
                    }

                    // We're good
                    Position.X += sign;
                    move       -= sign;
                }
            }
        }

        public void MoveXExact(int move, Collision onCollide = null)
        {
            int sign = Mathf.Sign(move);

            while ( move != 0 )
            {
                Platform first = CollideFirst<Solid>(Position + new Vector2(sign, 0));
                if ( first != null )
                {
                    // Hit a solid!
                    onCollide?.Invoke(new CollisionData
                    {
                        Hit            = first,
                        Direction      = new Vector2(sign, 0),
                        TargetPosition = Position + new Vector2(sign, 0)
                    });
                    break;
                }

                first = CollideFirst<JumpThru>(Position + new Vector2(sign, 0));
                if ( first != null )
                {
                    // Check the jumpthru's orientation to see if we can ignore it
                    JumpThru jt = (JumpThru)first;
                    if ( (jt.Rotation == 1 && sign < 0) || (jt.Rotation == 3 && sign > 0) )
                    {
                        onCollide?.Invoke(new CollisionData
                        {
                            Hit            = first,
                            Direction      = new Vector2(sign, 0),
                            TargetPosition = Position + new Vector2(sign, 0)
                        });
                        break;
                    }
                }

                // We're good
                Position.X += sign;
                move       -= sign;
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
                    Platform first = CollideFirst<Solid>(Position + new Vector2(0, sign));
                    if ( first != null )
                    {
                        // Hit a solid!
                        onCollide?.Invoke(new CollisionData
                        {
                            Hit            = first,
                            Direction      = new Vector2(0, sign),
                            TargetPosition = Position + new Vector2(0, sign)
                        });
                        break;
                    }

                    first = CollideFirst<JumpThru>(Position + new Vector2(0, sign));
                    if ( first != null )
                    {
                        // Check the jumpthru's orientation to see if we can ignore it
                        JumpThru jt = (JumpThru)first;
                        if ( (jt.Rotation == 0 && sign > 0) || (jt.Rotation == 2 && sign < 0) )
                        {
                            onCollide?.Invoke(new CollisionData
                            {
                                Hit            = first,
                                Direction      = new Vector2(sign, 0),
                                TargetPosition = Position + new Vector2(sign, 0)
                            });
                            break;
                        }
                    }

                    // We're good
                    Position.Y += sign;
                    move       -= sign;
                }
            }
        }

        public void MoveYExact(int move, Collision onCollide = null)
        {
            int sign = Mathf.Sign(move);

            while ( move != 0 )
            {
                Platform first = CollideFirst<Solid>(Position + new Vector2(0, sign));
                if ( first != null )
                {
                    // Hit a solid!
                    onCollide?.Invoke(new CollisionData
                    {
                        Hit            = first,
                        Direction      = new Vector2(0, sign),
                        TargetPosition = Position + new Vector2(0, sign)
                    });
                    break;
                }

                first = CollideFirst<JumpThru>(Position + new Vector2(0, sign));
                if ( first != null )
                {
                    // Check the jumpthru's orientation to see if we can ignore it
                    JumpThru jt = (JumpThru)first;
                    if ( (jt.Rotation == 0 && sign > 0) || (jt.Rotation == 2 && sign < 0) )
                    {
                        onCollide?.Invoke(new CollisionData
                        {
                            Hit            = first,
                            Direction      = new Vector2(sign, 0),
                            TargetPosition = Position + new Vector2(sign, 0)
                        });
                        break;
                    }
                }

                // We're good
                Position.Y += sign;
                move       -= sign;
            }
        }

        protected void MoveTowardsX(float amount, float maxDistance)
        {
            float clampedAmount = Mathf.Clamp(amount - X, -maxDistance, maxDistance);
            xRemainder += clampedAmount;
            int move = Mathf.RoundToInt(xRemainder);

            int sign = Mathf.Sign(move);
            while ( move != 0 )
            {
                Position.X += sign;
                move -= sign;
            }
        }

        protected void MoveTowardsY(float amount, float maxDistance)
        {
            float clampedAmount = Mathf.Clamp(amount - Y, -maxDistance, maxDistance);
            yRemainder += clampedAmount;
            int move = Mathf.RoundToInt(yRemainder);

            int sign = Mathf.Sign(move);
            while ( move != 0 )
            {
                Position.Y += sign;
                move       -= sign;
            }
        }

        protected void ZeroRemainderX()
        {
            xRemainder = 0;
        }

        protected void ZeroRemainderY()
        {
            yRemainder = 0;
        }

        public virtual bool IsRiding(Solid solid)
        {
            if ( solid.CollideCheck(this, Position + Vector2.UnitY) )
            {
                return true;
            }

            return false;
        }

        public virtual bool IsRiding(JumpThru jumpThru)
        {
            if ( jumpThru.CollideCheck(this, Position + Vector2.UnitY) )
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
