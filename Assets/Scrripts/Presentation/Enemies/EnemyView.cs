using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;
using Asteroids.Core.Enemies;

namespace Asteroids.Presentation.Enemies
{
    /// <summary>
    /// Base enemy view - MonoBehaviour that represents enemy in the scene
    /// Subscribes to component signals for position/rotation updates
    /// </summary>
    public abstract class EnemyView : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; protected set; }

        [Inject] protected SignalBus _signalBus;
        [Inject] protected EnemySpawner _enemySpawner;
        [Inject] protected TickableManager _tickableManager;
        [Inject] protected DiContainer _container;

        public virtual void Initialize()
        {
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
        }

        protected virtual void RegisterTickableComponents()
        {
            foreach (var tickableComponent in Entity.GetTickableComponents())
            {
                _tickableManager.Add(tickableComponent);
            }
        }

        /// <summary>
        /// Unregister tickable components from TickableManager
        /// </summary>
        private void UnregisterTickableComponents()
        {
            foreach (var tickableComponent in Entity.GetTickableComponents())
            {
                try
                {
                    _tickableManager.Remove(tickableComponent);
                }
                catch
                {
                    // Component might not be registered, ignore
                }
            }
        }

        protected virtual void OnTransformChanged(TransformChangedSignal signal)
        {
            // Update Unity transform from signal
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        /// <summary>
        /// Set spawn position - updates both Unity transform and TransformComponent
        /// Call this when spawning or reusing enemy from pool
        /// </summary>
        public void SetSpawnPosition(Vector2 position)
        {
            // Unregister tickable components before setting spawn position
            UnregisterTickableComponents();

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

            // Reset ScreenWrapComponent flag when reusing from pool
            var screenWrap = Entity.GetComponent<ScreenWrapComponent>();
            if (screenWrap != null)
            {
                screenWrap.Reset();
            }
            RegisterTickableComponents();
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            // Check if collision is with a bullet/projectile
            var bulletView = collision.gameObject.GetComponent<BulletView>();
            if (bulletView == null || Entity == null)
            {
                return;
            }

            // All enemies die immediately on bullet hit (no health system)
            HandleEnemyDeath();
        }

        /// <summary>
        /// Handle enemy death - override in derived classes for specific behavior
        /// </summary>
        protected virtual void HandleEnemyDeath()
        {
        }
    }
}

