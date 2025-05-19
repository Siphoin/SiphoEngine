using SFML.System;
using SiphoEngine.MathExtensions;

namespace SiphoEngine.Physics
{
    public static class PhysicsEngine
    {
        private static List<Rigidbody> _rigidbodies = new List<Rigidbody>();
        private static Vector2f _gravity = new Vector2f(0, 9.8f);
        public static bool EnableGravity = true;

        public static Vector2f Gravity { get => _gravity; set => _gravity = value; }

        internal static void Initialize(Vector2f? gravity = null)
        {
            if (gravity.HasValue)
            {
                _gravity = gravity.Value;
            }
        }
        internal static void RegisterRigidbody(Rigidbody rigidbody)
        {
            if (!_rigidbodies.Contains(rigidbody))
                _rigidbodies.Add(rigidbody);
        }

        internal static void UnregisterRigidbody(Rigidbody rigidbody)
        {
            if (rigidbody != null && _rigidbodies.Contains(rigidbody))
            {
                _rigidbodies.Remove(rigidbody);
                rigidbody.Dispose(); // Добавляем очистку
            }
        }

        internal static void Update(float fixedTime)
        {
            foreach (var rb in _rigidbodies)
            {
                if (EnableGravity && rb.UseGravity)
                    rb.AddForce(_gravity * rb.Mass);

                rb.UpdatePhysics(fixedTime);
            }

            for (int i = 0; i < _rigidbodies.Count; i++)
            {
                for (int j = i + 1; j < _rigidbodies.Count; j++)
                {
                    if (CheckCollision(_rigidbodies[i], _rigidbodies[j], out CollisionInfo info))
                    {
                        ResolveCollision(info);
                    }
                }
            }
        }

        private static bool CheckCollision(Rigidbody a, Rigidbody b, out CollisionInfo info)
        {
            info = new CollisionInfo();
            return a.Collider.CheckCollision(b.Collider, ref info);
        }

        private static void ResolveCollision(CollisionInfo info)
        {
            if (info.A == null || info.B == null) return;

            float pushFactor = 0.2f;
            info.A.Transform.Position += info.Normal * info.Depth * pushFactor;
            info.B.Transform.Position -= info.Normal * info.Depth * pushFactor;

            Vector2f relativeVelocity = info.B.Velocity - info.A.Velocity;
            float velocityAlongNormal = relativeVelocity.Dot(info.Normal);

            if (velocityAlongNormal > 0) return;

            float restitution = 0.8f;
            float j = -(1 + restitution) * velocityAlongNormal;
            j /= 1 / info.A.Mass + 1 / info.B.Mass;

            Vector2f impulse = j * info.Normal;
            info.A.ApplyImpulse(-impulse);
            info.B.ApplyImpulse(impulse);
        }
    }
}