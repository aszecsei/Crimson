using System;
using System.Collections.Generic;
using System.Text;

namespace Crimson.Physics
{
    public static class PhysicsEntityListExtensions
    {
        public static List<PhysicsEntity> PhysicsEntities(this List<Entity> entities)
        {
            List<PhysicsEntity> res = new List<PhysicsEntity>();
            foreach (Entity e in entities)
            {
                if (e is PhysicsEntity pe)
                {
                    res.Add(pe);
                }
            }
            return res;
        }
    }
}
