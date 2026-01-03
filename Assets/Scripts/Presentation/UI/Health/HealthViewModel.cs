using System;
using Zenject;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;

namespace Asteroids.Presentation.UI
{
    public class HealthViewModel : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly HealthSettings _healthSettings;

        private float _currentHealth;

        public float CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                if (_currentHealth != value)
                {
                    _currentHealth = value;
                    OnHealthChanged?.Invoke(_currentHealth);
                }
            }
        }

        public event Action<float> OnHealthChanged;

        public HealthViewModel(SignalBus signalBus, HealthSettings healthSettings)
        {
            _signalBus = signalBus;
            _healthSettings = healthSettings;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<HealthChangedSignal>(OnHealthChangedSignal);

            CurrentHealth = _healthSettings.MaxHealth;
        }

        public void Dispose()
        {
            _signalBus?.Unsubscribe<HealthChangedSignal>(OnHealthChangedSignal);
        }

        private void OnHealthChangedSignal(HealthChangedSignal signal)
        {
            CurrentHealth = signal.CurrentHealth;
        }
    }
}