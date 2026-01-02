using Zenject;

namespace Asteroids.Core.Entity.Components
{
    public class HealthComponent : IComponent
    {
        private readonly HealthChangedSignal _signal = new HealthChangedSignal();
        private readonly SignalBus _signalBus;

        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;

        public HealthComponent(SignalBus signalBus, float maxHealth)
        {
            _signalBus = signalBus;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            FireSignal();
        }

        public void TakeDamage(float damage)
        {
            if (IsDead)
            {
                return;
            }

            CurrentHealth = System.Math.Max(0f, CurrentHealth - damage);
            FireSignal();
        }

        public void ResetHealth()
        {
            CurrentHealth = MaxHealth;
            FireSignal();
        }

        private void FireSignal()
        {
            if (_signalBus == null)
            {
                return;
            }

            _signal.CurrentHealth = CurrentHealth;
            _signal.MaxHealth = MaxHealth;
            _signalBus.Fire(_signal);
        }
    }
}

