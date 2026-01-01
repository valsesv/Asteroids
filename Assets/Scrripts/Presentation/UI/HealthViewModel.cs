using System;
using Zenject;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;

namespace Asteroids.Presentation.UI
{
    /// <summary>
    /// ViewModel for health display (MVVM pattern)
    /// Non-MonoBehaviour class that manages health state for UI
    /// </summary>
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
            // Subscribe to health changes first
            _signalBus.Subscribe<HealthChangedSignal>(OnHealthChangedSignal);

            // Initialize current health from settings (starts at max health)
            // This will trigger OnHealthChanged event so View gets initial value
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