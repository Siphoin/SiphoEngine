using SFML.Graphics;
using SiphoEngine.Core.Components.Render;
using SiphoEngine.Core.SiphoEngine;


namespace SiphoEngine
{
    public static class GameEngine
    {
        public static event Action OnLoadingPrefabs;
        private static Scene _activeScene;
        private static List<Scene> _scenes = new List<Scene>();

        public static RenderWindow? MainWindow { get; private set; }
        private static List<Camera> _cameras = new List<Camera>();

        public static Scene ActiveScene => _activeScene;

        internal static void InitializePrefabs()
        {
            // Создаем защищенную директорию для префабов
            var prefabsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Prefabs");
            if (!Directory.Exists(prefabsDir))
            {
                Directory.CreateDirectory(prefabsDir);
            }

            OnLoadingPrefabs?.Invoke();
        }

        internal static void InitializeWindow(RenderWindow window)
        {
            MainWindow = window;
        }

        internal static void RegisterCamera(Camera camera)
        {
            _cameras.Add(camera);
            _cameras = _cameras.OrderByDescending(c => c.Priority).ToList();
        }

        internal static void BeforeRender()
        {
            if (_cameras.Count > 0)
            {
                MainWindow.SetView(Camera.Main.GetView());
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