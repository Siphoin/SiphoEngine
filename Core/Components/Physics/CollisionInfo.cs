using SFML.System;

namespace SiphoEngine.Physics
{
    public struct CollisionInfo
    {
        public Rigidbody A;
        public Rigidbody B;
        public Vector2f Normal;
        public float Depth;
    }
}
