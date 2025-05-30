﻿using System.Text.Json.Serialization;
using SFML.System;
using SiphoEngine.Core;
using SiphoEngine.Core.Physics;
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
        private bool _isTrigger;

        public float Mass
        {
            get => _mass;
            set => _mass = value > 0 ? value : 1f;
        }

        public Vector2f Velocity => _velocity;
        public bool UseGravity { get; set; } = true;

        public bool IsTrigger
        {
            get => _isTrigger;
            set
            {
                _isTrigger = value;
                if (_collider != null)
                {
                    _collider.IsTrigger = value;
                }
            }
        }
        public Collider? Collider => _collider ??= GameObject.GetComponent<Collider>();

        public void Awake()
        {
            _collider = GameObject.GetComponent<Collider>();
            if (_collider != null)
            {
                _collider.IsTrigger = _isTrigger;
            }
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


        public override void Dispose()
        {
            PhysicsEngine.UnregisterRigidbody(this);
            base.Dispose();
        }


    }
}
