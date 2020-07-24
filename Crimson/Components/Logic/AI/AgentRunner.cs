using System.Collections.Generic;
using Crimson.AI;

namespace Crimson
{
    // TODO: Make this generic as an "AI controller" that just runs some sort of algorithm
    public class AgentRunner : Component
    {
        private Agent _behavior;
        private List<ISensor> _sensors = new List<ISensor>();
        
        public AgentRunner(Agent agent) : base(true, false)
        {
            _behavior = agent;
        }

        public AgentRunner AddSensor(ISensor sensor)
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