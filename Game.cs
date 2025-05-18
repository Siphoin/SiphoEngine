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

        public void Run(uint width = 800, uint height = 600, string title = "SiphoEngine Game", bool fullscreen = false)
        {
            _window = new RenderWindow(
                fullscreen ? VideoMode.DesktopMode : new VideoMode(width, height),
                title,
                fullscreen ? Styles.Fullscreen : Styles.Default
            );

            View gameView = new View(new FloatRect(0, 0, width, height));
            View renderView = gameView;

            _window.Closed += (sender, e) => _window.Close();
            _window.Resized += (sender, e) =>
            {
                UpdateViewForAspectRatio(gameView, renderView);
            };

            _gameClock = new Clock();
            Input.Initialize(_window);

            UpdateViewForAspectRatio(gameView, renderView);

            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                _window.Clear(Color.Black);

                _window.SetView(gameView);

                float deltaTime = _gameClock.Restart().AsSeconds();
                Time.Update(deltaTime);
                GameEngine.Update(Time.DeltaTime);

                _window.SetView(renderView);
                GameEngine.ActiveScene?.Draw(_window);

                _window.Display();
            }
        }

        private void UpdateViewForAspectRatio(View gameView, View renderView)
        {
            float gameAspect = gameView.Size.X / gameView.Size.Y;
            float windowAspect = (float)_window.Size.X / _window.Size.Y;

            if (windowAspect > gameAspect)
            {
                float viewWidth = gameView.Size.Y * windowAspect;
                renderView.Viewport = new FloatRect(
                    (1 - gameAspect / windowAspect) / 2, 0,
                    gameAspect / windowAspect, 1
                );
            }
            else
            {
                float viewHeight = gameView.Size.X / windowAspect;
                renderView.Viewport = new FloatRect(
                    0, (1 - windowAspect / gameAspect) / 2,
                    1, windowAspect / gameAspect
                );
            }

            renderView.Size = gameView.Size;
            renderView.Center = gameView.Center;
        }
    }
}