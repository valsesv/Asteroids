using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Factory for creating UFO entities
    /// Uses EnemyFactory to reuse common enemy creation logic
    /// </summary>
    public static class UfoFactory
    {
        /// <summary>
        /// Create a UFO entity using EnemyFactory logic
        /// UFOs don't have health - they die immediately on bullet hit
        /// </summary>
        public static GameEntity CreateUfo(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            TransformComponent playerTransform,
            EnemySettings enemySettings,
            ScreenBounds screenBounds)
        {
            // Use static CreateEnemy method from EnemyFactory (maxHealth not used for UFOs)
            var entity = EnemyFactory.CreateEnemy(EnemyType.Ufo, position, rotation, signalBus, screenBounds);

            // Add UFO-specific component
            var ufoComponent = new UfoComponent();
            entity.AddComponent(ufoComponent);

            // Add movement component (chases player with direct motion) - use speed from settings
            var movement = new UfoMovement(entity, playerTransform, signalBus, enemySettings.UfoSpeed);
            entity.AddComponent(movement);

            return entity;
        }
    }
}

