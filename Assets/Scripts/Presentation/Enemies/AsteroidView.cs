using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.Enemies
{
    public class AsteroidView : EnemyView
    {
        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            EnemySettings enemySettings)
        {
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            Entity = AsteroidFactory.CreateAsteroidEntity(position, rotation, signalBus, enemySettings.AsteroidSpeed, screenBounds);

            _container.BindInstance(Entity).AsSingle();
            RegisterTickableComponents();
        }

        public void SetDirection(Vector2 direction)
        {
            var movement = Entity.GetComponent<AsteroidMovement>();
            if (movement != null)
            {
                movement.SetDirection(direction);
            }
        }

        public override void HandleEnemyDeath()
        {
            base.HandleEnemyDeath();

            var asteroidComponent = Entity?.GetComponent<AsteroidComponent>();
            if (asteroidComponent == null)
            {
                return;
            }
            var transformComponent = Entity.GetComponent<TransformComponent>();
            var physicsComponent = Entity.GetComponent<PhysicsComponent>();

            Vector2 position = transformComponent?.Position ?? Vector2.zero;
            Vector2 velocity = physicsComponent?.Velocity ?? Vector2.zero;

            _enemySpawner.FragmentAsteroid(this, position, velocity, asteroidComponent);
        }

        public override void HandleInstaDeath()
        {
            base.HandleInstaDeath();
        }
    }
}

