using Zenject;

namespace Asteroids.Core.Entity.Components
{
    public class HealthComponent : IComponent
    {
        private readonly SignalBus _signalBus;

        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;

        public HealthComponent(float maxHealth, SignalBus signalBus = null)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            _signalBus = signalBus;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead)
            {
                return;
            }

            CurrentHealth = System.Math.Max(0f, CurrentHealth - damage);

            if (IsDead)
            {
                _signalBus?.Fire<GameOverSignal>();
            }
        }

        public void ResetHealth()
        {
            CurrentHealth = MaxHealth;
        }
    }
}

