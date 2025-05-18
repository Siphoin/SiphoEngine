using SFML.Graphics;
using SFML.System;
using SiphoEngine.Core.PlayerLoop;

namespace SiphoEngine.Core.Components.Render
{
    public class Camera : Component, IAwakable, IUpdatable
    {
        private View _view;
        public float OrthographicSize { get; set; } = 5f;
        public Color BackgroundColor { get; set; } = Color.Black;

        public static Camera? Main => GameEngine.GetAllCameras().FirstOrDefault(x => x.GameObject.Tag == Tags.MAIN_CAMERA_TAG);

        public int Priority { get; internal set; }

        public void Awake()
        {
            GameEngine.RegisterCamera(this);
            UpdateView();
        }

        public void Update()
        {
            UpdateView();
        }

        public View GetView() => _view;

        public void UpdateView()
        {
            var windowSize = GameEngine.MainWindow.Size;
            float aspectRatio = (float)windowSize.X / windowSize.Y;

            // Центр камеры в мировых координатах
            Vector2f viewCenter = Transform.Position;

            _view = new View(
                center: viewCenter,
                size: new Vector2f(OrthographicSize * 2 * aspectRatio, OrthographicSize * 2)
            );

        }

        public void OnWindowResized(uint width, uint height)
        {
            UpdateView();
        }
    }
}