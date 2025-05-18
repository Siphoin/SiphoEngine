using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SiphoEngine.Core.PlayerLoop;

namespace SiphoEngine.Core.Components.Render
{
    public class Camera : Component, IAwakable
    {
        public float OrthographicSize { get; set; } = 5f;
        public int Priority { get; set; } = 0;
        private View _view;
        public Color BackgroundColor { get; set; } = Color.Black;

        public void Awake()
        {
            GameEngine.RegisterCamera(this);
            UpdateView();
        }

        public View GetView() => _view;

        public void UpdateView()
        {
            var windowSize = GameEngine.MainWindow.Size;
            float aspectRatio = (float)windowSize.X / windowSize.Y;

            // Фиксируем ширину, высота рассчитывается автоматически
            float viewHeight = OrthographicSize * 2f;
            float viewWidth = viewHeight * aspectRatio;

            _view = new View(Transform.Position, new Vector2f(viewWidth, viewHeight));
        }

        public void OnWindowResized(uint width, uint height)
        {
            UpdateView();
        }
    }
}
