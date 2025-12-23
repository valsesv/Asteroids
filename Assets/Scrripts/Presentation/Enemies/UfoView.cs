using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// UFO view that creates GameEntity using UfoFactory
    /// </summary>
    public class UfoView : EnemyView
    {
        [SerializeField] private float _maxHealth = 100f;

        private DiContainer _container;
        private TickableManager _tickableManager;

        [Inject]
        public void Construct(
            SignalBus signalBus,
            ScreenBounds screenBounds,
            DiContainer container,
            TickableManager tickableManager,
            ShipView shipView,
            EnemySettings enemySettings)
        {
            _container = container;
            _tickableManager = tickableManager;

            // Get position and rotation from Unity transform
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            // Create base enemy entity using static UfoFactory
            Entity = UfoFactory.CreateUfo(position, rotation, _maxHealth, signalBus);

            // Get player TransformComponent from parent container (PlayerInstaller)
            TransformComponent playerTransform = shipView.Entity.GetComponent<TransformComponent>();

            // Add movement component (chases player with direct motion) - use speed from settings
            var physics = Entity.GetComponent<PhysicsComponent>();
            var transformComponent = Entity.GetComponent<TransformComponent>();
            var movement = new UfoMovement(Entity, physics, transformComponent, playerTransform, signalBus, enemySettings.UfoSpeed);
            Entity.AddComponent(movement);

            // Add screen wrap component for teleportation at screen boundaries
            var entityTransform = Entity.GetComponent<TransformComponent>();
            var screenWrap = new ScreenWrapComponent(entityTransform, screenBounds, signalBus);
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

