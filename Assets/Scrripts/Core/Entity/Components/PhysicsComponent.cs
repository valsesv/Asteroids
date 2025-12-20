using UnityEngine;

namespace Asteroids.Core.Entity.Components
{
    /// <summary>
    /// Component for physics data (velocity, forces, mass)
    /// </summary>
    public class PhysicsComponent : IComponent
    {
        public Vector2 Velocity { get; private set; }
        public float Mass { get; private set; }

        public PhysicsComponent(float mass = 1f)
        {
            Mass = mass;
            Velocity = Vector2.zero;
        }

        public void SetVelocity(Vector2 velocity)
        {
            Velocity = velocity;
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

        public void ApplyImpulse(Vector2 impulse)
        {
            if (Mass > 0)
            {
                Velocity += impulse / Mass;
            }
        }

        public void ApplyFriction(float frictionCoefficient, float deltaTime)
        {
            Velocity *= Mathf.Pow(frictionCoefficient, deltaTime);
        }

        public void ClampSpeed(float maxSpeed)
        {
            if (Velocity.magnitude > maxSpeed)
            {
                Velocity = Velocity.normalized * maxSpeed;
            }
        }
    }
}

