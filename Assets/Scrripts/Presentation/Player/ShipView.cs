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
    /// <summary>
    /// Ship view - MonoBehaviour that represents the ship in the scene
    /// Subscribes to component signals directly (no ViewModel needed for game objects)
    /// </summary>
    public class ShipView : MonoBehaviour, IInitializable, IDisposable
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
            _signalBus.Subscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus.Subscribe<PhysicsChangedSignal>(OnPhysicsChanged);
            _signalBus.Subscribe<InvincibilityChangedSignal>(OnInvincibilityChanged);

            // Initialize invincibility effects
            if (_invincibilityEffects != null)
            {
                _invincibilityEffects.Initialize();
            }
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<TransformChangedSignal>(OnTransformChanged);
            _signalBus?.Unsubscribe<PhysicsChangedSignal>(OnPhysicsChanged);
            _signalBus?.Unsubscribe<InvincibilityChangedSignal>(OnInvincibilityChanged);

            // Cleanup effects
            _invincibilityEffects?.Dispose();
        }

        private void OnTransformChanged(TransformChangedSignal signal)
        {
            transform.position = new Vector3(signal.X, signal.Y, 0f);
            transform.rotation = Quaternion.Euler(0f, 0f, signal.Rotation);
        }

        private void OnPhysicsChanged(PhysicsChangedSignal signal)
        {
            // Can be used for visual effects based on speed
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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"[ShipView] Collision entered with: {collision.gameObject.name}");

            var enemyView = collision.gameObject.GetComponent<EnemyView>();
            if (enemyView == null)
            {
                return;
            }

            // Get damage handler from entity (handles damage, bounce, and invincibility in Core)
            var damageHandler = Entity?.GetComponent<DamageHandler>();
            if (damageHandler == null)
            {
                Debug.LogWarning("[ShipView] DamageHandler not found on GameEntity!");
                return;
            }

            // Don't process collision if player is invincible
            if (damageHandler.IsInvincible)
            {
                return;
            }

            // Handle collision (applies bounce, damage, and starts invincibility)
            GameEntity enemyEntity = enemyView.Entity;
            bool damageTaken = damageHandler.HandleCollision(enemyEntity, 1f);
            if (damageTaken)
            {
                var healthComponent = Entity?.GetComponent<HealthComponent>();
                Debug.Log($"[ShipView] Player took damage! Health: {healthComponent?.CurrentHealth}/{healthComponent?.MaxHealth}");
            }
        }
    }
}

