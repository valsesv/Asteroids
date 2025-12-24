using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity.Components;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Logic class for laser shooting with recharge system (not a component)
    /// </summary>
    public class LaserShootingLogic
    {
        private readonly TransformComponent _transform;
        private readonly IInputProvider _inputProvider;
        private readonly LaserSettings _laserSettings;
        private readonly SignalBus _signalBus;
        private readonly LaserFactory _laserFactory;

        private int _currentCharges;
        private bool _isRecharging = false;
        private CancellationTokenSource _rechargeCancellationTokenSource;

        public int CurrentCharges => _currentCharges;
        public int MaxCharges => _laserSettings.MaxCharges;
        public float RechargeTime => _laserSettings.RechargeTime;

        public LaserShootingLogic(
            TransformComponent transform,
            IInputProvider inputProvider,
            LaserSettings laserSettings,
            SignalBus signalBus,
            LaserFactory laserFactory)
        {
            _transform = transform;
            _inputProvider = inputProvider;
            _laserSettings = laserSettings;
            _signalBus = signalBus;
            _laserFactory = laserFactory;

            _currentCharges = _laserSettings.MaxCharges;
            _rechargeCancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Try to shoot laser (called from WeaponShooting component)
        /// </summary>
        public void TryShoot()
        {
            // Check input
            if (_inputProvider.GetShootLaserInput() && _currentCharges > 0)
            {
                Shoot();
            }

            // Start recharging if needed
            if (_currentCharges < _laserSettings.MaxCharges && !_isRecharging)
            {
                StartRecharging().Forget();
            }
        }

        private void Shoot()
        {
            // Calculate laser direction based on ship rotation
            float angle = _transform.Rotation * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Create laser
            _laserFactory.CreateLaser(_transform.Position, direction, _laserSettings.Duration, _laserSettings.Width);

            _currentCharges--;
            FireLaserChargesChangedSignal();
        }

        private async UniTaskVoid StartRecharging()
        {
            _isRecharging = true;

            while (_currentCharges < _laserSettings.MaxCharges)
            {
                await UniTask.Delay((int)(_laserSettings.RechargeTime * 1000), cancellationToken: _rechargeCancellationTokenSource.Token);

                if (_rechargeCancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                _currentCharges++;
                FireLaserChargesChangedSignal();
            }

            _isRecharging = false;
        }

        private void FireLaserChargesChangedSignal()
        {
            _signalBus?.Fire(new LaserChargesChangedSignal
            {
                CurrentCharges = _currentCharges,
                MaxCharges = _laserSettings.MaxCharges,
                RechargeTime = _laserSettings.RechargeTime
            });
        }

        public void Dispose()
        {
            _rechargeCancellationTokenSource?.Cancel();
            _rechargeCancellationTokenSource?.Dispose();
        }
    }
}

