using SFML.System;
using SiphoEngine.Physics;

namespace SiphoEngine.Core.Physics
{
    public struct CollisionInfo
    {
        public Rigidbody A {  get; set; }
        public Rigidbody B { get; set; }
        public Vector2f Normal { get; set; }
        public float Depth { get; set; }
    }
}
