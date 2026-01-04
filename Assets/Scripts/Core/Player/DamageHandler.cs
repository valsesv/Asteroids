using System;
using Cysharp.Threading.Tasks;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Entity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Core.Player
{
    public class DamageHandler : IComponent
    {
        private readonly HealthComponent _healthComponent;
        private readonly ShipComponent _shipComponent;
        private readonly PhysicsComponent _physicsComponent;
        private readonly TransformComponent _transformComponent;
        private readonly float _invincibilityDuration;
        private readonly float _bounceForce;

        public bool IsInvincible { get; private set; }

        public event Action OnInvincibilityStarted;
        public event Action OnInvincibilityEnded;

        public DamageHandler(HealthComponent healthComponent, GameEntity entity, float invincibilityDuration, float bounceForce)
        {
            _healthComponent = healthComponent;
            _shipComponent = entity.GetComponent<ShipComponent>();
            _physicsComponent = entity.GetComponent<PhysicsComponent>();
            _transformComponent = entity.GetComponent<TransformComponent>();
            _invincibilityDuration = invincibilityDuration;
            _bounceForce = bounceForce;
        }

        public bool HandleCollision(GameEntity enemyEntity, float damage)
        {
            if (_healthComponent.IsDead)
            {
                return false;
            }

            if (IsInvincible)
            {
                return false;
            }

            ApplyBounce(enemyEntity);

            _healthComponent.TakeDamage(damage);

            if (_healthComponent.IsDead == false)
            {
                _ = StartInvincibilityAsync();
            }

            return true;
        }

        private void ApplyBounce(GameEntity enemyEntity)
        {
            var enemyTransform = enemyEntity?.GetComponent<TransformComponent>();

            Vector2 shipPosition = _transformComponent.Position;
            Vector2 enemyPosition = enemyTransform.Position;

            Vector2 direction = (shipPosition - enemyPosition).normalized;

            if (direction.magnitude < 0.01f)
            {
                float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
            }

            Vector2 shipImpulse = direction * _bounceForce;
            _physicsComponent.ApplyImpulse(shipImpulse);
        }

        private async UniTask StartInvincibilityAsync()
        {
            IsInvincible = true;
            _shipComponent.CanControl = false;
            OnInvincibilityStarted?.Invoke();

            await UniTask.Delay(TimeSpan.FromSeconds(_invincibilityDuration));

            IsInvincible = false;
            _shipComponent.CanControl = true;
            OnInvincibilityEnded?.Invoke();
        }
    }
}

