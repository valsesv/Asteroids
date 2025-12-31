using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Logic class for laser shooting (not a component)
    /// </summary>
    public class LaserShootingLogic
    {
        private readonly TransformComponent _transform;
        private readonly IInputProvider _inputProvider;
        private readonly LaserSettings _laserSettings;
        private readonly LaserComponent _laserComponent;
        private readonly SignalBus _signalBus;

        public LaserShootingLogic(
            TransformComponent transform,
            IInputProvider inputProvider,
            LaserSettings laserSettings,
            LaserComponent laserComponent,
            SignalBus signalBus)
        {
            _transform = transform;
            _inputProvider = inputProvider;
            _laserSettings = laserSettings;
            _laserComponent = laserComponent;
            _signalBus = signalBus;
        }

        /// <summary>
        /// Try to shoot laser (called from WeaponShooting component)
        /// </summary>
        public void TryShoot()
        {
            // Check input
            if (!_inputProvider.GetShootLaserInput())
            {
                return;
            }

            // Check if we have charges
            if (!_laserComponent.TryConsumeCharge())
            {
                return;
            }

            Shoot();
        }

        private void Shoot()
        {
            // Calculate laser direction based on ship rotation
            float angle = (_transform.Rotation + 90f) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Calculate laser start position (spawn slightly in front of ship)
            Vector2 startPosition = _transform.Position + direction * 1f;

            _signalBus?.Fire(new LaserShotSignal
            {
                StartPosition = startPosition,
                Direction = direction
            });
        }
    }
}

