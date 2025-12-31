using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Player
{
    /// <summary>
    /// Component that handles bullet lifetime and auto-destruction
    /// </summary>
    public class BulletLifetime : ITickableComponent
    {
        private readonly BulletComponent _bulletComponent;
        private readonly GameEntity _entity;
        private readonly SignalBus _signalBus;
        private float _elapsedTime;

        public BulletLifetime(GameEntity entity, SignalBus signalBus)
        {
            _entity = entity;
            _bulletComponent = entity.GetComponent<BulletComponent>();
            _signalBus = signalBus;
        }

        public void Tick()
        {
            _elapsedTime += Time.deltaTime;

            if (_elapsedTime >= _bulletComponent.Lifetime)
            {
                // Fire signal to destroy bullet
                _signalBus?.Fire(new BulletDestroyedSignal { Entity = _entity });
            }
        }

        /// <summary>
        /// Reset elapsed time when bullet is reused from pool
        /// </summary>
        public void Reset()
        {
            _elapsedTime = 0f;
        }
    }
}

