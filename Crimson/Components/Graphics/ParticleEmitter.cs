using Microsoft.Xna.Framework;

namespace Crimson
{
    public class ParticleEmitter : Component
    {
        public int Amount;
        public float? Direction;
        public float Interval;
        public Vector2 Position;
        public Vector2 Range;

        public ParticleSystem System;

        private float timer;

        public Entity Track;
        public ParticleType Type;

        public ParticleEmitter(
            ParticleSystem system,
            ParticleType type,
            Vector2 position,
            Vector2 range,
            int amount,
            float interval
        ) : base(true, false)
        {
            System = system;
            Type = type;
            Position = position;
            Range = range;
            Amount = amount;
            Interval = interval;
        }

        public ParticleEmitter(
            ParticleSystem system,
            ParticleType type,
            Vector2 position,
            Vector2 range,
            float direction,
            int amount,
            float interval
        )
            : this(system, type, position, range, amount, interval)
        {
            Direction = direction;
        }

        public ParticleEmitter(
            ParticleSystem system,
            ParticleType type,
            Entity track,
            Vector2 position,
            Vector2 range,
            float direction,
            int amount,
            float interval
        )
            : this(system, type, position, range, amount, interval)
        {
            Direction = direction;
            Track = track;
        }

        public void SimulateCycle()
        {
            Simulate(Type.LifeMax);
        }

        public void Simulate(float duration)
        {
            var steps = duration / Interval;
            for (var i = 0; i < steps; i++)
            for (var j = 0; j < Amount; j++)
            {
                // create the particle
                var particle = new Particle();
                var pos = Entity.Position + Position + Utils.Random.Range(-Range, Range);
                particle = Direction.HasValue
                    ? Type.Create(ref particle, pos, Direction.Value)
                    : Type.Create(ref particle, pos);
                particle.Track = Track;

                // simulate for a duration
                var simulateFor = duration - Interval * i;
                if (particle.SimulateFor(simulateFor))
                    System.Add(particle);
            }
        }

        public void Emit()
        {
            if (Direction.HasValue)
                System.Emit(Type, Amount, Entity.Position + Position, Range, Direction.Value);
            else
                System.Emit(Type, Amount, Entity.Position + Position, Range);
        }

        public override void Update()
        {
            timer -= Time.DeltaTime;
            if (timer <= 0)
            {
                timer = Interval;
                Emit();
            }
        }
    }
}