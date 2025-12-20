using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Factory for creating asteroid entities
    /// </summary>
    public static class AsteroidFactory
    {
        /// <summary>
        /// Create an asteroid entity with all required components
        /// </summary>
        public static GameEntity CreateAsteroid(
            AsteroidSize size,
            Vector2 position,
            float rotation,
            float maxHealth,
            Vector2 direction,
            float speed,
            SignalBus signalBus)
        {
            // Create base enemy entity
            var entity = EnemyFactory.CreateEnemy(EnemyType.Asteroid, position, rotation, maxHealth, signalBus);

            // Add asteroid-specific component
            var asteroidComponent = new AsteroidComponent(size);
            entity.AddComponent(asteroidComponent);

            // Add movement component (handles velocity and will handle collisions)
            var physics = entity.GetComponent<PhysicsComponent>();
            var movement = new AsteroidMovement(entity, physics, direction, speed, signalBus);
            entity.AddComponent(movement);

            return entity;
        }
    }
}

