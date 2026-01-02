using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    public static class UfoFactory
    {
        public static GameEntity CreateUfo(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            TransformComponent playerTransform,
            EnemySettings enemySettings,
            ScreenBounds screenBounds)
        {
            var entity = EnemyFactory.CreateEnemy(EnemyType.Ufo, position, rotation, signalBus, screenBounds);

            var ufoComponent = new UfoComponent();
            entity.AddComponent(ufoComponent);

            var movement = new UfoMovement(entity, playerTransform, enemySettings.UfoSpeed);
            entity.AddComponent(movement);

            return entity;
        }
    }
}

