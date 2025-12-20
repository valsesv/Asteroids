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
            SignalBus signalBus)
        {
            // Create base enemy entity
            var entity = EnemyFactory.CreateEnemy(EnemyType.Asteroid, position, rotation, maxHealth, signalBus);

            // Add asteroid-specific component
            var asteroidComponent = new AsteroidComponent(size);
            entity.AddComponent(asteroidComponent);

            return entity;
        }
    }
}

