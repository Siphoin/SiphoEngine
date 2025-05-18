using SFML.Graphics;
using SiphoEngine.Core.Components.Render;
using SiphoEngine.Core.SiphoEngine;


namespace SiphoEngine
{
    public static class GameEngine
    {
        private static Scene _activeScene;
        private static List<Scene> _scenes = new List<Scene>();

        public static RenderWindow? MainWindow { get; private set; }
        private static List<Camera> _cameras = new List<Camera>();

        public static Scene ActiveScene => _activeScene;

        public static void InitializeWindow(RenderWindow window)
        {
            MainWindow = window;
        }

        public static void RegisterCamera(Camera camera)
        {
            _cameras.Add(camera);
            _cameras = _cameras.OrderByDescending(c => c.Priority).ToList();
        }

        public static void BeforeRender()
        {
            if (_cameras.Count > 0)
            {
                MainWindow.SetView(_cameras[0].GetView());
            }
        }

        public static Scene AddScene(Scene scene)
        {
            _scenes.Add(scene);

            if (ActiveScene is null)
            {
                LoadScene(scene);
            }
            return scene;
        }

        public static void LoadScene(Scene scene)
        {
            _activeScene = scene;
            _activeScene.Initialize();
        }

        public static void Update(float deltaTime)
        {
            _activeScene?.Update(deltaTime);
        }

        internal static IEnumerable<Camera> GetAllCameras()
        {
            return _cameras;
        }
    }
}