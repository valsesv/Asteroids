using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Factory for creating bullet entities
    /// </summary>
    public class BulletFactory
    {
        private readonly SignalBus _signalBus;

        public BulletFactory(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        /// <summary>
        /// Create a bullet entity
        /// </summary>
        public GameEntity CreateBullet(Vector2 position, Vector2 direction, float speed, float lifetime)
        {
            // Calculate rotation from direction
            float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            var entity = new GameEntity(position, rotation, _signalBus);

            // Add bullet component
            var bulletComponent = new BulletComponent(speed, lifetime);
            entity.AddComponent(bulletComponent);

            // Add physics component with initial velocity
            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, _signalBus, mass: 0.1f, frictionCoefficient: 1f);
            physicsComponent.SetVelocity(direction.normalized * speed);
            entity.AddComponent(physicsComponent);

            // Add bullet lifetime component
            var bulletLifetime = new BulletLifetime(entity, _signalBus);
            entity.AddComponent(bulletLifetime);

            // Add collision handler
            var collisionHandler = new BulletCollisionHandler(entity, _signalBus);
            entity.AddComponent(collisionHandler);

            // Fire signal that bullet was created
            _signalBus?.Fire(new BulletCreatedSignal { Entity = entity });

            return entity;
        }
    }
}

