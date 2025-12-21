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
        [SerializeField] private float _speed = 5f;

        private DiContainer _container;
        private TickableManager _tickableManager;

        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            DiContainer container,
            AsteroidFactory asteroidFactory,
            TickableManager tickableManager)
        {
            _container = container;
            _tickableManager = tickableManager;

            // Get position and rotation from Unity transform
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            // Create base enemy entity using AsteroidFactory
            Entity = asteroidFactory.CreateAsteroidEntity(position, rotation, _maxHealth, signalBus);

            // Add asteroid-specific component
            var asteroidComponent = new AsteroidComponent(_size);
            Entity.AddComponent(asteroidComponent);

            // Add movement component (sets initial velocity)
            var physics = Entity.GetComponent<PhysicsComponent>();
            var movement = new AsteroidMovement(Entity, physics, _direction, _speed, signalBus);
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
    }
}

