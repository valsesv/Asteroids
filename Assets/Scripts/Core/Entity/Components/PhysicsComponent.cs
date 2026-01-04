using UnityEngine;

namespace Asteroids.Core.Entity.Components
{
    public class PhysicsComponent : ITickableComponent
    {
        private readonly TransformComponent _transform;

        public Vector2 Velocity { get; private set; }
        public float Mass { get; private set; }
        public float FrictionCoefficient { get; private set; }

        public PhysicsComponent(TransformComponent transform, float mass = 1f, float frictionCoefficient = 1f)
        {
            _transform = transform;
            Mass = mass;
            Velocity = Vector2.zero;
            FrictionCoefficient = frictionCoefficient;
        }

        public void Tick()
        {
            if (FrictionCoefficient < 1f)
            {
                Velocity *= Mathf.Pow(FrictionCoefficient, Time.deltaTime);
            }

            _transform.Move(Velocity * Time.deltaTime);
        }

        public void AddVelocity(Vector2 delta)
        {
            Velocity += delta;
        }

        public void ApplyForce(Vector2 force)
        {
            if (Mass > 0)
            {
                Velocity += force / Mass * Time.deltaTime;
            }
        }

        public void ApplyFriction(float frictionCoefficient, float deltaTime)
        {
            Velocity *= Mathf.Pow(frictionCoefficient, deltaTime);
        }

        public void ApplyImpulse(Vector2 impulse)
        {
            if (Mass > 0)
            {
                Velocity += impulse / Mass;
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
        }
    }
}