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
        [SerializeField] private AsteroidSize _size = AsteroidSize.Large;
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private Vector2 _direction = Vector2.right;

        private DiContainer _container;
        private TickableManager _tickableManager;

        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            DiContainer container,
            TickableManager tickableManager,
            EnemySettings enemySettings)
        {
            _container = container;
            _tickableManager = tickableManager;

            // Get position and rotation from Unity transform
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            // Create base enemy entity using static AsteroidFactory
            Entity = AsteroidFactory.CreateAsteroidEntity(position, rotation, _maxHealth, signalBus);

            // Add asteroid-specific component
            var asteroidComponent = new AsteroidComponent(_size);
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

            // Add ITickable components to TickableManager so they update automatically
            RegisterTickableComponents();
        }

        private void RegisterTickableComponents()
        {
            // Add ITickable components directly to TickableManager
            // This allows them to be updated even if they were created after TickableManager initialization
            foreach (var tickableComponent in Entity.GetTickableComponents())
            {
                _tickableManager.Add(tickableComponent);
            }
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
    }
}

