using System;
using System.Collections.Generic;

namespace Crimson
{
    public class Spritesheet<T> : Image
    {
        private readonly Dictionary<T, Animation> _animations;
        private float _animationTimer;
        private Animation _currentAnimation;
        private bool _played;
        public int CurrentFrame;
        public Action<T> OnAnimate;
        public Action<T> OnFinish;
        public Action<T> OnLoop;
        public float Rate = 1;
        public bool UseRawDeltaTime;

        public Spritesheet(CTexture texture, int frameWidth, int frameHeight, int frameSep = 0)
            : base(texture, true)
        {
            SetFrames(texture, frameWidth, frameHeight, frameSep);
            _animations = new Dictionary<T, Animation>();
        }

        public void SetFrames(CTexture texture, int frameWidth, int frameHeight, int frameSep = 0)
        {
            var frames = new List<CTexture>();
            int x = 0, y = 0;

            while (y <= texture.Height - frameHeight)
            {
                while (x <= texture.Width - frameWidth)
                {
                    frames.Add(texture.GetSubtexture(x, y, frameWidth, frameHeight));
                    x += frameWidth + frameSep;
                }

                y += frameHeight + frameSep;
                x = 0;
            }

            Frames = frames.ToArray();
        }

        public override void Update()
        {
            if (Animating && _currentAnimation.Delay > 0)
            {
                //Timer
                if (UseRawDeltaTime)
                    _animationTimer += Time.RawDeltaTime * Rate;
                else
                    _animationTimer += Time.DeltaTime * Rate;

                //Next Frame
                if (Math.Abs(_animationTimer) >= _currentAnimation.Delay)
                {
                    CurrentAnimationFrame += Math.Sign(_animationTimer);
                    _animationTimer -= Math.Sign(_animationTimer) * _currentAnimation.Delay;

                    //End of Animation
                    if (CurrentAnimationFrame < 0 || CurrentAnimationFrame >= _currentAnimation.Frames.Length)
                    {
                        //Looped
                        if (_currentAnimation.Loop)
                        {
                            CurrentAnimationFrame -= Math.Sign(CurrentAnimationFrame) * _currentAnimation.Frames.Length;
                            CurrentFrame = _currentAnimation.Frames[CurrentAnimationFrame];

                            OnAnimate?.Invoke(CurrentAnimationID);

                            OnLoop?.Invoke(CurrentAnimationID);
                        }
                        else
                        {
                            //Ended
                            if (CurrentAnimationFrame < 0)
                                CurrentAnimationFrame = 0;
                            else
                                CurrentAnimationFrame = _currentAnimation.Frames.Length - 1;

                            Animating = false;
                            _animationTimer = 0;
                            OnFinish?.Invoke(CurrentAnimationID);
                        }
                    }
                    else
                    {
                        //Continue Animation
                        CurrentFrame = _currentAnimation.Frames[CurrentAnimationFrame];
                        OnAnimate?.Invoke(CurrentAnimationID);
                    }
                }
            }
        }

        public override void Render()
        {
            Texture = Frames[CurrentFrame];
            base.Render();
        }

        private struct Animation
        {
            public float Delay;
            public int[] Frames;
            public bool Loop;
        }

        #region Animation Definition

        public void Add(T id, bool loop, float delay, params int[] frames)
        {
#if DEBUG
            foreach (var i in frames)
                if (i >= Frames.Length)
                    throw new IndexOutOfRangeException("Specified frames is out of max range for this Spritesheet");
#endif

            _animations[id] = new Animation {Delay = delay, Frames = frames, Loop = loop};
        }

        public void Add(T id, float delay, params int[] frames)
        {
            Add(id, true, delay, frames);
        }

        public void Add(T id, int frame)
        {
            Add(id, false, 0, frame);
        }

        public void ClearAnimations()
        {
            _animations.Clear();
        }

        #endregion

        #region Animation Playback

        public bool IsPlaying(T id)
        {
            if (!_played) return false;

            if (CurrentAnimationID == null) return id == null;

            return CurrentAnimationID.Equals(id);
        }

        public void Play(T id, bool restart = false)
        {
            if (!IsPlaying(id) || restart)
            {
#if DEBUG
                if (!_animations.ContainsKey(id)) throw new Exception("No Animation defined for ID: " + id);
#endif
                CurrentAnimationID = id;
                _currentAnimation = _animations[id];
                _animationTimer = 0;
                CurrentAnimationFrame = 0;
                _played = true;

                Animating = _currentAnimation.Frames.Length > 1;
                CurrentFrame = _currentAnimation.Frames[0];
            }
        }

        public void Reverse(T id, bool restart = false)
        {
            Play(id, restart);
            if (Rate > 0) Rate *= -1;
        }

        public void Stop()
        {
            Animating = false;
            _played = false;
        }

        #endregion

        #region Properties

        public CTexture[] Frames { get; private set; }

        public bool Animating { get; private set; }

        public T CurrentAnimationID { get; private set; }

        public int CurrentAnimationFrame { get; private set; }

        public override float Width
        {
            get
            {
                if (Frames.Length > 0) return Frames[0].Width;

                return 0;
            }
        }

        public override float Height
        {
            get
            {
                if (Frames.Length > 0) return Frames[0].Height;

                return 0;
            }
        }

        #endregion
    }
}