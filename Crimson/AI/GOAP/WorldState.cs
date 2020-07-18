using System.Text;

namespace Crimson.AI.GOAP
{
    public class WorldState
    {
        public long Values;

        public long DontCare;

        internal ActionPlanner Planner;

        public static WorldState Create(ActionPlanner planner)
        {
            return new WorldState(planner, 0, -1);
        }

        public WorldState(ActionPlanner planner, long values, long dontCare)
        {
            Planner = planner;
            Values = values;
            DontCare = dontCare;
        }

        public bool Set(string conditionName, bool value)
        {
            return Set(Planner.FindConditionNameIndex(conditionName), value);
        }

        internal bool Set(int conditionId, bool value)
        {
            Values = value ? (Values | (1L << conditionId)) : (Values & ~(1L << conditionId));
            DontCare ^= (1 << conditionId);
            return true;
        }

        public bool Equals(WorldState other)
        {
            var care = DontCare ^ -1L;
            return (Values & care) == (other.Values & care);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < ActionPlanner.MAX_CONDITIONS; ++i)
            {
                if ((DontCare & (1L << i)) == 0)
                {
                    var val = Planner.ConditionNames[i];
                    if (val == null)
                        continue;

                    bool set = (Values & (1L << i)) != 0L;
                    if (sb.Length > 0)
                        sb.Append(", ");
                    sb.Append(set ? val.ToUpper() : val);
                }
            }

            return sb.ToString();
        }
    }
}