using System.Text.Json.Serialization;
using SFML.System;
using SiphoEngine.Core;
using SiphoEngine.Core.PlayerLoop;

namespace SiphoEngine.Physics
{
    public class Rigidbody : Component, IAwakable
    {
        private  float _restitution = 0.5f; // Коэффициент упругости по умолчанию

        private Vector2f _velocity;
        private Vector2f _force;
        private float _mass = 1f;
        [JsonIgnore]
        private Collider _collider;

        public float Mass
        {
            get => _mass;
            set => _mass = value > 0 ? value : 1f;
        }

        public Vector2f Velocity => _velocity;
        public bool UseGravity { get; set; } = true;

        public float InverseMass { get; private set; } = 1f;
        public float Restitution
        {
            get => _restitution;
            set => _restitution = Math.Clamp(value, 0f, 1f);
        }
        public Collider? Collider => _collider ??= GameObject.GetComponent<Collider>();

        public void Awake()
        {
            _collider = GameObject.GetComponent<Collider>();
            PhysicsEngine.RegisterRigidbody(this);
        }

        public void AddForce(Vector2f force)
        {
            _force += force;
        }

        public void ApplyImpulse(Vector2f impulse)
        {
            _velocity += impulse / _mass;
        }

        internal void UpdatePhysics(float deltaTime)
        {
            Vector2f acceleration = _force / _mass;
            _velocity += acceleration * deltaTime;
            GameObject.Transform.Position += _velocity * deltaTime;

            _force = new Vector2f(0, 0);
        }



    }
}
