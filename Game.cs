using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SiphoEngine.Core;
using Time = SiphoEngine.Core.Time;

namespace SiphoEngine
{
    public class Game
    {
        private RenderWindow? _window;
        private Clock? _gameClock;
        public event Action OnRunning;
        public void Run(uint width = 800, uint height = 600, string title = "SiphoEngine Game", bool fullscreen = false)
        {
            _window = new RenderWindow(
                fullscreen ? VideoMode.DesktopMode : new VideoMode(width, height),
                title,
                fullscreen ? Styles.Fullscreen : Styles.Default
            );

            GameEngine.InitializeWindow(_window);

            _window.Closed += (sender, e) => _window.Close();
            _window.Resized += OnWindowResized;

            _gameClock = new Clock();
            Input.Initialize(_window);

            OnRunning?.Invoke();

            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                _window.Clear(Color.Black);

                float deltaTime = _gameClock.Restart().AsSeconds();
                Time.Update(deltaTime);
                GameEngine.Update(Time.DeltaTime);

                // Установка вида камеры перед отрисовкой
                GameEngine.BeforeRender();
                GameEngine.ActiveScene?.Draw(_window);

                _window.Display();
            }
        }


        private void OnWindowResized(object sender, SizeEventArgs e)
        {
            foreach (var camera in GameEngine.GetAllCameras())
            {
                camera.OnWindowResized(e.Width, e.Height);
            }
        }
    }
}