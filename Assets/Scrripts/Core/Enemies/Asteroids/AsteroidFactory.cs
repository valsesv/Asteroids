using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Factory for creating asteroid entities
    /// Uses EnemyFactory to reuse common enemy creation logic
    /// </summary>
    public static class AsteroidFactory
    {
        /// <summary>
        /// Create an asteroid entity using EnemyFactory logic
        /// Asteroids don't have health - they die immediately on bullet hit
        /// </summary>
        public static GameEntity CreateAsteroidEntity(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            float speed,
            ScreenBounds screenBounds)
        {
            var entity = EnemyFactory.CreateEnemy(EnemyType.Asteroid, position, rotation, signalBus, screenBounds);
            // Use static CreateEnemy method from EnemyFactory (maxHealth not used for asteroids)

            var asteroidComponent = new AsteroidComponent();
            entity.AddComponent(asteroidComponent);

            // Add movement component (sets initial velocity) - use speed from settings
            var movement = new AsteroidMovement(entity, speed, signalBus);
            entity.AddComponent(movement);

            return entity;
        }
    }
}
