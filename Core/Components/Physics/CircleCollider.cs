using SFML.System;
using SiphoEngine.Core;
using SiphoEngine.Core.Debugging;
using SiphoEngine.Core.Physics;

namespace SiphoEngine.Physics
{
    public class CircleCollider : Collider
    {
        public float Radius { get; set; } = 32f;
        public Vector2f Offset { get; set; }

        public override bool CheckCollision(Collider other, ref CollisionInfo info)
        {
#if DEBUG
            DebugDraw.DrawCircleCollider(Transform.Position, Radius, Offset);
#endif

            return CheckCollisionTypes(this, other, ref info);
        }

        internal bool CheckCircleCollision(CircleCollider other, ref CollisionInfo info)
        {
            Vector2f centerA = Transform.Position + Offset;
            Vector2f centerB = other.Transform.Position + other.Offset;

            Vector2f direction = centerB - centerA;
            float distanceSquared = direction.X * direction.X + direction.Y * direction.Y;
            float combinedRadius = Radius + other.Radius;
            float combinedRadiusSquared = combinedRadius * combinedRadius;

            if (distanceSquared > combinedRadiusSquared)
                return false;

            if (distanceSquared < float.Epsilon)
            {
                info.Normal = new Vector2f(1, 0);
                info.Depth = combinedRadius;
            }
            else
            {
                float distance = (float)Math.Sqrt(distanceSquared);
                info.Normal = direction / distance;
                info.Depth = combinedRadius - distance;
            }

            info.A = GameObject.GetComponent<Rigidbody>();
            info.B = other.GameObject.GetComponent<Rigidbody>();
            return true;
        }
    }
}