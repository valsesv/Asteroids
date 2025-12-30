using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Factory for creating bullet entities (static methods like enemy factories)
    /// </summary>
    public static class BulletFactory
    {
        public static GameEntity CreateBullet(Vector2 position, Vector2 direction, SignalBus signalBus)
        {
            // Calculate rotation from direction
            float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var entity = new GameEntity(position, rotation, signalBus);

            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, signalBus, mass: 1f, frictionCoefficient: 1f);
            entity.AddComponent(physicsComponent);

            return entity;
        }
    }
}

