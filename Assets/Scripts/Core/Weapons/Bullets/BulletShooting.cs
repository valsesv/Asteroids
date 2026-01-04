using System;
using UnityEngine;
using Asteroids.Core.PlayerInput;
using Asteroids.Core.Entity.Components;
using Asteroids.Core.Weapons;

namespace Asteroids.Core.Player
{
    public class BulletShootingLogic
    {
        private TransformComponent _transform;
        private readonly IInputProvider _inputProvider;
        private readonly BulletSettings _bulletSettings;

        private float _lastShotTime;

        public event Action<Vector2, Vector2> OnBulletShot;

        public BulletShootingLogic(
            IInputProvider inputProvider,
            BulletSettings bulletSettings)
        {
            _inputProvider = inputProvider;
            _bulletSettings = bulletSettings;
        }

        public void Init(TransformComponent transform)
        {
            _transform = transform;
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

            OnBulletShot?.Invoke(bulletPosition, direction);

            _lastShotTime = Time.time;
        }
    }
}