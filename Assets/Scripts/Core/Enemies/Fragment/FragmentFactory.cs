using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Factory for creating Fragment entities
    /// Uses EnemyFactory to reuse common enemy creation logic
    /// </summary>
    public static class FragmentFactory
    {
        /// <summary>
        /// Create a Fragment entity using EnemyFactory logic
        /// Fragments don't have health - they die immediately on bullet hit
        /// </summary>
        public static GameEntity CreateFragment(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            float speed,
            ScreenBounds screenBounds)
        {
            // Use static CreateEnemy method from EnemyFactory (maxHealth not used for fragments)
            var entity = EnemyFactory.CreateEnemy(EnemyType.Fragment, position, rotation, signalBus, screenBounds);

            // Add Fragment-specific component
            var fragmentComponent = new FragmentComponent();
            entity.AddComponent(fragmentComponent);

            // Add movement component (sets initial velocity) - use FragmentSpeed from settings
            var movement = new AsteroidMovement(entity, speed, signalBus);
            entity.AddComponent(movement);

            return entity;
        }
    }
}

