using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Factory for creating bullet entities (static methods like enemy factories)
    /// </summary>
    public static class BulletFactory
    {
        /// <summary>
        /// Create a bullet entity (static method like AsteroidFactory.CreateAsteroidEntity)
        /// </summary>
        public static GameEntity CreateBullet(Vector2 position, float speed, float lifetime, SignalBus signalBus)
        {
            var entity = new GameEntity(position, 0f, signalBus);

            // Add bullet component
            var bulletComponent = new BulletComponent(speed, lifetime);
            entity.AddComponent(bulletComponent);

            // Add physics component
            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, signalBus, mass: 0.1f, frictionCoefficient: 1f);
            entity.AddComponent(physicsComponent);

            // Add bullet movement component (sets initial velocity) - use speed from settings
            var movement = new BulletMovement(entity, physicsComponent, speed, signalBus);
            entity.AddComponent(movement);

            // Add bullet lifetime component
            var bulletLifetime = new BulletLifetime(entity, signalBus);
            entity.AddComponent(bulletLifetime);

            return entity;
        }
    }
}

