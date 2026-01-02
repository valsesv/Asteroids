using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    public class LaserShootingLogic
    {
        private readonly TransformComponent _transform;
        private readonly IInputProvider _inputProvider;
        private readonly LaserComponent _laserComponent;
        private readonly SignalBus _signalBus;

        public LaserShootingLogic(
            TransformComponent transform,
            IInputProvider inputProvider,
            LaserComponent laserComponent,
            SignalBus signalBus)
        {
            _transform = transform;
            _inputProvider = inputProvider;
            _laserComponent = laserComponent;
            _signalBus = signalBus;
        }

        public void TryShoot()
        {
            if (!_inputProvider.GetShootLaserInput())
            {
                return;
            }

            if (!_laserComponent.TryConsumeCharge())
            {
                return;
            }

            Shoot();
        }

        private void Shoot()
        {
            float angle = (_transform.Rotation + 90f) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector2 startPosition = _transform.Position + direction * 1f;

            _signalBus?.Fire(new LaserShotSignal
            {
                StartPosition = startPosition,
                Direction = direction
            });
        }
    }
}

