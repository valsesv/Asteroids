using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that manages laser charges and recharge logic
    /// </summary>
    public class LaserComponent : ITickableComponent
    {
        private readonly LaserSettings _settings;
        private readonly SignalBus _signalBus;
        private readonly LaserChargesChangedSignal _chargesChangedSignal = new LaserChargesChangedSignal();

        private int _currentCharges;
        private float _rechargeTimer;
        private bool _isRecharging;

        public int CurrentCharges => _currentCharges;
        public int MaxCharges => _settings.MaxCharges;
        public float RechargeProgress => _rechargeTimer / _settings.RechargeTime;

        public LaserComponent(LaserSettings settings, SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
            _currentCharges = _settings.MaxCharges;
            _rechargeTimer = 0f;
            _isRecharging = false;
        }

        /// <summary>
        /// Try to consume a laser charge
        /// Returns true if charge was consumed, false if no charges available
        /// </summary>
        public bool TryConsumeCharge()
        {
            if (_currentCharges <= 0)
            {
                return false;
            }

            _currentCharges--;
            FireChargesChangedSignal();
            return true;
        }

        /// <summary>
        /// Tick - handles recharge logic
        /// </summary>
        public void Tick()
        {
            // If we have max charges, don't recharge
            if (_currentCharges >= _settings.MaxCharges)
            {
                _rechargeTimer = 0f;
                _isRecharging = false;
                return;
            }

            // Start recharging if not already
            if (!_isRecharging)
            {
                _isRecharging = true;
                _rechargeTimer = 0f;
            }

            // Increment recharge timer
            _rechargeTimer += Time.deltaTime;

            // Check if we can add a charge
            if (_rechargeTimer >= _settings.RechargeTime)
            {
                _currentCharges++;
                _rechargeTimer = 0f;
                FireChargesChangedSignal();

                // Continue recharging if we don't have max charges
                if (_currentCharges >= _settings.MaxCharges)
                {
                    _isRecharging = false;
                }
            }
            else
            {
                // Fire signal to update UI with progress
                FireChargesChangedSignal();
            }
        }

        private void FireChargesChangedSignal()
        {
            if (_signalBus != null)
            {
                _chargesChangedSignal.CurrentCharges = _currentCharges;
                _chargesChangedSignal.MaxCharges = _settings.MaxCharges;
                _chargesChangedSignal.RechargeProgress = RechargeProgress;
                _signalBus.Fire(_chargesChangedSignal);
            }
        }
    }
}

