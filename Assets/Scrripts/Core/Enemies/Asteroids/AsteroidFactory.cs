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
        /// </summary>
        public static GameEntity CreateAsteroidEntity(
            Vector2 position,
            float rotation,
            float maxHealth,
            SignalBus signalBus)
        {
            // Use static CreateEnemy method from EnemyFactory
            return EnemyFactory.CreateEnemy(EnemyType.Asteroid, position, rotation, maxHealth, signalBus);
        }
    }
}
