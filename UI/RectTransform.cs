using SFML.System;
using SiphoEngine.Core;

namespace SiphoEngine.UI
{
    public class RectTransform : Transform
    {
        private Vector2f _size = new Vector2f(100, 100);
        private Vector2f _pivot = new Vector2f(0.5f, 0.5f);

        public Vector2f Size
        {
            get => _size;
            set { if (_size != value) { _size = value; SetDirty(); } }
        }

        public Vector2f Pivot
        {
            get => _pivot;
            set { if (_pivot != value) { _pivot = value; SetDirty(); } }
        }

        public Vector2f Center => new Vector2f(
            WorldPosition.X - Size.X * Pivot.X,
            WorldPosition.Y - Size.Y * Pivot.Y
        );
    }
}