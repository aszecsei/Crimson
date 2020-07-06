using Microsoft.Xna.Framework;

namespace Crimson.UI
{
    public class UISubsystem : Subsystem
    {
        public static UISubsystem Instance;
        
        public Widget? Root;

        private int _screenWidth = 0;
        private int _screenHeight = 0;
        private float _screenScale = 1f;
        private Matrix _cameraMatrix;
        private bool _dirty = true;

        public bool Debug = false;
        public float Alpha = 1f;

        public float ScreenScale
        {
            get => _screenScale;
            set
            {
                _screenScale = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Resets the entire UI subsystem, removing all UI elements from the screen and reverting all other
        /// values to their defaults.
        /// </summary>
        public void Reset()
        {
            Root = null;
            _screenWidth = 0;
            _screenHeight = 0;
            _screenScale = 1f;
        }

        protected override void Startup()
        {
            base.Startup();

            Instance = this;
        }

        private void CheckLayout()
        {
            // Check the layout
            if (Engine.ViewWidth != _screenWidth || Engine.ViewHeight != _screenHeight)
            {
                _dirty = true;
            }

            if (_dirty)
            {
                _screenWidth = Engine.ViewWidth;
                _screenHeight = Engine.ViewHeight;
                _cameraMatrix = Matrix.CreateScale(_screenScale);

                if (Root != null)
                {
                    Root.Geometry = new Rect(0, 0, _screenWidth / _screenScale, _screenHeight / _screenScale);
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            
            CheckLayout();

            Root?.Update();
        }

        protected override void AfterRender()
        {
            base.AfterRender();
            
            CheckLayout();

            Draw.SpriteBatch.Begin(transformMatrix: _cameraMatrix);
            
            Root?.Render(Alpha);
            
            if (Debug)
                Root?.DebugRender();
            
            Draw.SpriteBatch.End();
        }
    }
}