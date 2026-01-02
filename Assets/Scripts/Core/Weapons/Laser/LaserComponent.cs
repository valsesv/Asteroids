using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Weapons;

namespace Asteroids.Core.Player
{
    public class LaserComponent : ITickableComponent
    {
        private readonly LaserSettings _settings;
        private readonly SignalBus _signalBus;
        private readonly LaserChargesChangedSignal _chargesChangedSignal = new LaserChargesChangedSignal();

        public int CurrentCharges { get; private set; }
        private float _rechargeTimer;
        private bool _isRecharging;

        public float RechargeProgress => _rechargeTimer / _settings.RechargeTime;

        public LaserComponent(LaserSettings settings, SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
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
            FireChargesChangedSignal();
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
                FireChargesChangedSignal();

                if (CurrentCharges >= _settings.MaxCharges)
                {
                    _isRecharging = false;
                }
            }
            else
            {
                FireChargesChangedSignal();
            }
        }

        private void FireChargesChangedSignal()
        {
            _chargesChangedSignal.CurrentCharges = CurrentCharges;
            _chargesChangedSignal.MaxCharges = _settings.MaxCharges;
            _chargesChangedSignal.RechargeProgress = RechargeProgress;
            _signalBus.Fire(_chargesChangedSignal);
        }
    }
}

