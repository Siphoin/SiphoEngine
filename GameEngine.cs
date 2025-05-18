using SiphoEngine.Core.SiphoEngine;


namespace SiphoEngine
{
    public static class GameEngine
    {
        private static Scene _activeScene;
        private static List<Scene> _scenes = new List<Scene>();

        public static Scene ActiveScene => _activeScene;

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
    }
}