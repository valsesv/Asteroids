using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    /// <summary>
    /// Base factory/Builder for creating enemy entities
    /// Creates GameEntity and adds enemy-specific components
    /// </summary>
    public static class EnemyFactory
    {
        /// <summary>
        /// Create a basic enemy entity with common components
        /// </summary>
        public static GameEntity CreateEnemy(
            EnemyType type,
            Vector2 position,
            float rotation,
            float maxHealth,
            SignalBus signalBus)
        {
            var entity = new GameEntity(position, rotation, signalBus);

            // Add enemy component
            var enemyComponent = new EnemyComponent(type);
            entity.AddComponent(enemyComponent);

            // Add physics component
            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, signalBus, mass: 1f, frictionCoefficient: 1f);
            entity.AddComponent(physicsComponent);

            return entity;
        }
    }
}