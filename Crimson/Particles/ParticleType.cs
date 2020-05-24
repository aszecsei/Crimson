#region Using Statements

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion

namespace Crimson
{
    public class ParticleType
    {
        public enum ColorModes
        {
            Static,
            Choose,
            Blink,
            Fade
        }

        public enum FadeModes
        {
            None,
            Linear,
            Late,
            InAndOut
        }

        public enum RotationModes
        {
            None,
            Random,
            SameAsDirection
        }

        private static readonly List<ParticleType> AllTypes = new List<ParticleType>();
        public Vector2 Acceleration;
        public Color Color;
        public Color Color2;
        public ColorModes ColorMode;
        public float Direction;
        public float DirectionRange;
        public FadeModes FadeMode;
        public float Friction;
        public float LifeMax;
        public float LifeMin;
        public RotationModes RotationMode;
        public bool ScaleOut;
        public float Size;
        public float SizeRange;

        public CTexture Source;
        public Chooser<CTexture> SourceChooser;
        public float SpeedMax;
        public float SpeedMin;
        public float SpeedMultiplier;
        public bool SpinFlippedChance;
        public float SpinMax;
        public float SpinMin;
        public bool UseActualDeltaTime;

        public ParticleType()
        {
            Color = Color2 = Color.White;
            ColorMode = ColorModes.Static;
            FadeMode = FadeModes.None;
            SpeedMin = SpeedMax = 0;
            SpeedMultiplier = 1;
            Acceleration = Vector2.Zero;
            Friction = 0f;
            Direction = DirectionRange = 0;
            LifeMin = LifeMax = 0;
            Size = 2;
            SizeRange = 0;
            SpinMin = SpinMax = 0;
            SpinFlippedChance = false;
            RotationMode = RotationModes.None;

            AllTypes.Add(this);
        }

        public ParticleType(ParticleType copyFrom)
        {
            Source = copyFrom.Source;
            SourceChooser = copyFrom.SourceChooser;
            Color = copyFrom.Color;
            Color2 = copyFrom.Color2;
            ColorMode = copyFrom.ColorMode;
            FadeMode = copyFrom.FadeMode;
            SpeedMin = copyFrom.SpeedMin;
            SpeedMax = copyFrom.SpeedMax;
            SpeedMultiplier = copyFrom.SpeedMultiplier;
            Acceleration = copyFrom.Acceleration;
            Friction = copyFrom.Friction;
            Direction = copyFrom.Direction;
            DirectionRange = copyFrom.DirectionRange;
            LifeMin = copyFrom.LifeMin;
            LifeMax = copyFrom.LifeMax;
            Size = copyFrom.Size;
            SizeRange = copyFrom.SizeRange;
            RotationMode = copyFrom.RotationMode;
            SpinMin = copyFrom.SpinMin;
            SpinMax = copyFrom.SpinMax;
            SpinFlippedChance = copyFrom.SpinFlippedChance;
            ScaleOut = copyFrom.ScaleOut;
            UseActualDeltaTime = copyFrom.UseActualDeltaTime;

            AllTypes.Add(this);
        }

        public Particle Create(ref Particle particle, Vector2 position)
        {
            return Create(ref particle, position, Direction);
        }

        public Particle Create(ref Particle particle, Vector2 position, Color color)
        {
            return Create(ref particle, null, position, Direction, color);
        }

        public Particle Create(ref Particle particle, Vector2 position, float direction)
        {
            return Create(ref particle, null, position, direction, Color);
        }

        public Particle Create(ref Particle particle, Vector2 position, Color color, float direction)
        {
            return Create(ref particle, null, position, direction, color);
        }

        public Particle Create(ref Particle particle, Entity entity, Vector2 position, float direction, Color color)
        {
            particle.Track = entity;
            particle.Type = this;
            particle.Active = true;
            particle.Position = position;

            // source texture
            if (SourceChooser != null)
                particle.Source = SourceChooser.Choose();
            else if (Source != null)
                particle.Source = Source;
            else
                particle.Source = Draw.Particle;

            // size
            if (SizeRange != 0)
                particle.StartSize = particle.Size = Size - SizeRange * .5f + Utils.Random.NextFloat(SizeRange);
            else
                particle.StartSize = particle.Size = Size;

            // color
            if (ColorMode == ColorModes.Choose)
                particle.StartColor = particle.Color = Utils.Random.Choose(color, Color2);
            else
                particle.StartColor = particle.Color = color;

            // speed / direction
            var moveDirection = direction - DirectionRange / 2 + Utils.Random.NextFloat() * DirectionRange;
            particle.Speed = Mathf.AngleToVector(moveDirection, Utils.Random.Range(SpeedMin, SpeedMax));

            // life
            particle.StartLife = particle.Life = Utils.Random.Range(LifeMin, LifeMax);

            // rotation
            if (RotationMode == RotationModes.Random)
                particle.Rotation = Utils.Random.NextAngle();
            else if (RotationMode == RotationModes.SameAsDirection)
                particle.Rotation = moveDirection;
            else
                particle.Rotation = 0;

            // spin
            particle.Spin = Utils.Random.Range(SpinMin, SpinMax);
            if (SpinFlippedChance)
                particle.Spin *= Utils.Random.Choose(1, -1);

            return particle;
        }
    }
}