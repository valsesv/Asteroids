using UnityEngine;
using Asteroids.Core.Weapons;
using Asteroids.Core.Entity;

namespace Asteroids.Core.Player
{
    public class LaserComponent : ITickableComponent
    {
        private readonly LaserSettings _settings;

        public int CurrentCharges { get; private set; }
        public int MaxCharges => _settings.MaxCharges;
        private float _rechargeTimer;
        private bool _isRecharging;

        public float RechargeProgress => _rechargeTimer / _settings.RechargeTime;

        public LaserComponent(LaserSettings settings)
        {
            _settings = settings;
            CurrentCharges = _settings.MaxCharges;
            _rechargeTimer = 0f;
            _isRecharging = false;
        }

        public bool TryConsumeCharge()
        {
            if (CurrentCharges <= 0)
            {
                return false;
            }

            CurrentCharges--;
            return true;
        }

        public void Tick()
        {
            if (CurrentCharges >= _settings.MaxCharges)
            {
                _rechargeTimer = 0f;
                _isRecharging = false;
                return;
            }

            if (!_isRecharging)
            {
                _isRecharging = true;
                _rechargeTimer = 0f;
            }

            _rechargeTimer += Time.deltaTime;

            if (_rechargeTimer >= _settings.RechargeTime)
            {
                CurrentCharges++;
                _rechargeTimer = 0f;

                if (CurrentCharges >= _settings.MaxCharges)
                {
                    _isRecharging = false;
                }
            }
        }
    }
}

