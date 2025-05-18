using SFML.System;

namespace SiphoEngine.Core
{
    public class Transform : Component
    {
        public Vector2f Position { get; set; } = new Vector2f(0, 0);
        public Vector2f Scale { get; set; } = new Vector2f(1, 1);
        public float Rotation { get; set; } = 0f;

        public void Translate(Vector2f translation)
        {
            Position += translation;
        }
    }
}
