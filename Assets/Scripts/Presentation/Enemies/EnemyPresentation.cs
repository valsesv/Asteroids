using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;
using Asteroids.Presentation.Effects;

namespace Asteroids.Presentation.Enemies
{
    public abstract class EnemyPresentation : MonoBehaviour, IInitializable, IDisposable
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
            transformComponent.SetPosition(position);

            var screenWrap = Entity.GetComponent<ScreenWrapComponent>();
            screenWrap.Reset();
        }

        public virtual void GetDamage()
        {
            HandleInstaDeath();
        }

        public virtual void HandleInstaDeath()
        {
            if (!IsEnemyInGameArea())
            {
                return;
            }

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
            var bulletPresentation = collision.gameObject.GetComponent<BulletPresentation>();
            if (bulletPresentation != null)
            {
                GetDamage();
                return;
            }

            var shipPresentation = collision.gameObject.GetComponent<ShipPresentation>();
            if (shipPresentation == null)
            {
                return;
            }
            if (_isPlayerInvincible)
            {
                return;
            }

            GetDamage();
            return;
        }

        private bool IsEnemyInGameArea()
        {
            if (Entity == null)
            {
                return false;
            }

            var screenWrap = Entity.GetComponent<ScreenWrapComponent>();
            return screenWrap != null && screenWrap.IsInGameArea;
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

