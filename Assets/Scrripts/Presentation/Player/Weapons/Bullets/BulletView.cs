using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Presentation.Enemies;

namespace Asteroids.Presentation.Player
{
    /// <summary>
    /// Bullet view - MonoBehaviour that represents a bullet in the scene
    /// Creates GameEntity in Construct method (like enemies do)
    /// </summary>
    public class BulletView : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; private set; }

        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _container;
        [Inject] private TickableManager _tickableManager;

        [Inject]
        public void Construct(BulletSettings bulletSettings)
        {
            // Get position and rotation from Unity transform (like enemies do)
            Vector2 position = new Vector2(transform.position.x, transform.position.y);

            // Create bullet entity using BulletFactory (like enemies use their factories)
            // Use speed and lifetime from settings (like enemies use settings)
            Entity = BulletFactory.CreateBullet(position, bulletSettings.Speed, bulletSettings.Lifetime, _signalBus);

            // Register Entity in container (like enemies do)
            _container.BindInstance(Entity).AsSingle();

            // Add ITickable components to TickableManager (like enemies do)
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

        public void Initialize()
        {
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        public void SetSpawnParameters(Vector2 position, Vector2 direction)
        {
            transform.position = new Vector3(position.x, position.y, 0f);

            var transformComponent = Entity.GetComponent<TransformComponent>();
            transformComponent.SetPosition(position);

            var movement = Entity.GetComponent<BulletMovement>();
            movement.SetDirection(direction);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _signalBus?.Fire(new BulletDestroyedSignal { Entity = Entity });
        }
    }
}

