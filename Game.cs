using System.Diagnostics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SiphoEngine.Core;
using SiphoEngine.Core.Audio;
using SiphoEngine.Core.Components.Render;
using SiphoEngine.Core.Debugging;
using SiphoEngine.Core.Physics;
using SiphoEngine.Core.ResourceSystem;
using Debug = SiphoEngine.Core.Debugging.Debug;
using Time = SiphoEngine.Core.Time;

namespace SiphoEngine
{
    public sealed class Game
    {
        public event Action? OnRunning;
        public event Action? OnLoadAssets;
        private View _gameView;
        private RenderWindow? _window;
        private bool _fullscreen;
        private bool _isActive;
        private string _contextRootPath;

        public string ContentRootPath
        {
            get
            {
                return _contextRootPath;
            }

            set
            {
                if (!_isActive)
                {
                    _contextRootPath = value;
                }
            }
        }

        public ResourceManager ResourceManager { get; private set; }

        public void Run(uint width = 800, uint height = 600, string title = "SiphoEngine Game", bool fullscreen = false)
        {
#if DEBUG
            Debug.Initialize();
            Debug.EnableGlobalExceptionHandling();
#endif

            // Инициализация окна
            _fullscreen = fullscreen;
            _window = new RenderWindow(
                fullscreen ? VideoMode.DesktopMode : new VideoMode(width, height),
                title,
                fullscreen ? Styles.Fullscreen : Styles.Default
            );

            _gameView = new View(new FloatRect(0, 0, width, height));
            _window.SetView(_gameView);
            _window.SetVerticalSyncEnabled(true);
            _window.SetFramerateLimit(61);

            InitializeResources();

            GameEngine.InitializePrefabs();
            GameEngine.InitializeScenes();
            GameEngine.InitializeWindow(_window);
            Input.Initialize(_window);

            _window.Closed += (sender, e) => _window.Close();
            _window.Resized += OnWindowResized;

            PreciseTimer.Initialize();
            var frameTimer = Stopwatch.StartNew();
            long lastFrameTicks = frameTimer.ElapsedTicks;
            float frameTimeAccumulator = 0f;
            int frameCount = 0;
            float fpsTimer = 0f;
            _isActive = true;
            OnRunning?.Invoke();
            while (_window.IsOpen)
            {
                _window.DispatchEvents();

                long currentTicks = frameTimer.ElapsedTicks;
                float rawDeltaTime = (currentTicks - lastFrameTicks) / (float)Stopwatch.Frequency;
                lastFrameTicks = currentTicks;

                Time.Update();

                frameTimeAccumulator += Time.DeltaTime;
                while (frameTimeAccumulator >= Time.FixedDeltaTime)
                {
                    GameEngine.Update(Time.FixedDeltaTime);
                    PhysicsEngine.Update(Time.FixedDeltaTime);
                    frameTimeAccumulator -= Time.FixedDeltaTime;
                }

                AudioEngine.Update();

                _window.Clear(Camera.Main?.BackgroundColor ?? Color.Black);

                GameEngine.BeforeRender();
                GameEngine.ActiveScene?.Draw(_window);

#if DEBUG
                DebugDraw.Render(_window);
#endif

                Input.Update();

                _window.Display();

                float elapsedThisFrame = (frameTimer.ElapsedTicks - currentTicks) / (float)Stopwatch.Frequency;
                float targetFrameTime = 1f / 60f;
                float remainingTime = targetFrameTime - elapsedThisFrame;

                if (remainingTime > 0.001f)
                {
                    int sleepMs = (int)(remainingTime * 1000) - 1;
                    if (sleepMs > 0)
                        Thread.Sleep(sleepMs);

                    while ((frameTimer.ElapsedTicks - currentTicks) / (float)Stopwatch.Frequency < targetFrameTime) { }
                }

                // 9. FPS счетчик
                frameCount++;
                fpsTimer += Time.DeltaTime;
                if (fpsTimer >= 1.0f)
                {
                    frameCount = 0;
                    fpsTimer = 0f;
                }
            }

            _isActive = false;
#if DEBUG
            Debug.Shutdown();
#endif
        }

        private void InitializeResources()
        {
            if (string.IsNullOrEmpty(_contextRootPath))
            {
                ResourceManager = new ResourceManager();
            }

            else
            {
                ResourceManager = new ResourceManager(_contextRootPath);
            }

            OnLoadAssets?.Invoke();


        }

        private void OnWindowResized(object sender, SizeEventArgs e)
        {
            if (_fullscreen)
            {
                UpdateViewForFullscreen();
            }
            else
            {
                _gameView.Size = new Vector2f(e.Width, e.Height);
                _window.SetView(_gameView);
            }

            foreach (var camera in GameEngine.GetAllCameras())
            {
                camera.OnWindowResized(e.Width, e.Height);
            }
        }

        private void UpdateViewForFullscreen()
        {
            if (_window == null) return;

            float gameAspect = _gameView.Size.X / _gameView.Size.Y;
            float windowAspect = (float)_window.Size.X / _window.Size.Y;

            if (windowAspect > gameAspect)
            {
                float viewHeight = _gameView.Size.Y;
                float viewWidth = viewHeight * windowAspect;
                _gameView.Viewport = new FloatRect(
                    (1 - gameAspect / windowAspect) / 2, 0,
                    gameAspect / windowAspect, 1
                );
            }
            else
            {
                float viewWidth = _gameView.Size.X;
                float viewHeight = viewWidth / windowAspect;
                _gameView.Viewport = new FloatRect(
                    0, (1 - windowAspect / gameAspect) / 2,
                    1, windowAspect / gameAspect
                );
            }
        }
    }
}