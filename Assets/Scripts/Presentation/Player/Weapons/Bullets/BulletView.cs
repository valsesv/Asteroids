using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Core.Weapons;

namespace Asteroids.Presentation.Player
{
    public class BulletView : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; private set; }

        [Inject] private SignalBus _signalBus;
        [Inject] private DiContainer _container;
        [Inject] private TickableManager _tickableManager;
        [Inject] private ProjectileSpawner _projectileSpawner;

        [Inject]
        public void Construct(BulletSettings bulletSettings)
        {
            var position = new Vector2(transform.position.x, transform.position.y);

            Entity = BulletFactory.CreateBullet(position, bulletSettings.Speed, bulletSettings.Lifetime, _signalBus);

            _container.BindInstance(Entity).AsSingle();
            RegisterTickableComponents();
        }

        public void Initialize()
        {
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus.Subscribe<BulletDestroyedSignal>(OnBulletDestroyed);
        }
        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus?.Unsubscribe<BulletDestroyedSignal>(OnBulletDestroyed);
        }

        public void SetSpawnParameters(Vector2 position, Vector2 direction)
        {
            transform.position = new Vector3(position.x, position.y, 0f);

            var transformComponent = Entity.GetComponent<TransformComponent>();
            transformComponent.SetPosition(position);

            var movement = Entity.GetComponent<BulletMovement>();
            movement.SetDirection(direction);

            var bulletLifetime = Entity.GetComponent<BulletLifetime>();
            bulletLifetime?.Reset();
        }

        private void OnBulletDestroyed(BulletDestroyedSignal signal)
        {
            _projectileSpawner.ReturnBullet(this);
        }

        private void OnCollisionEnter2D(Collision2D _collision)
        {
            OnBulletDestroyed(null);
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        private void RegisterTickableComponents()
        {
            foreach (var tickableComponent in Entity.GetTickableComponents())
            {
                _tickableManager.Add(tickableComponent);
            }
        }
    }
}
