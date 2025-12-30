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
            SignalBus signalBus)
        {
            // Use static CreateEnemy method from EnemyFactory (maxHealth not used for asteroids)
            return EnemyFactory.CreateEnemy(EnemyType.Asteroid, position, rotation, 0f, signalBus);
        }
    }
}
