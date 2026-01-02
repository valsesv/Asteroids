using UnityEngine;
using Zenject;

namespace Asteroids.Core.Entity.Components
{
    public class PhysicsComponent : ITickableComponent
    {
        private readonly PhysicsChangedSignal _signal = new PhysicsChangedSignal();
        private readonly TransformComponent _transform;
        private readonly SignalBus _signalBus;
        private readonly float _frictionCoefficient;

        public Vector2 Velocity { get; private set; }
        public float Mass { get; private set; }
        public float FrictionCoefficient => _frictionCoefficient;

        public PhysicsComponent(TransformComponent transform, SignalBus signalBus, float mass = 1f, float frictionCoefficient = 1f)
        {
            _transform = transform;
            _signalBus = signalBus;
            Mass = mass;
            Velocity = Vector2.zero;
            _frictionCoefficient = frictionCoefficient;
        }

        public void Tick()
        {
            if (_frictionCoefficient < 1f)
            {
                Velocity *= Mathf.Pow(_frictionCoefficient, Time.deltaTime);
            }

            _transform.Move(Velocity * Time.deltaTime);
            FireSignal();
        }

        public void AddVelocity(Vector2 delta)
        {
            Velocity += delta;
            FireSignal();
        }

        public void ApplyForce(Vector2 force)
        {
            if (Mass > 0)
            {
                Velocity += force / Mass * Time.deltaTime;
                FireSignal();
            }
        }

        public void ApplyFriction(float frictionCoefficient, float deltaTime)
        {
            Velocity *= Mathf.Pow(frictionCoefficient, deltaTime);
            FireSignal();
        }

        public void ApplyImpulse(Vector2 impulse)
        {
            if (Mass > 0)
            {
                Velocity += impulse / Mass;
                FireSignal();
            }
        }

        public void ClampSpeed(float maxSpeed)
        {
            if (Velocity.magnitude > maxSpeed)
            {
                Velocity = Velocity.normalized * maxSpeed;
            }
        }

        public void SetVelocity(Vector2 velocity)
        {
            Velocity = velocity;
            FireSignal();
        }

        private void FireSignal()
        {
            if (_signalBus == null)
            {
                return;
            }
            _signal.VelocityX = Velocity.x;
            _signal.VelocityY = Velocity.y;
            _signal.Speed = Velocity.magnitude;
            _signalBus.Fire(_signal);
        }
    }
}

