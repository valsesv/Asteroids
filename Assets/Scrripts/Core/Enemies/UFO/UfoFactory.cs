using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Factory for creating UFO entities
    /// Inherits from EnemyFactory to reuse common enemy creation logic
    /// </summary>
    public class UfoFactory : EnemyFactory
    {
        /// <summary>
        /// Create a UFO entity using base EnemyFactory logic
        /// </summary>
        public GameEntity CreateUfo(
            Vector2 position,
            float rotation,
            float maxHealth,
            SignalBus signalBus)
        {
            // Use protected CreateEnemy method from base class
            var entity = CreateEnemy(EnemyType.Ufo, position, rotation, maxHealth, signalBus);

            // Add UFO-specific component
            var ufoComponent = new UfoComponent();
            entity.AddComponent(ufoComponent);

            return entity;
        }
    }
}

