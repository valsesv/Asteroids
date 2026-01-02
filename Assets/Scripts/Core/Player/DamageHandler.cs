using Cysharp.Threading.Tasks;
using Asteroids.Core.Entity.Components;
using Zenject;
using Asteroids.Core.Entity;
using UnityEngine;
using UnityEngine.Assertions;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles player damage and invincibility
    /// </summary>
    public class DamageHandler : IComponent
    {
        private readonly HealthComponent _healthComponent;
        private readonly ShipComponent _shipComponent;
        private readonly GameEntity _entity;
        private readonly SignalBus _signalBus;
        private readonly float _invincibilityDuration;
        private readonly float _bounceForce;

        public bool IsInvincible { get; private set; }

        /// <summary>
        /// Create damage handler with all required dependencies
        /// </summary>
        public DamageHandler(HealthComponent healthComponent, GameEntity entity, SignalBus signalBus, float invincibilityDuration = 3f, float bounceForce = 5f)
        {
            _healthComponent = healthComponent;
            _shipComponent = entity.GetComponent<ShipComponent>();
            _entity = entity;
            _signalBus = signalBus;
            _invincibilityDuration = invincibilityDuration;
            _bounceForce = bounceForce;
        }

        /// <summary>
        /// Handle collision with enemy - apply damage, bounce, and start invincibility
        /// </summary>
        public bool HandleCollision(GameEntity enemyEntity, float damage)
        {
            // Don't take damage if already dead
            if (_healthComponent.IsDead)
            {
                return false;
            }

            // Don't take damage if invincible
            if (IsInvincible)
            {
                return false;
            }

            // Apply bounce physics
            ApplyBounce(enemyEntity);

            // Take damage (1 health point per collision as per requirements)
            _healthComponent.TakeDamage(damage);

            // Apply damage to enemy
            var enemyHealth = enemyEntity?.GetComponent<HealthComponent>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Start invincibility period
            StartInvincibility().Forget();

            return true;
        }

        /// <summary>
        /// Apply bounce physics to both ship and enemy
        /// </summary>
        private void ApplyBounce(GameEntity enemyEntity)
        {
            var shipPhysics = _entity.GetComponent<PhysicsComponent>();

            Assert.IsNotNull(shipPhysics);

            // Get positions from transform components
            var shipTransform = _entity.GetComponent<TransformComponent>();
            var enemyTransform = enemyEntity?.GetComponent<TransformComponent>();

            Vector2 shipPosition = shipTransform.Position;
            Vector2 enemyPosition = enemyTransform.Position;

            // Calculate direction from enemy to ship
            Vector2 direction = (shipPosition - enemyPosition).normalized;

            // If direction is zero (objects are exactly on top of each other), use a random direction
            if (direction.magnitude < 0.01f)
            {
                float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
            }

            // Apply bounce impulse to ship (away from enemy)
            Vector2 shipImpulse = direction * _bounceForce;
            shipPhysics.ApplyImpulse(shipImpulse);
        }

        /// <summary>
        /// Start invincibility period (3 seconds, disable control)
        /// </summary>
        private async UniTaskVoid StartInvincibility()
        {
            IsInvincible = true;
            if (_shipComponent != null)
            {
                _shipComponent.CanControl = false;
            }

            // Fire invincibility started signal
            var invincibilitySignal = new InvincibilityChangedSignal
            {
                IsInvincible = true
            };
            _signalBus.Fire(invincibilitySignal);

            // Wait for invincibility duration (milliseconds)
            await UniTask.Delay(System.TimeSpan.FromSeconds(_invincibilityDuration));

            // End invincibility
            IsInvincible = false;
            if (_shipComponent != null)
            {
                _shipComponent.CanControl = true;
            }

            // Fire invincibility ended signal
            invincibilitySignal.IsInvincible = false;
            _signalBus.Fire(invincibilitySignal);
        }
    }
}

