using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;

namespace Asteroids.Core.Enemies
{
    public static class AsteroidFactory
    {
        public static GameEntity CreateAsteroidEntity(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            float speed,
            ScreenBounds screenBounds)
        {
            var entity = EnemyFactory.CreateEnemy(EnemyType.Asteroid, position, rotation, signalBus, screenBounds);

            var asteroidComponent = new AsteroidComponent();
            entity.AddComponent(asteroidComponent);

            var movement = new AsteroidMovement(entity, speed);
            entity.AddComponent(movement);

            return entity;
        }
    }
}
