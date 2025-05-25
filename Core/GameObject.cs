using SiphoEngine.Core.Components;
using SiphoEngine.Core.Coroutines;
using SiphoEngine.Core.PlayerLoop;
using SiphoEngine.Core.SiphoEngine;

namespace SiphoEngine.Core
{
    public class GameObject : Object, IDisposable
    {
        private List<Component> _components = new List<Component>();
        private CoroutineRunner _coroutineRunner = new();
        private bool _activeSelf = true;

        public Transform Transform { get; private set; }

        public string Name { get; set; }
        public bool ActiveSelf
        {
            get
            {
                return _activeSelf;
            }

            set
            {
                _activeSelf = value;
                if (_activeSelf)
                {
                    OnEnable();
                }

                else
                {
                    OnDisable();
                }
            }
        }
        public Scene? Scene { get; internal set; }

        public bool IsPrefab { get; internal set; }

        public string Tag { get; set; }
        internal IEnumerable<Component> Components => _components;

        public ICoroutineRunner CoroutineRunner => _coroutineRunner;

        public GameObject(string name = "GameObject")
        {
            Name = name;
            Transform = AddComponent<Transform>();
        }

        public object AddComponent(Type type)
        {
            var component = Activator.CreateInstance(type) as Component;
            if (component == null)
                throw new ArgumentException($"Type {type.Name} is not a Component");

            component.GameObject = this;
            _components.Add(component);
            Scene?.RegisterComponent(component);
            return component;
        }

        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();
            component.GameObject = this;
            _components.Add(component);
            Scene?.RegisterComponent(component);
            return component;
        }

        public T? GetComponent<T>() where T : Component
        {
            return _components.Find(c => c is T) as T;
        }

        public bool TryGetComponent<T>(out T? result) where T : Component
        {
           object obj = _components.FirstOrDefault(x => x is T);

            if (obj != null)
            {
                result = obj as T;
            }

            result = null;

            return result != null;
        }

        public T GetComponentInParent<T>() where T : Component
        {
            Transform current = Transform;
            while (current != null)
            {

                if (current.GameObject.GetComponent<T>() is T component)
                    return component;

                current = current.Parent;
            }
            return default;
        }

        public T GetComponentInChildren<T>() where T : Component
        {
            foreach (var child in Transform.Children)
            {
                if (child.GameObject.GetComponent<T>() is T component)
                    return component;

                var childComponent = child.GameObject.GetComponentInChildren<T>();
                if (childComponent != null)
                    return childComponent;
            }
            return default;
        }

        public IEnumerable<T> GetComponentsInChildren<T>() where T : Component
        {
            foreach (var child in Transform.Children)
            {
                if (child.GameObject.GetComponent<T>() is T component)
                    yield return component;

                foreach (var childComponent in child.GameObject.GetComponentsInChildren<T>())
                    yield return childComponent;
            }
        }

        public void SetActive (bool active)
        {
            ActiveSelf = active;

        }

        public override void Destroy ()
        {
            StopAllCoroutines();
            Scene?.DestroyGameObject(this);
        }


        internal AsyncCoroutine StartCoroutine(IEnumerator<ICoroutineYield> coroutine)
        {
           return _coroutineRunner.StartCoroutine(coroutine);
        }

        internal void DelayAction(float delay, Action action)
        {
            _coroutineRunner.DelayAction(delay, action);
        }

        internal void StopAllCoroutines()
        {
            _coroutineRunner?.StopAllCoroutines();
        }

        internal void StopCoroutine(ref AsyncCoroutine coroutine)
        {
           _coroutineRunner.StopCoroutine(ref coroutine);
        }

        internal void UpdateCoroutineRunner ()
        {
            _coroutineRunner.Update(Time.FixedDeltaTime);
        }


        public void Dispose()
        {
            StopAllCoroutines();
            foreach (var item in _components)
            {
                item.Dispose();
            }
        }

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }

    }
}
