using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;

namespace Asteroids.Presentation.Player
{
    public class BulletPresentation : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; private set; }

        [Inject] private DiContainer _container;
        [Inject] private ProjectileSpawner _projectileSpawner;

        private TransformComponent _transformComponent;
        private BulletLifetime _bulletLifetime;

        [Inject]
        public void Construct(EntityFactory<BulletComponent> entityFactory)
        {
            var position = new Vector2(transform.position.x, transform.position.y);

            Entity = entityFactory.Create(position, 0f, physicsMass: 0.1f);

            _container.BindInstance(Entity).AsSingle();
        }

        public void Initialize()
        {
            _transformComponent = Entity?.GetComponent<TransformComponent>();
            _bulletLifetime = Entity?.GetComponent<BulletLifetime>();

            if (_bulletLifetime != null)
            {
                _bulletLifetime.OnExpired += OnBulletExpired;
            }
        }

        public void Dispose()
        {
            if (_bulletLifetime != null)
            {
                _bulletLifetime.OnExpired -= OnBulletExpired;
            }
        }

        private void OnBulletExpired()
        {
            _projectileSpawner.ReturnBullet(this);
        }

        private void LateUpdate()
        {
            if (_transformComponent != null)
            {
                transform.position = new Vector3(_transformComponent.Position.x, _transformComponent.Position.y, 0f);
                transform.rotation = Quaternion.Euler(0f, 0f, _transformComponent.Rotation);
            }
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

        private void OnCollisionEnter2D(Collision2D _)
        {
            _projectileSpawner.ReturnBullet(this);
        }
    }
}