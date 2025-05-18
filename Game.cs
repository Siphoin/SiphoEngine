using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace SiphoEngine
{
    public class Game
    {
        private RenderWindow? _window;
        private Clock? _gameClock;

        public void Run(uint width = 800, uint height = 600, string title = "SiphoEngine Game")
        {
            _window = new RenderWindow(new VideoMode(width, height), title);
            _window.Closed += (sender, e) => _window.Close();

            _gameClock = new Clock();


            while (_window.IsOpen)
            {
                _window.DispatchEvents();
                _window.Clear(Color.Black);

                float deltaTime = _gameClock.Restart().AsSeconds();
                GameEngine.Update(deltaTime);

                _window.Display();
            }
        }
    }
}