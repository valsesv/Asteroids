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

        [Inject] private SignalBus _signalBus;

        [Inject]
        public void Construct(GameEntity entity)
        {
            Entity = entity;
        }

        public void Initialize()
        {
            Assert.IsNotNull(_invincibilityEffects, "InvincibilityEffects is not assigned in ShipPresentation!");

            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus.Subscribe<InvincibilityChangedSignal>(OnInvincibilityChanged);

            _invincibilityEffects.Initialize();
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus?.Unsubscribe<InvincibilityChangedSignal>(OnInvincibilityChanged);

            _invincibilityEffects?.Dispose();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"[ShipPresentation] Collision entered with: {collision.gameObject.name}");

            var enemyPresentation = collision.gameObject.GetComponent<EnemyPresentation>();
            if (enemyPresentation == null)
            {
                return;
            }

            var damageHandler = Entity?.GetComponent<DamageHandler>();
            if (damageHandler == null)
            {
                Debug.LogWarning("[ShipPresentation] DamageHandler not found on GameEntity!");
                return;
            }

            if (damageHandler.IsInvincible)
            {
                return;
            }

            GameEntity enemyEntity = enemyPresentation.Entity;
            bool damageTaken = damageHandler.HandleCollision(enemyEntity, 1f);
            if (damageTaken)
            {
                var healthComponent = Entity?.GetComponent<HealthComponent>();
                Debug.Log($"[ShipPresentation] Player took damage! Health: {healthComponent?.CurrentHealth}/{healthComponent?.MaxHealth}");
            }
        }

        private void OnInvincibilityChanged(InvincibilityChangedSignal signal)
        {
            if (signal.IsInvincible)
            {
                _invincibilityEffects.StartEffects();
            }
            else
            {
                _invincibilityEffects.StopEffects();
            }
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }
    }
}

