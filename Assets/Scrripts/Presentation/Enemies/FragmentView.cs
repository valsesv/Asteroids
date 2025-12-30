using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// Fragment view that creates GameEntity using FragmentFactory
    /// Fragments are spawned when asteroids are destroyed by bullets
    /// </summary>
    public class FragmentView : EnemyView
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

            // Create base enemy entity using static FragmentFactory (no health needed)
            Entity = FragmentFactory.CreateFragment(position, rotation, signalBus);

            // Add movement component (sets initial velocity) - use FragmentSpeed from settings
            var physics = Entity.GetComponent<PhysicsComponent>();
            var movement = new AsteroidMovement(Entity, physics, _direction, enemySettings.FragmentSpeed, signalBus);
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
        /// Set movement direction - used when spawning fragment
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
        /// Handle fragment death - return to pool (fragments don't fragment further)
        /// </summary>
        protected override void HandleEnemyDeath()
        {
            base.HandleEnemyDeath();

            // Return fragment to pool
            _enemySpawner.ReturnEnemy(this);
        }
    }
}

