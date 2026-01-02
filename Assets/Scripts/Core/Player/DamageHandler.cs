using Cysharp.Threading.Tasks;
using Asteroids.Core.Entity.Components;
using Zenject;
using Asteroids.Core.Entity;
using UnityEngine;
using UnityEngine.Assertions;

namespace Asteroids.Core.Player
{
    public class DamageHandler : IComponent
    {
        private readonly HealthComponent _healthComponent;
        private readonly ShipComponent _shipComponent;
        private readonly GameEntity _entity;
        private readonly SignalBus _signalBus;
        private readonly float _invincibilityDuration;
        private readonly float _bounceForce;

        public bool IsInvincible { get; private set; }

        public DamageHandler(HealthComponent healthComponent, GameEntity entity, SignalBus signalBus, float invincibilityDuration = 3f, float bounceForce = 5f)
        {
            _healthComponent = healthComponent;
            _shipComponent = entity.GetComponent<ShipComponent>();
            _entity = entity;
            _signalBus = signalBus;
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
            StartInvincibility().Forget();

            return true;
        }

        private void ApplyBounce(GameEntity enemyEntity)
        {
            var shipPhysics = _entity.GetComponent<PhysicsComponent>();

            Assert.IsNotNull(shipPhysics);


            var shipTransform = _entity.GetComponent<TransformComponent>();
            var enemyTransform = enemyEntity?.GetComponent<TransformComponent>();

            Vector2 shipPosition = shipTransform.Position;
            Vector2 enemyPosition = enemyTransform.Position;

            Vector2 direction = (shipPosition - enemyPosition).normalized;

            if (direction.magnitude < 0.01f)
            {
                float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
            }

            Vector2 shipImpulse = direction * _bounceForce;
            shipPhysics.ApplyImpulse(shipImpulse);
        }

        private async UniTaskVoid StartInvincibility()
        {
            IsInvincible = true;
            if (_shipComponent != null)
            {
                _shipComponent.CanControl = false;
            }

            var invincibilitySignal = new InvincibilityChangedSignal
            {
                IsInvincible = true
            };
            _signalBus.Fire(invincibilitySignal);

            await UniTask.Delay(System.TimeSpan.FromSeconds(_invincibilityDuration));

            IsInvincible = false;
            if (_shipComponent != null && !_healthComponent.IsDead)
            {
                _shipComponent.CanControl = true;
            }

            invincibilitySignal.IsInvincible = false;
            _signalBus.Fire(invincibilitySignal);
        }
    }
}

