using UnityEngine;
using Zenject;

namespace Asteroids.Core.Entity.Components
{
    /// <summary>
    /// Component for physics data (velocity, forces, mass)
    /// Automatically updates transform position each tick
    /// </summary>
    public class PhysicsComponent : ITickableComponent
    {
        private readonly PhysicsChangedSignal _signal = new PhysicsChangedSignal();

        private readonly TransformComponent _transform;
        private readonly SignalBus _signalBus;
        private readonly float _frictionCoefficient;

        public Vector2 Velocity { get; private set; }
        public float Mass { get; private set; }
        public float FrictionCoefficient => _frictionCoefficient;

        /// <summary>
        /// Create physics component with all required dependencies
        /// </summary>
        public PhysicsComponent(TransformComponent transform, SignalBus signalBus, float mass = 1f, float frictionCoefficient = 1f)
        {
            _transform = transform;
            _signalBus = signalBus;
            Mass = mass;
            Velocity = Vector2.zero;
            _frictionCoefficient = frictionCoefficient;
        }

        public void SetVelocity(Vector2 velocity, SignalBus signalBus = null)
        {
            Velocity = velocity;
            FireSignal(signalBus);
        }

        public void AddVelocity(Vector2 delta, SignalBus signalBus = null)
        {
            Velocity += delta;
            FireSignal(signalBus);
        }

        public void ApplyForce(Vector2 force, SignalBus signalBus = null)
        {
            if (Mass > 0)
            {
                Velocity += force / Mass * Time.deltaTime;
                FireSignal(signalBus);
            }
        }

        public void ApplyImpulse(Vector2 impulse, SignalBus signalBus = null)
        {
            if (Mass > 0)
            {
                Velocity += impulse / Mass;
                FireSignal(signalBus);
            }
        }

        public void ApplyFriction(float frictionCoefficient, float deltaTime, SignalBus signalBus = null)
        {
            Velocity *= Mathf.Pow(frictionCoefficient, deltaTime);
            FireSignal(signalBus);
        }

        public void ClampSpeed(float maxSpeed)
        {
            if (Velocity.magnitude > maxSpeed)
            {
                Velocity = Velocity.normalized * maxSpeed;
            }
        }

        /// <summary>
        /// Called each frame to update physics (friction and position integration)
        /// </summary>
        public void Tick()
        {
            // Apply friction automatically
            if (_frictionCoefficient < 1f)
            {
                Velocity *= Mathf.Pow(_frictionCoefficient, Time.deltaTime);
            }

            // Update position based on velocity
            _transform.Move(Velocity * Time.deltaTime, _signalBus);
            FireSignal(_signalBus);
        }

        private void FireSignal(SignalBus signalBus)
        {
            if (signalBus == null)
            {
                return;
            }
            _signal.VelocityX = Velocity.x;
            _signal.VelocityY = Velocity.y;
            _signal.Speed = Velocity.magnitude;
            signalBus.Fire(_signal);
        }
    }
}

