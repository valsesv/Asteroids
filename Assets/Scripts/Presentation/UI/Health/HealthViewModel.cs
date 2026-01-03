using System;
using Zenject;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Player;
using Asteroids.Core.Entity;

namespace Asteroids.Presentation.UI
{
    public class HealthViewModel : ITickable
    {
        private readonly GameEntity _playerEntity;
        private readonly HealthSettings _healthSettings;
        private HealthComponent _healthComponent;

        public float CurrentHealth { get; private set; }

        public event Action<float> OnHealthChanged;

        public HealthViewModel(GameEntity playerEntity, HealthSettings healthSettings)
        {
            _playerEntity = playerEntity;
            _healthSettings = healthSettings;
            _healthComponent = _playerEntity?.GetComponent<HealthComponent>();
            CurrentHealth = _healthSettings.MaxHealth;
        }

        public void Tick()
        {
            if (_healthComponent != null && CurrentHealth != _healthComponent.CurrentHealth)
            {
                CurrentHealth = _healthComponent.CurrentHealth;
                OnHealthChanged?.Invoke(CurrentHealth);
            }
        }
    }
}