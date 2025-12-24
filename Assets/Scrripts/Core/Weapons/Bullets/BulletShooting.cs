using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Logic class for bullet shooting (not a component)
    /// </summary>
    public class BulletShootingLogic
    {
        private readonly TransformComponent _transform;
        private readonly IInputProvider _inputProvider;
        private readonly BulletSettings _bulletSettings;
        private readonly SignalBus _signalBus;
        private readonly BulletFactory _bulletFactory;

        private float _lastShotTime;

        public BulletShootingLogic(
            TransformComponent transform,
            IInputProvider inputProvider,
            BulletSettings bulletSettings,
            SignalBus signalBus,
            BulletFactory bulletFactory)
        {
            _transform = transform;
            _inputProvider = inputProvider;
            _bulletSettings = bulletSettings;
            _signalBus = signalBus;
            _bulletFactory = bulletFactory;
        }

        /// <summary>
        /// Try to shoot a bullet (called from WeaponShooting component)
        /// </summary>
        public void TryShoot()
        {
            // Check fire rate cooldown
            if (Time.time - _lastShotTime < 1f / _bulletSettings.FireRate)
            {
                return;
            }

            // Check input
            if (_inputProvider.GetShootBulletInput())
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            // Calculate bullet direction based on ship rotation
            float angle = _transform.Rotation * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Create bullet at ship position
            Vector2 bulletPosition = _transform.Position + direction * 0.5f; // Spawn slightly in front of ship
            _bulletFactory.CreateBullet(bulletPosition, direction, _bulletSettings.Speed, _bulletSettings.Lifetime);

            _lastShotTime = Time.time;

            // Fire signal for UI updates
            _signalBus?.Fire(new BulletShotSignal());
        }
    }
}

