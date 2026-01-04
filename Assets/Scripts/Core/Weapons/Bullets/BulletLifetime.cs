using System;
using UnityEngine;
using Asteroids.Core.Entity;

namespace Asteroids.Core.Player
{
    public class BulletLifetime : ITickableComponent
    {
        private readonly BulletComponent _bulletComponent;
        private float _elapsedTime;
        private bool _hasExpired;

        public event Action OnExpired;

        public BulletLifetime(GameEntity entity)
        {
            _bulletComponent = entity.GetComponent<BulletComponent>();
        }

        public void Tick()
        {
            if (_hasExpired)
            {
                return;
            }

            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _bulletComponent.Lifetime)
            {
                _hasExpired = true;
                OnExpired?.Invoke();
            }
        }

        public void Reset()
        {
            _elapsedTime = 0f;
            _hasExpired = false;
        }
    }
}