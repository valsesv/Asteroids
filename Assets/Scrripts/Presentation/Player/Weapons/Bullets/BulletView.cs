using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;

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
            Vector2 position = new Vector2(transform.position.x, transform.position.y);

            Entity = BulletFactory.CreateBullet(position, Vector2.zero, _signalBus);
            
            var bulletComponent = new BulletComponent(bulletSettings.Speed, bulletSettings.Lifetime);
            Entity.AddComponent(bulletComponent);

            var physics = Entity.GetComponent<PhysicsComponent>();
            var movement = new BulletMovement(Entity, physics, Vector2.zero, bulletSettings.Speed, _signalBus);
            Entity.AddComponent(movement);

            var bulletLifetime = new BulletLifetime(Entity, _signalBus);
            Entity.AddComponent(bulletLifetime);

            _container.BindInstance(Entity).AsSingle();

            RegisterTickableComponents();
        }

        private void RegisterTickableComponents()
        {
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

        public void SetSpawnPosition(Vector2 position)
        {
            // Update Unity transform
            transform.position = new Vector3(position.x, position.y, 0f);

            // Update TransformComponent if Entity is already created
            if (Entity == null)
            {
                return;
            }

            var transformComponent = Entity.GetComponent<TransformComponent>();
            if (transformComponent != null)
            {
                transformComponent.SetPosition(position);
            }
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        public void SetDirection(Vector2 direction)
        {
            var movement = Entity.GetComponent<BulletMovement>();
            movement?.SetDirection(direction);
        }
    }
}

