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
    /// </summary>
    public class BulletView : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; private set; }

        [Inject] private SignalBus _signalBus;
        [Inject] private TickableManager _tickableManager;

        [Inject]
        public void Construct(GameEntity entity)
        {
            Entity = entity;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);

            // Initialize position from Entity if available
            if (Entity != null)
            {
                var transformComponent = Entity.GetComponent<TransformComponent>();
                if (transformComponent != null)
                {
                    transform.position = new Vector3(transformComponent.Position.x, transformComponent.Position.y, 0f);
                    transform.rotation = Quaternion.Euler(0f, 0f, transformComponent.Rotation);
                }

                // Note: ITickable components are registered in ProjectileSpawner to avoid duplication
            }
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            // Update Unity transform from signal
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var enemyView = other.GetComponent<Enemies.EnemyView>();
            if (enemyView != null && Entity != null)
            {
                var collisionHandler = Entity.GetComponent<BulletCollisionHandler>();
                if (collisionHandler != null)
                {
                    collisionHandler.HandleCollision(enemyView.Entity);
                }
            }
        }
    }
}

