using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;
using Asteroids.Presentation.Effects;

namespace Asteroids.Presentation.Enemies
{
    public abstract class EnemyView : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; protected set; }

        [Inject] protected SignalBus _signalBus;
        [Inject] protected EnemySpawner _enemySpawner;
        [Inject] protected TickableManager _tickableManager;
        [Inject] protected DiContainer _container;
        [Inject(Optional = true)] protected ParticleEffectSpawner _particleEffectSpawner;

        private bool _isPlayerInvincible;

        public virtual void Initialize()
        {
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus.Subscribe<InvincibilityChangedSignal>(OnInvincibilityChanged);
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus?.Unsubscribe<InvincibilityChangedSignal>(OnInvincibilityChanged);
        }

        public void SetSpawnPosition(Vector2 position)
        {
            transform.position = new Vector3(position.x, position.y, 0f);

            if (Entity == null)
            {
                return;
            }

            var transformComponent = Entity.GetComponent<TransformComponent>();
            if (transformComponent != null)
            {
                transformComponent.SetPosition(position);
            }

            var screenWrap = Entity.GetComponent<ScreenWrapComponent>();
            if (screenWrap != null)
            {
                screenWrap.Reset();
            }
        }

        public virtual void HandleEnemyDeath()
        {
            HandleInstaDeath();
        }

        public virtual void HandleInstaDeath()
        {
            if (_particleEffectSpawner != null)
            {
                _particleEffectSpawner.SpawnExplosion(transform.position);
            }

            if (_enemySpawner != null)
            {
                _enemySpawner.ReturnEnemy(this);
            }

            if (_signalBus != null && Entity != null)
            {
                _signalBus.Fire(new EnemyDestroyedSignal { Entity = Entity });
            }
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            var bulletView = collision.gameObject.GetComponent<BulletView>();
            if (bulletView != null)
            {
                HandleEnemyDeath();
                return;
            }

            var shipView = collision.gameObject.GetComponent<ShipView>();
            if (shipView != null)
            {
                if (_isPlayerInvincible)
                {
                    return;
                }

                HandleEnemyDeath();
                return;
            }
        }

        protected virtual void OnTransformChanged(TransformChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        protected virtual void RegisterTickableComponents()
        {
            foreach (var tickableComponent in Entity.GetTickableComponents())
            {
                _tickableManager.Add(tickableComponent);
            }
        }

        private void OnInvincibilityChanged(InvincibilityChangedSignal signal)
        {
            _isPlayerInvincible = signal.IsInvincible;
        }
    }
}

