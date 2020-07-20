using System.Collections.Generic;
using Crimson.AI;

namespace Crimson
{
    // TODO: Make this generic as an "AI controller" that just runs some sort of algorithm
    public class BehaviorTree : Component
    {
        private Crimson.AI.BehaviorTree.BehaviorTree _behavior;
        private List<ISensor> _sensors = new List<ISensor>();
        
        public BehaviorTree(Crimson.AI.BehaviorTree.BehaviorTree bt) : base(true, false)
        {
            _behavior = bt;
        }

        public BehaviorTree AddSensor(ISensor sensor)
        {
            _sensors.Add(sensor);
            return this;
        }

        public override void Update()
        {
            base.Update();
            // TODO: Prioritize sensor checks
            // TODO: Limit sensor checks based on time
            for (var i = 0; i < _sensors.Count; ++i)
                _sensors[i].Sense(Entity, _behavior.Context);
            _behavior.Tick();
        }
    }
}