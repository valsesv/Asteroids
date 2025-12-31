using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// Asteroid view that creates GameEntity using AsteroidFactory
    /// </summary>
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

        /// <summary>
        /// Set movement direction - used when spawning asteroid
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            var movement = Entity.GetComponent<AsteroidMovement>();
            if (movement != null)
            {
                movement.SetDirection(direction);
            }
        }

        /// <summary>
        /// Handle asteroid death - always fragment into Fragment enemies
        /// </summary>
        protected override void HandleEnemyDeath()
        {
            base.HandleEnemyDeath();

            // Get asteroid component
            var asteroidComponent = Entity?.GetComponent<AsteroidComponent>();
            if (asteroidComponent == null)
            {
                return;
            }
            // Get position and velocity for fragmentation
            var transformComponent = Entity.GetComponent<TransformComponent>();
            var physicsComponent = Entity.GetComponent<PhysicsComponent>();

            Vector2 position = transformComponent?.Position ?? Vector2.zero;
            Vector2 velocity = physicsComponent?.Velocity ?? Vector2.zero;

            // Fragment the asteroid into Fragment enemies
            _enemySpawner.FragmentAsteroid(this, position, velocity, asteroidComponent);
        }
    }
}

