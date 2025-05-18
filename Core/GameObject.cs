using SiphoEngine.Core.PlayerLoop;
using SiphoEngine.Core.SiphoEngine;

namespace SiphoEngine.Core
{
    public class GameObject : Object, IDisposable
    {
        private List<Component> _components = new List<Component>();
        private bool _activeSelf;

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

        public override void Destroy ()
        {
            Scene?.DestroyGameObject(this);
            Dispose();
        }

        public virtual void OnEnable() { }
        public virtual void OnDisable() { }


        public void Dispose()
        {
            foreach (var item in _components)
            {
                item.Dispose();
            }
        }
    }
}
