using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using Crimson.Audio;
using Crimson.Input;
using Crimson.Physics;
using Crimson.Tweening;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Crimson
{
    public class Engine : Game
    {
        private const int TARGET_FRAMERATE = 60;
        
        private static int s_viewPadding;
        private static bool s_resizing;

        // content directory
#if !CONSOLE
        private static readonly string
            AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

        //util
        public static Color ClearColor;
        public static bool ExitOnEscapeKeypress;
        private Scene? _nextScene;

        private RenderTarget2D? _renderTarget2D;

        // scene
        private Scene? _scene;
        public string Title;
        public Version Version = new Version(0, 0, 0);

        public Engine(int width, int height, int windowWidth, int windowHeight, string windowTitle, bool fullscreen)
        {
            Instance = this;
            
            Subsystems = new SubsystemList();

            Title = Window.Title = windowTitle;
            Width = width;
            Height = height;
            ClearColor = Color.Black;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.DeviceReset += OnGraphicsReset;
            Graphics.DeviceCreated += OnGraphicsCreate;
            
            Graphics.SynchronizeWithVerticalRetrace = true;
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Graphics.HardwareModeSwitch = false;

            Graphics.ApplyChanges();

#if PS4 || XBOXONE
            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
#elif NSWITCH
            Graphics.PreferredBackBufferWidth = 1280;
            Graphics.PreferredBackBufferHeight = 720;
#else
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnClientSizeChanged;

            if (fullscreen)
            {
                Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Graphics.IsFullScreen = true;
            }
            else
            {
                Graphics.PreferredBackBufferWidth = windowWidth;
                Graphics.PreferredBackBufferHeight = windowHeight;
                Graphics.IsFullScreen = false;
            }
#endif
            Graphics.ApplyChanges();

            GraphicsDevice.PresentationParameters.MultiSampleCount = 4;

            Content.RootDirectory = @"Content";

            IsMouseVisible = false;
            ExitOnEscapeKeypress = true;

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;

            Time.onFPSUpdate += OnFPSUpdate;

            // Add base subsystems
            Subsystems.Register<AudioSubsystem>();
            Subsystems.Register<PhysicsSubsystem>();
            Subsystems.Register<TweenSubsystem>();
        }

        public RenderTarget2D? RenderTarget
        {
            get => _renderTarget2D;
            set
            {
                _renderTarget2D = value;
                GraphicsDevice.SetRenderTarget(value);
            }
        }

        // references
        public static Engine Instance { get; private set; }
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static Commands Commands { get; private set; }
        public static Pooler Pooler { get; private set; }
        public static SubsystemList Subsystems { get; private set; }

        // screen size
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int ViewWidth { get; private set; }
        public static int ViewHeight { get; private set; }

        public static int ViewPadding
        {
            get => s_viewPadding;
            set
            {
                s_viewPadding = value;
                Instance.UpdateView();
            }
        }

        public static string ContentDirectory => Path.Combine(AssemblyDirectory, Instance.Content.RootDirectory);

#if !CONSOLE
        protected virtual void OnClientSizeChanged(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !s_resizing)
            {
                s_resizing = true;

                Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                UpdateView();

                s_resizing = false;
            }
        }
#endif

        protected virtual void OnGraphicsReset(object sender, EventArgs e)
        {
            UpdateView();

            _scene?.HandleGraphicsReset();

            if (_nextScene != _scene) _nextScene?.HandleGraphicsReset();
        }

        protected virtual void OnGraphicsCreate(object sender, EventArgs e)
        {
            UpdateView();

            _scene?.HandleGraphicsCreate();

            if (_nextScene != _scene) _nextScene?.HandleGraphicsCreate();
        }

        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);

            _scene?.GainFocus();
        }

        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);

            _scene?.LoseFocus();
        }

        protected override void Initialize()
        {
            base.Initialize();

            CInput.Initialize();
            Tracker.Initialize();
            Pooler = new Pooler();
            Commands = new Commands();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Crimson.Draw.Initialize(GraphicsDevice);
            // SDFTextRenderer.Initialize();
            Subsystems.Startup();
        }

        protected override void Update(GameTime gameTime)
        {
            Time.Update(gameTime);

            Subsystems.Tick();
            
            CInput.Update();

#if !CONSOLE
            if (ExitOnEscapeKeypress && CInput.Keyboard.Pressed(Keys.Escape))
            {
                Exit();
                return;
            }
#endif

            // We need to know if the scene's update methods should be called
            bool isSceneFrozen = false;
            if (Time.FreezeTimer > 0)
            {
                Time.FreezeTimer = Mathf.Max(Time.FreezeTimer - Time.RawDeltaTime, 0);
                isSceneFrozen = true;
            }
            
            // Call the 3 update methods on subsystems, then the current scene.
            Subsystems.BeforeUpdate();
            if (!isSceneFrozen)
            {
                _scene?.BeforeUpdate();
            }
            Subsystems.Update();
            if (!isSceneFrozen)
            {
                _scene?.Update();
            }
            Subsystems.AfterUpdate();
            if (!isSceneFrozen)
            {
                _scene?.AfterUpdate();
            }

            // Debug console
            if (Commands.Open && Commands.Enabled)
                Commands.UpdateOpen();
            else if (Commands.Enabled) Commands.UpdateClosed();

            // Changing scenes
            if (_scene != _nextScene)
            {
                Scene? lastScene = _scene;
                _scene?.End();

                _scene = _nextScene;
                OnSceneTransition(lastScene, _nextScene);
                Subsystems.OnSceneTransition(lastScene, _nextScene);
                _scene?.Begin();
            }

            base.Update(gameTime);
        }

        private void OnFPSUpdate(int newFps)
        {
#if DEBUG
            Window.Title = Title + " " + newFps + " fps - " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") +
                           " MB";
#endif
        }

        protected override void Draw(GameTime gameTime)
        {
            RenderCore();

            base.Draw(gameTime);

            if (Commands.Open) Commands.Render();

            Time.OnDraw(gameTime);
        }

        protected virtual void RenderCore()
        {
            RenderTarget = null;

            Subsystems.BeforeRender();
            _scene?.BeforeRender();
            
            GraphicsDevice.Viewport = Viewport;
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, ClearColor, 1,
                0);

            // Render the current scene
            _scene?.Render();
            
            Subsystems.AfterRender();
            _scene?.AfterRender();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            CInput.Shutdown();
            Subsystems.Shutdown();
        }

        #region Scene Management

        /// <summary>
        /// Called after a scene ends, before the next scene begins
        /// </summary>
        protected virtual void OnSceneTransition(Scene? from, Scene? to)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Time.TimeRate = 1f;
        }

        /// <summary>
        ///     The currently active scene. Note that if set, the scene will not actually change until the end of the update.
        /// </summary>
        public static Scene? Scene
        {
            get => Instance._scene;
            set => Instance._nextScene = value;
        }

        #endregion

        #region Screen

        public static Viewport Viewport { get; private set; }
        public static Matrix ScreenMatrix;

        public static void SetWindowed(int width, int height)
        {
#if !CONSOLE
            if (width > 0 && height > 0)
            {
                s_resizing = true;
                Graphics.PreferredBackBufferWidth = width;
                Graphics.PreferredBackBufferHeight = height;
                Graphics.IsFullScreen = false;
                Graphics.ApplyChanges();
                Utils.Log("WINDOW-" + width + "x" + height);
                s_resizing = false;
            }
#endif
        }

        public static void SetFullscreen()
        {
#if !CONSOLE
            s_resizing = true;
            Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Graphics.IsFullScreen = true;
            Graphics.ApplyChanges();
            Utils.Log("FULLSCREEN");
            s_resizing = false;
#endif
        }

        private void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // get view size
            if (screenWidth / Width > screenHeight / Height)
            {
                ViewWidth = (int) (screenHeight / Height * Width);
                ViewHeight = (int) screenHeight;
            }
            else
            {
                ViewWidth = (int) screenWidth;
                ViewHeight = (int) (screenWidth / Width * Height);
            }

            // apply view padding
            var aspect = ViewHeight / (float) ViewWidth;
            ViewWidth -= ViewPadding * 2;
            ViewHeight -= (int) (aspect * ViewPadding * 2);

            // update screen matrix
            ScreenMatrix = Matrix.CreateScale(ViewWidth / (float) Width);

            // update viewport
            Viewport = new Viewport
            {
                X = (int) (screenWidth / 2 - (float)ViewWidth / 2),
                Y = (int) (screenHeight / 2 - (float)ViewHeight / 2),
                Width = ViewWidth,
                Height = ViewHeight,
                MinDepth = 0,
                MaxDepth = 1
            };
            
            // inform all subsystems that the view has changed
            Subsystems.ViewUpdated();
        }

        #endregion
    }
}