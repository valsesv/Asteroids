using System;
using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Presentation.Enemies;
using UnityEngine.Assertions;

namespace Asteroids.Presentation.Player
{
    public class ShipPresentation : MonoBehaviour, IInitializable, IDisposable
    {
        public GameEntity Entity { get; private set; }

        [SerializeField] private InvincibilityEffects _invincibilityEffects;

        private TransformComponent _transformComponent;
        private DamageHandler _damageHandler;
        private HealthComponent _healthComponent;

        [Inject]
        public void Construct(GameEntity entity)
        {
            Entity = entity;

            _transformComponent = Entity?.GetComponent<TransformComponent>();
            _damageHandler = Entity?.GetComponent<DamageHandler>();
            _healthComponent = Entity?.GetComponent<HealthComponent>();
        }

        public void Initialize()
        {
            Assert.IsNotNull(_invincibilityEffects, "InvincibilityEffects is not assigned in ShipPresentation!");

            _invincibilityEffects.Initialize();

            _damageHandler.OnInvincibilityStarted += OnInvincibilityStarted;
            _damageHandler.OnInvincibilityEnded += OnInvincibilityEnded;
        }

        public void Dispose()
        {
            _damageHandler.OnInvincibilityStarted -= OnInvincibilityStarted;
            _damageHandler.OnInvincibilityEnded -= OnInvincibilityEnded;

            _invincibilityEffects?.Dispose();
        }

        private void LateUpdate()
        {
            transform.position = new Vector3(_transformComponent.Position.x, _transformComponent.Position.y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, _transformComponent.Rotation);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"[ShipPresentation] Collision entered with: {collision.gameObject.name}");

            var enemyPresentation = collision.gameObject.GetComponent<EnemyPresentation>();
            if (enemyPresentation == null)
            {
                return;
            }

            if (_damageHandler.IsInvincible)
            {
                return;
            }

            GameEntity enemyEntity = enemyPresentation.Entity;
            bool damageTaken = _damageHandler.HandleCollision(enemyEntity, 1f);
            Debug.Log($"[ShipPresentation] Player took damage! Health: {_healthComponent?.CurrentHealth}/{_healthComponent?.MaxHealth}");
        }

        private void OnInvincibilityStarted()
        {
            _invincibilityEffects.StartEffects();
        }

        private void OnInvincibilityEnded()
        {
            _invincibilityEffects.StopEffects();
        }
    }
}

