using SFML.Graphics;
using SiphoEngine.Core.Components.Render;
using SiphoEngine.Core.Debugging;
using SiphoEngine.Core.SiphoEngine;


namespace SiphoEngine
{
    public static class GameEngine
    {
        public static event Action OnLoadingPrefabs;
        public static event Action OnLoadScenes;


        public static event Action<Scene> OnUnloadScene;
        public static event Action<Scene> OnLoadScene;


        private static Scene _activeScene;
        private static List<Scene> _scenes = new List<Scene>();

        internal static RenderWindow? MainWindow { get; private set; }
        private static List<Camera> _cameras = new List<Camera>();
        private static bool _isScenesLoaded;

        public static Scene ActiveScene => _activeScene;

        public static IEnumerable<Scene> Scenes => _scenes;

        internal static void InitializePrefabs()
        {
            var prefabsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources/Prefabs");
            if (!Directory.Exists(prefabsDir))
            {
                Directory.CreateDirectory(prefabsDir);
            }

            OnLoadingPrefabs?.Invoke();
        }

        internal static void InitializeScenes ()
        {
            if (_isScenesLoaded)
            {
                return;
            }
            OnLoadScenes?.Invoke();
            _isScenesLoaded = true;
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
            if (!scene)
            {
                throw new ArgumentNullException(nameof(scene));
            }
            if (_isScenesLoaded)
            {
                throw new InvalidOperationException($"You must adding scenes on {nameof(OnLoadScenes)} stage");
            }
            if (_scenes.Contains(scene))
            {
                throw new InvalidOperationException($"Scene {scene.Name} exist.");
            }

            _scenes.Add(scene);

            if (ActiveScene is null)
            {
                LoadScene(scene);
            }
                return scene;
        }

        public static void AddScenes(IEnumerable<Scene> scenes)
        {
            foreach (var scene in scenes)
            {
                AddScene(scene);
            }
          
        }

        public static void AddScenes(params Scene[] scenes)
        {
            foreach (var scene in scenes)
            {
                AddScene(scene);
            }

        }



        public static void LoadScene(Scene scene)
        {
            if (_activeScene)
            {
                _activeScene.Clear();

                OnUnloadScene?.Invoke(_activeScene);
                
            }
            _activeScene = scene;
            _activeScene.Initialize();
            OnLoadScene?.Invoke(_activeScene);
        }

        public static void LoadScene (string sceneName)
        {
            var scene = _scenes.SingleOrDefault(s => s.Name == sceneName);

            if (scene is null)
            {
                throw new NullReferenceException($"scene with name {sceneName} not found");
            }

            LoadScene(scene);
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