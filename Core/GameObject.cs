using SiphoEngine.Core.PlayerLoop;
using SiphoEngine.Core.SiphoEngine;

namespace SiphoEngine.Core
{
    public class GameObject : Object
    {
        private List<Component> _components = new List<Component>();
        public Transform Transform { get; private set; }

        public string Name { get; set; }
        public bool ActiveSelf { get; set; } = true;
        public Scene Scene { get; internal set; }

        public GameObject(string name = "GameObject")
        {
            Name = name;
            Transform = AddComponent<Transform>();
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
    }
}
