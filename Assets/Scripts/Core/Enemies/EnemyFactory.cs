using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    public static class EnemyFactory
    {
        public static GameEntity CreateEnemy(
            EnemyType type,
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            ScreenBounds screenBounds)
        {
            var entity = new GameEntity(position, rotation, signalBus);

            var enemyComponent = new EnemyComponent(type);
            entity.AddComponent(enemyComponent);

            var transform = entity.GetComponent<TransformComponent>();
            var physicsComponent = new PhysicsComponent(transform, signalBus, mass: 1f, frictionCoefficient: 1f);
            entity.AddComponent(physicsComponent);

            var transformComponent = entity.GetComponent<TransformComponent>();
            var screenWrap = new ScreenWrapComponent(transformComponent, screenBounds);
            entity.AddComponent(screenWrap);

            return entity;
        }
    }
}