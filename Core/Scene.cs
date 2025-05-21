using SFML.Graphics;
using SiphoEngine.Core.Debugging;
using SiphoEngine.Core.Physics;
using SiphoEngine.Core.PlayerLoop;
using SiphoEngine.Physics;

namespace SiphoEngine.Core
{
    namespace SiphoEngine
    {
        public abstract class Scene : Object
        {
            private List<GameObject> _gameObjects = new List<GameObject>();
            private List<IUpdatable> _updatables = new List<IUpdatable>();
            private List<IFixedUpdatable> _fixedUpdatables = new List<IFixedUpdatable>();
            private List<ILateUpdatable> _lateUpdatables = new List<ILateUpdatable>();
            private List<IStartable> _startables = new List<IStartable>();
            private List<IAwakable> _awakables = new List<IAwakable>();
            private List<IDrawable> _drawables = new List<IDrawable>();

            public string Name { get; private set; }
            public bool IsInitialized { get; private set; }
            public int CountGameObjects => _gameObjects.Count;

            public Scene()
            {
               Name = GetType().Name;
            }

            public Scene (string name)
            {
                Name = name;
            }

            public GameObject CreateGameObject(string name = "GameObject")
            {
                var gameObject = new GameObject(name);
                AddGameObject(gameObject);
                return gameObject;
            }

            public void AddGameObject (GameObject gameObject)
            {
                if (Contains(gameObject))
                {
                    throw new InvalidOperationException($"gamebject {gameObject.Name} contains in scene");
                }
                gameObject.Scene = this;
                _gameObjects.Add(gameObject);

                foreach (var component in gameObject.Components)
                {
                    RegisterComponent(component);
                }
            }

            private bool Contains (GameObject gameObject)
            {
                return _gameObjects.Contains(gameObject);
            }

            internal void RegisterComponent(Component component)
            {
                if (component is IAwakable awakable)
                {
                    if (!_awakables.Contains(awakable))
                    {
                        awakable.Awake();
                        _awakables.Add(awakable);
                    }
                }

                if (component is IStartable startable)
                {
                    if (!_startables.Contains(startable))
                    {
                        startable.Start();
                        _startables.Add(startable);
                    }
                }
                if (component is IUpdatable updatable) _updatables.Add(updatable);
                if (component is IFixedUpdatable fixedUpdatable) _fixedUpdatables.Add(fixedUpdatable);
                if (component is ILateUpdatable lateUpdatable) _lateUpdatables.Add(lateUpdatable);
                if (component is IDrawable drawable) _drawables.Add(drawable);
            }

            internal void UnregisterComponent(Component component)
            {
                if (component is IAwakable awakable) _awakables.Remove(awakable);
                if (component is IStartable startable) _startables.Remove(startable);
                if (component is IUpdatable updatable) _updatables.Remove(updatable);
                if (component is IFixedUpdatable fixedUpdatable) _fixedUpdatables.Remove(fixedUpdatable);
                if (component is ILateUpdatable lateUpdatable) _lateUpdatables.Remove(lateUpdatable);
                if (component is IDrawable drawable) _drawables.Remove(drawable);


            }

            internal void DestroyGameObject (GameObject go)
            {
                foreach (var item in go.Components)
                {
                    UnregisterComponent(item);

                    if (item is Rigidbody rigidbody)
                    {
                        PhysicsEngine.UnregisterRigidbody(rigidbody);
                    }

                    DestroyComponent(item);
                }
                _gameObjects.Remove(go);
                go.Dispose();
            }

            internal void DestroyComponent (Component component)
            {
                component?.Dispose();
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

                for (int i = 0; i < _updatables.Count; i++)
                {
                    IUpdatable? updatable = _updatables[i];
                    updatable.Update();
                }

                for (int i = 0; i < _gameObjects.Count; i++)
                {
                    GameObject? item = _gameObjects[i];
                    item.UpdateCoroutineRunner();
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
                    for (int i = 0; i < _fixedUpdatables.Count; i++)
                    {
                        IFixedUpdatable? fixedUpdatable = _fixedUpdatables[i];
                        fixedUpdatable.FixedUpdate();
                    }
                    fixedUpdateAccumulator -= Time.FixedDeltaTime;
                }
            }

            private void HandleLateUpdate()
            {
                for (int i = 0; i < _lateUpdatables.Count; i++)
                {
                    ILateUpdatable? lateUpdatable = _lateUpdatables[i];
                    lateUpdatable.LateUpdate();
                }
            }

            internal void Draw(RenderTarget target)
            {
                for (int i = 0; i < _drawables.Count; i++)
                {
                    IDrawable? drawable = _drawables[i];
                    drawable.Draw(target);
                }
            }

            internal void Clear ()
            {
#if DEBUG
                int countUnload = 0;
#endif
                for (int i = 0; i < _gameObjects.Count; i++)
                {
                    _gameObjects[i].Dispose();
                    DestroyGameObject(_gameObjects[i]);
#if DEBUG
                    countUnload++;
#endif
                }

#if DEBUG
                Debug.Log($"Unload scene {Name}: count unloaded GameObjects: {countUnload}");
#endif

                _drawables.Clear();
                _gameObjects.Clear();
                _awakables.Clear();
                _updatables.Clear();
                _fixedUpdatables.Clear();
                _lateUpdatables.Clear();
                PhysicsEngine.ClearRigidbodies();
                GC.Collect();
            }

            public override void Destroy()
            {
                throw new InvalidOperationException($"scene not support Destroy");
            }


            public  T FindObjectWithType<T>() where T : Component
            {
                var component = _gameObjects.FirstOrDefault(x => x.TryGetComponent(out T _));

                if (component is null)
                {
                    return null;
                }

                return component as T;
            }
            public IEnumerable<T> FindObjectsWithType<T>() where T : Component
            {
                List<T> list = new List<T>();
                foreach (var gameObject in _gameObjects)
                {
                    foreach (var component in gameObject.Components)
                    {
                        if (component is T targetComponent)
                        {
                            list.Add(targetComponent);
                        }
                    }
                }

                return list;
            }



        }
    }
}
