using UnityEngine;
using Zenject;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Weapons;

namespace Asteroids.Core.Player
{
    public class BulletShootingLogic
    {
        private readonly TransformComponent _transform;
        private readonly IInputProvider _inputProvider;
        private readonly BulletSettings _bulletSettings;
        private readonly SignalBus _signalBus;

        private float _lastShotTime;

        public BulletShootingLogic(
            TransformComponent transform,
            IInputProvider inputProvider,
            BulletSettings bulletSettings,
            SignalBus signalBus)
        {
            _transform = transform;
            _inputProvider = inputProvider;
            _bulletSettings = bulletSettings;
            _signalBus = signalBus;
        }

        public void TryShoot()
        {
            if (Time.time - _lastShotTime < 1f / _bulletSettings.FireRate)
            {
                return;
            }

            if (_inputProvider.GetShootBulletInput())
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            float angle = (_transform.Rotation + 90f) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            Vector2 bulletPosition = _transform.Position + direction * 1f;

            _signalBus?.Fire(new BulletShotSignal
            {
                Position = bulletPosition,
                Direction = direction
            });

            _lastShotTime = Time.time;
        }
    }
}

