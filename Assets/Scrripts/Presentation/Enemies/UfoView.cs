using UnityEngine;
using Zenject;
using Asteroids.Core.Enemies;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

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
            UfoFactory ufoFactory,
            TickableManager tickableManager)
        {
            _container = container;
            _tickableManager = tickableManager;

            // Get position and rotation from Unity transform
            Vector2 position = new Vector2(transform.position.x, transform.position.y);
            float rotation = transform.eulerAngles.z;

            // Create base enemy entity using UfoFactory
            Entity = ufoFactory.CreateUfo(position, rotation, _maxHealth, signalBus);

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

