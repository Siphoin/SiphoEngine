using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SiphoEngine.Core;
using SiphoEngine.Core.Components.Render;
using Time = SiphoEngine.Core.Time;

namespace SiphoEngine
{
    public class Game
    {
        private RenderWindow? _window;
        private Clock? _gameClock;
        private View _gameView;
        private bool _fullscreen;

        public event Action OnRunning;

        public void Run(uint width = 800, uint height = 600, string title = "SiphoEngine Game", bool fullscreen = false)
        {
            _fullscreen = fullscreen;
            _window = new RenderWindow(
                fullscreen ? VideoMode.DesktopMode : new VideoMode(width, height),
                title,
                fullscreen ? Styles.Fullscreen : Styles.Default
            );

            // Фиксированный вид игры (логические координаты)
            _gameView = new View(new FloatRect(0, 0, width, height));

            _window.SetVerticalSyncEnabled(true);
            GameEngine.InitializePrefabs();
            GameEngine.InitializeWindow(_window);

            _window.Closed += (sender, e) => _window.Close();
            _window.Resized += OnWindowResized;

            _gameClock = new Clock();
            Input.Initialize(_window);

            OnRunning?.Invoke();

            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                _window.Clear(Camera.Main is null ? Color.Black : Camera.Main.BackgroundColor);

                float deltaTime = _gameClock.Restart().AsSeconds();
                Time.Update(deltaTime);
                GameEngine.Update(Time.DeltaTime);
                AudioEngine.Update();

                if (_fullscreen)
                {
                    UpdateViewForFullscreen();
                    _window.SetView(_gameView);
                }

                GameEngine.BeforeRender();
                GameEngine.ActiveScene?.Draw(_window);

                _window.Display();
            }
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