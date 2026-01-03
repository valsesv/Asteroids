using UnityEngine;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Zenject;

namespace Asteroids.Core.Enemies
{
    public class EnemyFactory
    {
        public GameEntity CreateEnemy(
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

        public GameEntity CreateAsteroid(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            float speed,
            ScreenBounds screenBounds)
        {
            var entity = CreateEnemy(EnemyType.Asteroid, position, rotation, signalBus, screenBounds);

            var asteroidComponent = new AsteroidComponent();
            entity.AddComponent(asteroidComponent);

            var movement = new AsteroidMovement(entity, speed);
            entity.AddComponent(movement);

            return entity;
        }

        public GameEntity CreateFragment(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            float speed,
            ScreenBounds screenBounds)
        {
            var entity = CreateEnemy(EnemyType.Fragment, position, rotation, signalBus, screenBounds);

            var fragmentComponent = new FragmentComponent();
            entity.AddComponent(fragmentComponent);

            var movement = new AsteroidMovement(entity, speed);
            entity.AddComponent(movement);

            return entity;
        }

        public GameEntity CreateUfo(
            Vector2 position,
            float rotation,
            SignalBus signalBus,
            TransformComponent playerTransform,
            EnemySettings enemySettings,
            ScreenBounds screenBounds)
        {
            var entity = CreateEnemy(EnemyType.Ufo, position, rotation, signalBus, screenBounds);

            var ufoComponent = new UfoComponent();
            entity.AddComponent(ufoComponent);

            var movement = new UfoMovement(entity, playerTransform, enemySettings.UfoSpeed, signalBus);
            entity.AddComponent(movement);

            return entity;
        }
    }
}
