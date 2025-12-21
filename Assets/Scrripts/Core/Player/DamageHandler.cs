using Cysharp.Threading.Tasks;
using Asteroids.Core.Entity.Components;
using Zenject;
using Asteroids.Core.Entity;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles player damage and invincibility
    /// </summary>
    public class DamageHandler : IComponent
    {
        private readonly HealthComponent _healthComponent;
        private readonly ShipModel _shipModel;
        private readonly SignalBus _signalBus;
        private readonly float _invincibilityDuration;

        public bool IsInvincible { get; private set; }

        /// <summary>
        /// Create damage handler with all required dependencies
        /// </summary>
        public DamageHandler(HealthComponent healthComponent, ShipModel shipModel, SignalBus signalBus, float invincibilityDuration = 3f)
        {
            _healthComponent = healthComponent;
            _shipModel = shipModel;
            _signalBus = signalBus;
            _invincibilityDuration = invincibilityDuration;
        }

        /// <summary>
        /// Try to take damage from collision
        /// Returns true if damage was taken, false if invincible or dead
        /// </summary>
        public bool TryTakeDamage(float damage)
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

            // Take damage
            _healthComponent.TakeDamage(damage);

            // Start invincibility period
            StartInvincibility().Forget();

            return true;
        }

        /// <summary>
        /// Start invincibility period (3 seconds, disable control)
        /// </summary>
        private async UniTaskVoid StartInvincibility()
        {
            IsInvincible = true;
            _shipModel.CanControl = false;

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
            _shipModel.CanControl = true;

            // Fire invincibility ended signal
            invincibilitySignal.IsInvincible = false;
            _signalBus.Fire(invincibilitySignal);
        }
    }
}

