using UnityEngine;
using Zenject;
using Asteroids.Core.Entity;
using Asteroids.Core.Entity.Components;

namespace Asteroids.Core.Enemies
{
    public class UfoMovement : ITickableComponent
    {
        private readonly PhysicsComponent _physics;
        private readonly TransformComponent _transform;
        private readonly TransformComponent _playerTransform;
        private readonly SignalBus _signalBus;
        private readonly float _speed;

        private bool _isGameActive = true;

        public UfoMovement(
            GameEntity entity,
            TransformComponent playerTransform,
            float speed,
            SignalBus signalBus)
        {
            _physics = entity.GetComponent<PhysicsComponent>();
            _transform = entity.GetComponent<TransformComponent>();
            _playerTransform = playerTransform;
            _speed = speed;
            _signalBus = signalBus;

            _signalBus.Subscribe<GameOverSignal>(OnGameOver);
            _signalBus.Subscribe<GameStartedSignal>(OnGameStarted);
        }

        public void Tick()
        {
            if (!_isGameActive)
            {
                return;
            }

            Vector2 playerPosition = _playerTransform.Position;
            Vector2 ufoPosition = _transform.Position;
            Vector2 motionDirection = (playerPosition - ufoPosition).normalized;

            _physics.SetVelocity(motionDirection * _speed);
            _physics.ClampSpeed(_speed);
        }

        private void OnGameOver(GameOverSignal signal)
        {
            _isGameActive = false;
            _physics.SetVelocity(Vector2.zero);
        }

        private void OnGameStarted(GameStartedSignal signal)
        {
            _isGameActive = true;
        }
    }
}

