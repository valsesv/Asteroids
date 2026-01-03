using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Presentation.Player;
using Asteroids.Presentation.Effects;
using Asteroids.Core.Player;

namespace Asteroids.Presentation.Enemies
{
    public abstract class EnemyPresentation : MonoBehaviour, IInitializable
    {
        public GameEntity Entity { get; protected set; }

        [Inject] protected EnemySpawner _enemySpawner;
        [Inject] protected TickableManager _tickableManager;
        [Inject] protected DiContainer _container;
        [Inject(Optional = true)] protected ParticleEffectSpawner _particleEffectSpawner;

        private TransformComponent _transformComponent;
        private GameEntity _playerEntity;
        private bool _isPlayerInvincible;

        public virtual void Initialize()
        {
            _transformComponent = Entity?.GetComponent<TransformComponent>();
        }

        public void SetPlayerEntity(GameEntity playerEntity)
        {
            _playerEntity = playerEntity;
        }

        private void LateUpdate()
        {
            if (_transformComponent != null)
            {
                transform.position = new Vector3(_transformComponent.Position.x, _transformComponent.Position.y, 0f);
                transform.rotation = Quaternion.Euler(0f, 0f, _transformComponent.Rotation);
            }
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

        protected virtual void RegisterTickableComponents()
        {
            foreach (var tickableComponent in Entity.GetTickableComponents())
            {
                _tickableManager.Add(tickableComponent);
            }
        }

        private void Update()
        {
            if (_playerEntity != null)
            {
                var damageHandler = _playerEntity.GetComponent<DamageHandler>();
                if (damageHandler != null)
                {
                    _isPlayerInvincible = damageHandler.IsInvincible;
                }
            }
        }
    }
}

