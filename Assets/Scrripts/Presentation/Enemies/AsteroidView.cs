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
        [SerializeField] private Vector2 _direction = Vector2.right;

        private DiContainer _container;

        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            DiContainer container,
            TickableManager tickableManager,
            EnemySettings enemySettings)
        {
            _container = container;

            // Get position and rotation from Unity transform
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            // Create base enemy entity using static AsteroidFactory (no health needed)
            Entity = AsteroidFactory.CreateAsteroidEntity(position, rotation, signalBus);

            // Add asteroid-specific component (no size needed)
            var asteroidComponent = new AsteroidComponent();
            Entity.AddComponent(asteroidComponent);

            // Add movement component (sets initial velocity) - use speed from settings
            var physics = Entity.GetComponent<PhysicsComponent>();
            var movement = new AsteroidMovement(Entity, physics, _direction, enemySettings.AsteroidSpeed, signalBus);
            Entity.AddComponent(movement);

            // Add screen wrap component for teleportation at screen boundaries
            var transformComponent = Entity.GetComponent<TransformComponent>();
            var screenWrap = new ScreenWrapComponent(transformComponent, screenBounds, signalBus);
            Entity.AddComponent(screenWrap);

            // Register Entity in container
            _container.BindInstance(Entity).AsSingle();
        }

        /// <summary>
        /// Set movement direction - used when spawning asteroid
        /// </summary>
        public void SetDirection(Vector2 direction)
        {
            if (Entity != null)
            {
                var movement = Entity.GetComponent<AsteroidMovement>();
                if (movement != null)
                {
                    movement.SetDirection(direction);
                }
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
            if (asteroidComponent != null)
            {
                // Get position and velocity for fragmentation
                var transformComponent = Entity.GetComponent<TransformComponent>();
                var physicsComponent = Entity.GetComponent<PhysicsComponent>();

                Vector2 position = transformComponent?.Position ?? Vector2.zero;
                Vector2 velocity = physicsComponent?.Velocity ?? Vector2.zero;

                // Fragment the asteroid into Fragment enemies
                _enemySpawner.FragmentAsteroid(this, position, velocity, asteroidComponent);
            }
            else
            {
                // No asteroid component: return to pool
                _enemySpawner.ReturnEnemy(this);
            }
        }
    }
}

