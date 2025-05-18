using SiphoEngine.Core.PlayerLoop;

namespace SiphoEngine.Core
{
    namespace SiphoEngine
    {
        public abstract class Scene
        {
            private List<GameObject> _gameObjects = new List<GameObject>();
            private List<IUpdatable> _updatables = new List<IUpdatable>();
            private List<IFixedUpdatable> _fixedUpdatables = new List<IFixedUpdatable>();
            private List<ILateUpdatable> _lateUpdatables = new List<ILateUpdatable>();
            private List<IStartable> _startables = new List<IStartable>();
            private List<IAwakable> _awakables = new List<IAwakable>();

            public string Name { get; private set; }
            public bool IsInitialized { get; private set; }

            public Scene(string name)
            {
                Name = name;
            }

            public GameObject CreateGameObject(string name = "GameObject")
            {
                var gameObject = new GameObject(name);
                gameObject.Scene = this;
                _gameObjects.Add(gameObject);
                return gameObject;
            }

            internal void RegisterComponent(Component component)
            {
                if (component is IAwakable awakable) _awakables.Add(awakable);
                if (component is IStartable startable) _startables.Add(startable);
                if (component is IUpdatable updatable) _updatables.Add(updatable);
                if (component is IFixedUpdatable fixedUpdatable) _fixedUpdatables.Add(fixedUpdatable);
                if (component is ILateUpdatable lateUpdatable) _lateUpdatables.Add(lateUpdatable);
            }

            public virtual void Initialize()
            {
                if (IsInitialized) return;

                foreach (var awakable in _awakables)
                {
                    awakable.Awake();
                }

                foreach (var startable in _startables)
                {
                    startable.Start();
                }

                IsInitialized = true;
            }

            public void Update(float deltaTime)
            {
                foreach (var updatable in _updatables)
                {
                    updatable.Update();
                }

                HandleFixedUpdate(deltaTime);

                HandleLateUpdate();
            }

            private void HandleFixedUpdate(float deltaTime)
            {
                float fixedUpdateAccumulator = 0f;
                fixedUpdateAccumulator += deltaTime;
                while (fixedUpdateAccumulator >= Time.FixedDeltaTime)
                {
                    foreach (var fixedUpdatable in _fixedUpdatables)
                    {
                        fixedUpdatable.FixedUpdate();
                    }
                    fixedUpdateAccumulator -= Time.FixedDeltaTime;
                }
            }

            private void HandleLateUpdate()
            {
                foreach (var lateUpdatable in _lateUpdatables)
                {
                    lateUpdatable.LateUpdate();
                }
            }
        }
    }
}
