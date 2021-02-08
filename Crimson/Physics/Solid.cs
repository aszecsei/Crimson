using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Crimson.Physics
{
    public class Solid : Platform
    {
        private float xRemainder;
        private float yRemainder;

        public Solid() { }

        public Solid(Vector2 position)
            : base(position) { }

        private List<Actor> GetAllRidingActors()
        {
            var actors = new List<Actor>();

            if ( Scene == null ) return actors;

            foreach ( Actor a in Scene.Tracker.GetEntities<Actor>() )
            {
                if ( a.IsRiding(this) )
                {
                    actors.Add(a);
                }
            }

            return actors;
        }

        public void Move(float x, float y)
        {
            xRemainder += x;
            yRemainder += y;
            int moveX = Mathf.RoundToInt(xRemainder);
            int moveY = Mathf.RoundToInt(yRemainder);

            if ( moveX != 0 || moveY != 0 )
            {
                // Loop through every actor in the level, add it to a
                // list if actor.IsRiding(this) is true
                List<Actor> riding = GetAllRidingActors();

                // Make this solid non-collidable for actors,
                // so that actors moved by it do not get stuck on it
                Collidable = false;

                if ( moveX != 0 )
                {
                    xRemainder -= moveX;
                    Position.X += moveX;

                    List<Entity> allActors = Scene.Tracker.GetEntities<Actor>();
                    foreach ( Actor actor in allActors )
                    {
                        if ( CollideCheck(actor) )
                        {
                            if ( moveX > 0 )
                            {
                                actor.MoveX(Right - actor.Left, data =>
                                {
                                    data.Pusher = this;
                                    actor.Squish(data);
                                });
                            }
                            else
                            {
                                actor.MoveX(Left - actor.Right, actor.Squish);
                            }
                        }
                        else if ( riding.Contains(actor) )
                        {
                            // Carry
                            actor.MoveX(moveX);
                        }
                    }
                }

                if ( moveY != 0 )
                {
                    yRemainder -= moveY;
                    Position.Y += moveY;

                    List<Entity> allActors = Scene.Tracker.GetEntities<Actor>();
                    foreach ( Actor actor in allActors )
                    {
                        if ( CollideCheck(actor) )
                        {
                            if ( moveY > 0 )
                            {
                                actor.MoveY(Bottom - actor.Top, actor.Squish);
                            }
                            else
                            {
                                actor.MoveY(Top - actor.Bottom, actor.Squish);
                            }
                        }
                        else if ( riding.Contains(actor) )
                        {
                            // Carry
                            actor.MoveY(moveY);
                        }
                    }
                }

                // Re-enable collisions
                Collidable = true;
            }
        }
    }
}
