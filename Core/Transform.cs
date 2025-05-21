using SFML.System;

namespace SiphoEngine.Core
{
    public class Transform : Component
    {
        private Vector2f _localPosition;
        private Transform _parent;
        private List<Transform> _children = new List<Transform>();
        private bool _isDirty = true;
        private Vector2f _cachedWorldPosition;

        public IReadOnlyList<Transform> Children => _children.AsReadOnly();

        public Transform GetChild(int index)
        {
            if (index < 0 || index >= _children.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            return _children[index];
        }

        public bool TryGetChild(int index, out Transform child)
        {
            child = null;
            if (index < 0 || index >= _children.Count)
                return false;

            child = _children[index];
            return true;
        }

        public int ChildCount => _children.Count;

        public Vector2f Position
        {
            get => _localPosition;
            set
            {
                _localPosition = value;
                SetDirty();
            }
        }

        public Transform Parent
        {
            get => _parent;
            set
            {
                if (_parent == value) return;
                
                _parent?._children.Remove(this);
                _parent = value;
                _parent?._children.Add(this);
                
                SetDirty();
            }
        }

        public Vector2f WorldPosition
        {
            get
            {
                if (_isDirty)
                {
                    _cachedWorldPosition = _parent == null
                        ? _localPosition
                        : _parent.WorldPosition + _localPosition;
                    _isDirty = false;
                }
                return _cachedWorldPosition;
            }
        }

        public Vector2f Scale { get; set; } = new Vector2f(1, 1);
        public float Rotation { get; set; } = 0f;

        protected void SetDirty()
        {
            if (_isDirty) return;
            _isDirty = true;
            
            foreach (var child in _children)
                child.SetDirty();
        }

        public void Translate(Vector2f translation)
        {
            Position += translation;
        }
    }
}