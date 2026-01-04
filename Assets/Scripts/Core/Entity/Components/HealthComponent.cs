using System;
using UnityEngine;

namespace Asteroids.Core.Entity.Components
{
    public class HealthComponent : IComponent
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsDead => CurrentHealth <= 0f;
        public event Action OnDeath;

        public HealthComponent(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage)
        {
            if (IsDead)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        public void ResetHealth()
        {
            CurrentHealth = MaxHealth;
        }
    }
}