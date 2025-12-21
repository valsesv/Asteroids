using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Factory for creating asteroid entities
    /// Inherits from EnemyFactory to reuse common enemy creation logic
    /// </summary>
    public class AsteroidFactory : EnemyFactory
    {
        /// <summary>
        /// Create an asteroid entity using base EnemyFactory logic
        /// </summary>
        public GameEntity CreateAsteroidEntity(
            Vector2 position,
            float rotation,
            float maxHealth,
            SignalBus signalBus)
        {
            // Use protected CreateEnemy method from base class
            return CreateEnemy(EnemyType.Asteroid, position, rotation, maxHealth, signalBus);
        }
    }
}
